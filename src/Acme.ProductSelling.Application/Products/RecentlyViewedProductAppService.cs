using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Products
{
    public class RecentlyViewedProductAppService : ProductSellingAppService, IRecentlyViewedProductAppService
    {
        private readonly IRecentlyViewedProductRepository _recentlyViewedRepository;
        private readonly IProductRepository _productRepository;
        // Inject the specific mapper
        private readonly ProductToRecentlyViewedProductDtoMapper _productMapper;

        private const int MAX_QUERY_BATCH_SIZE = 50;
        private const int MAX_GUEST_IDS_TO_PROCESS = 20;

        public RecentlyViewedProductAppService(
            IRecentlyViewedProductRepository recentlyViewedRepository,
            IProductRepository productRepository,
            ProductToRecentlyViewedProductDtoMapper productMapper)
        {
            _recentlyViewedRepository = recentlyViewedRepository;
            _productRepository = productRepository;
            _productMapper = productMapper;
        }

        public async Task<List<RecentlyViewedProductDto>> GetListAsync(GetRecentlyViewedInputDtos input)
        {
            if (input.MaxCount <= 0 || input.MaxCount > 100)
            {
                input.MaxCount = 6; // Safe default
            }

            var orderedProductIds = new List<Guid>();

            if (CurrentUser.IsAuthenticated)
            {
                try
                {
                    var dbHistory = await _recentlyViewedRepository.GetByUserIdAsync(
                        CurrentUser.GetId(),
                        input.MaxCount);

                    orderedProductIds.AddRange(dbHistory.Select(x => x.ProductId));
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Failed to fetch user recently viewed history");
                    // Continue with guest IDs if database fails
                }
            }
            if (input.GuestProductIds?.Any() == true)
            {
                var guestIdsToProcess = input.GuestProductIds
                    .Take(MAX_GUEST_IDS_TO_PROCESS)
                    .ToList();

                var existingIds = orderedProductIds.ToHashSet();
                foreach (var guestId in guestIdsToProcess)
                {
                    if (existingIds.Add(guestId))
                    {
                        orderedProductIds.Add(guestId);
                    }

                    // Safety break if we have enough
                    if (orderedProductIds.Count >= input.MaxCount * 2)
                    {
                        break;
                    }
                }
            }

            var targetIds = orderedProductIds
                .Take(Math.Min(input.MaxCount, MAX_QUERY_BATCH_SIZE))
                .ToList();

            if (!targetIds.Any())
            {
                return new List<RecentlyViewedProductDto>();
            }
            try
            {
                var query = await _productRepository.GetQueryableAsync();

                var products = await AsyncExecuter.ToListAsync(
                    query
                    .AsNoTracking() // No change tracking needed
                    .Include(p => p.StoreInventories) // Keep necessary include
                    .Where(p => targetIds.Contains(p.Id) && p.IsActive)
                    .Take(MAX_QUERY_BATCH_SIZE) // Additional safety limit
                );

                var productDict = products.ToDictionary(p => p.Id);

                return targetIds
                    .Select(id => productDict.ContainsKey(id) ? productDict[id] : null)
                    .Where(p => p != null)
                    .Select(p => _productMapper.Map(p))
                    .ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to fetch recently viewed products");
                return new List<RecentlyViewedProductDto>();
            }
        }

        [Authorize]
        public async Task SyncGuestHistoryAsync(SyncGuestHistoryInput input)
        {
            if (input.ProductIds == null || !input.ProductIds.Any())
            {
                return;
            }
            var idsToSync = input.ProductIds
               .Distinct()
               .Take(MAX_GUEST_IDS_TO_PROCESS)
               .ToList();
            try
            {
                var userId = CurrentUser.GetId();

                // OPTIMIZATION 8: Batch validate product IDs efficiently
                var validProductIds = await AsyncExecuter.ToListAsync(
                    (await _productRepository.GetQueryableAsync())
                    .AsNoTracking()
                    .Where(p => idsToSync.Contains(p.Id))
                    .Select(p => p.Id)
                );

                var validIdsSet = validProductIds.ToHashSet();

                // OPTIMIZATION 9: Fetch only necessary data
                var existingViews = await _recentlyViewedRepository.GetByUserIdAsync(
                    userId,
                    RecentlyViewedConsts.MaxItemsPerUser * 2);

                var existingProductMap = existingViews.ToDictionary(x => x.ProductId);

                // Process IDs (oldest first for proper ordering)
                var toProcess = idsToSync
                    .Where(id => validIdsSet.Contains(id))
                    .Reverse()
                    .ToList();

                var newItems = new List<RecentlyViewedProduct>();
                var itemsToUpdate = new List<RecentlyViewedProduct>();

                foreach (var productId in toProcess)
                {
                    if (existingProductMap.TryGetValue(productId, out var existingItem))
                    {
                        existingItem.UpdateViewedTime();
                        itemsToUpdate.Add(existingItem);
                    }
                    else
                    {
                        var newItem = new RecentlyViewedProduct(
                            GuidGenerator.Create(),
                            userId,
                            productId,
                            DateTime.UtcNow);

                        newItems.Add(newItem);
                        existingProductMap[productId] = newItem; // Prevent duplicates
                    }
                }

                // OPTIMIZATION 10: Batch database operations
                if (itemsToUpdate.Any())
                {
                    await _recentlyViewedRepository.UpdateManyAsync(itemsToUpdate);
                }

                if (newItems.Any())
                {
                    await _recentlyViewedRepository.InsertManyAsync(newItems);
                }

                // Clean up old items
                await _recentlyViewedRepository.DeleteOldestForUserAsync(
                    userId,
                    RecentlyViewedConsts.MaxItemsPerUser);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to sync guest history for user {UserId}", CurrentUser.GetId());
                throw; // Re-throw to let caller handle
            }
        }

        [Authorize]
        public async Task TrackProductViewAsync(Guid productId)
        {
            try
            {
                var userId = CurrentUser.GetId();

                // OPTIMIZATION 11: Use AnyAsync instead of full query
                var productExists = await _productRepository.AnyAsync(p => p.Id == productId);
                if (!productExists)
                {
                    return;
                }

                var existing = await _recentlyViewedRepository.FindByUserAndProductAsync(userId, productId);

                if (existing != null)
                {
                    existing.UpdateViewedTime();
                    await _recentlyViewedRepository.UpdateAsync(existing);
                }
                else
                {
                    await _recentlyViewedRepository.InsertAsync(
                        new RecentlyViewedProduct(
                            GuidGenerator.Create(),
                            userId,
                            productId,
                            DateTime.UtcNow));

                    // Clean up old items
                    await _recentlyViewedRepository.DeleteOldestForUserAsync(
                        userId,
                        RecentlyViewedConsts.MaxItemsPerUser);
                }
            }
            catch (Exception ex)
            {
                // OPTIMIZATION 12: Don't throw on tracking errors (non-critical feature)
                Logger.LogWarning(ex, "Failed to track product view for product {ProductId}", productId);
            }
        }

        [Authorize]
        public async Task ClearAsync()
        {
            var userId = CurrentUser.GetId();
            await _recentlyViewedRepository.DeleteAsync(x => x.UserId == userId);
        }
    }
}