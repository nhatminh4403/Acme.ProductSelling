using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Acme.ProductSelling.Products
{
    public class RecentlyViewedProductAppService : ProductSellingAppService, IRecentlyViewedProductAppService
    {
        private readonly IRecentlyViewedProductRepository _recentlyViewedRepository;
        private readonly IProductRepository _productRepository;
        // Inject the specific mapper
        private readonly ProductToRecentlyViewedProductDtoMapper _productMapper;

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
            var orderedProductIds = new List<Guid>();

            if (CurrentUser.IsAuthenticated)
            {
                var dbHistory = await _recentlyViewedRepository.GetByUserIdAsync(
                    CurrentUser.GetId(),
                    input.MaxCount);

                orderedProductIds.AddRange(dbHistory.Select(x => x.ProductId));
            }
            // Add guest IDs avoiding duplicates
            if (input.GuestProductIds?.Any() == true)
            {
                // Create HashSet for O(1) lookups to avoid n^2 complexity if lists are large
                var existingIds = orderedProductIds.ToHashSet();
                foreach (var guestId in input.GuestProductIds)
                {
                    if (existingIds.Add(guestId))
                    {
                        orderedProductIds.Add(guestId);
                    }
                }
            }

            var targetIds = orderedProductIds.Take(input.MaxCount).ToList();


            if (!targetIds.Any())
            {
                return new List<RecentlyViewedProductDto>();
            }
            var query = await _productRepository.GetQueryableAsync();

            var products = await AsyncExecuter.ToListAsync(
               query
               .AsNoTracking()
               .Include(p => p.StoreInventories) // Crucial Fix
               .Where(p => targetIds.Contains(p.Id) && p.IsActive)
           );

            return targetIds
              .Select(id => products.FirstOrDefault(p => p.Id == id))
              .Where(p => p != null)
              .Select(p => _productMapper.Map(p))
              .ToList();
        }

        [Authorize]
        public async Task SyncGuestHistoryAsync(SyncGuestHistoryInput input)
        {
            if (input.ProductIds == null || !input.ProductIds.Any())
            {
                return;
            }

            var userId = CurrentUser.GetId();
            var validProductIds = await _productRepository.GetListAsync(p => input.ProductIds.Contains(p.Id));
            var validIdsSet = validProductIds.Select(p => p.Id).ToHashSet();

            var existingViews = await _recentlyViewedRepository.GetByUserIdAsync(userId,
                                                                                RecentlyViewedConsts.MaxItemsPerUser * 2);
            var existingProductMap = existingViews.ToDictionary(x => x.ProductId);

            // Prepare list for processing
            var toProcess = input.ProductIds
                .Where(id => validIdsSet.Contains(id))
                .Distinct() // Prevent processing duplicates in same batch
                .Reverse() // Oldest first effectively pushes newest to top later
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
                    newItems.Add(new RecentlyViewedProduct(GuidGenerator.Create(),
                                                           userId,
                                                           productId,
                                                           DateTime.UtcNow));

                    // Mark as existing so we don't add twice in logic loop
                    existingProductMap[productId] = newItems.Last();
                }
            }

            // Batch Database Operations to reduce traffic
            if (itemsToUpdate.Any())
            {
                await _recentlyViewedRepository.UpdateManyAsync(itemsToUpdate);
            }
            if (newItems.Any())
            {
                await _recentlyViewedRepository.InsertManyAsync(newItems);
            }

            await _recentlyViewedRepository.DeleteOldestForUserAsync(
                userId,
                RecentlyViewedConsts.MaxItemsPerUser);
        }

        [Authorize]
        public async Task TrackProductViewAsync(Guid productId)
        {
            var userId = CurrentUser.GetId();

            if (!await _productRepository.AnyAsync(p => p.Id == productId))
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
                await _recentlyViewedRepository.InsertAsync(new RecentlyViewedProduct(GuidGenerator.Create(), userId, productId, DateTime.UtcNow));

                await _recentlyViewedRepository.DeleteOldestForUserAsync(userId,
                                                                         RecentlyViewedConsts.MaxItemsPerUser);
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