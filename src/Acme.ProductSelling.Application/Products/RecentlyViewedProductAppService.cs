using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Authorization;
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

        public RecentlyViewedProductAppService(IRecentlyViewedProductRepository recentlyViewedRepository,
                                               IProductRepository productRepository)
        {
            _recentlyViewedRepository = recentlyViewedRepository;
            _productRepository = productRepository;
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
            if (input.GuestProductIds?.Any() == true)
            {
                foreach (var guestId in input.GuestProductIds)
                {
                    if (!orderedProductIds.Contains(guestId))
                    {
                        orderedProductIds.Add(guestId);
                    }
                }
            }
            var targetIds = orderedProductIds
           .Take(input.MaxCount)
           .ToList();

            if (!targetIds.Any())
            {
                return new List<RecentlyViewedProductDto>();
            }

            // 4. Fetch products
            var products = await _productRepository.GetListAsync(
                p => targetIds.Contains(p.Id) && p.IsActive);

            // 5. Map in order of targetIds to preserve recency
            return targetIds
                .Select(id => products.FirstOrDefault(p => p.Id == id))
                .Where(p => p != null)
                .Select(MapToDto)
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

            // Validate which product IDs actually exist
            var validProductIds = await _productRepository
                .GetListAsync(p => input.ProductIds.Contains(p.Id));
            var validIds = validProductIds.Select(p => p.Id).ToHashSet();

            // Get user's existing history
            var existingViews = await _recentlyViewedRepository
                .GetByUserIdAsync(userId, RecentlyViewedConsts.MaxItemsPerUser);
            var existingProductIds = existingViews
                .Select(x => x.ProductId)
                .ToHashSet();

            // Process in reverse to maintain correct order (oldest first, newest last)
            var toProcess = input.ProductIds
                .Where(id => validIds.Contains(id))
                .Reverse()
                .ToList();

            foreach (var productId in toProcess)
            {
                if (existingProductIds.Contains(productId))
                {
                    // Update existing
                    var existing = existingViews.First(x => x.ProductId == productId);
                    existing.UpdateViewedTime();
                    await _recentlyViewedRepository.UpdateAsync(existing);
                }
                else
                {
                    // Insert new
                    await _recentlyViewedRepository.InsertAsync(
                        new RecentlyViewedProduct(
                            GuidGenerator.Create(),
                            userId,
                            productId,
                            DateTime.UtcNow));

                    existingProductIds.Add(productId);
                }
            }

            // Cleanup excess entries
            await _recentlyViewedRepository.DeleteOldestForUserAsync(
                userId,
                RecentlyViewedConsts.MaxItemsPerUser);
        }
        [Authorize]
        public async Task TrackProductViewAsync(Guid productId)
        {
            var userId = CurrentUser.GetId();

            // Validate product exists
            if (!await _productRepository.AnyAsync(p => p.Id == productId))
            {
                return; // Silently ignore invalid products
            }

            var existing = await _recentlyViewedRepository.FindByUserAndProductAsync(
                userId, productId);

            if (existing != null)
            {
                // Update timestamp
                existing.UpdateViewedTime();
                await _recentlyViewedRepository.UpdateAsync(existing);
            }
            else
            {
                // Insert new
                await _recentlyViewedRepository.InsertAsync(
                    new RecentlyViewedProduct(
                        GuidGenerator.Create(),
                        userId,
                        productId,
                        DateTime.UtcNow));

                // Cleanup old entries
                await _recentlyViewedRepository.DeleteOldestForUserAsync(
                    userId,
                    RecentlyViewedConsts.MaxItemsPerUser);
            }
        }
        [Authorize]
        public async Task ClearAsync()
        {
            var userId = CurrentUser.GetId();
            await _recentlyViewedRepository.DeleteAsync(x => x.UserId == userId);
        }
        private RecentlyViewedProductDto MapToDto(Product product)
        {
            return new RecentlyViewedProductDto
            {
                ProductId = product.Id,
                ProductName = product.ProductName,
                ImageUrl = product.ImageUrl,
                OriginalPrice = product.OriginalPrice,
                DiscountedPrice = product.DiscountedPrice,
                UrlSlug = product.UrlSlug,
                IsAvailableForPurchase = product.IsAvailableForPurchase(),
                DiscountPercent = product.DiscountPercent,
                TotalStockAcrossAllStores = product.StoreInventories?.Sum(x => x.Quantity) ?? 0
            };
        }
    }
}