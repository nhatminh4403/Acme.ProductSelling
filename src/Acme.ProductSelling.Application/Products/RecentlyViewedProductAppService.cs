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

            var products = await _productRepository.GetListAsync(
                p => targetIds.Contains(p.Id) && p.IsActive);

            return targetIds
                .Select(id => products.FirstOrDefault(p => p.Id == id))
                .Where(p => p != null)
                .Select(p => _productMapper.Map(p)) // Use Mapperly
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
            var validProductIds = await _productRepository
                .GetListAsync(p => input.ProductIds.Contains(p.Id));
            var validIds = validProductIds.Select(p => p.Id).ToHashSet();

            var existingViews = await _recentlyViewedRepository
                .GetByUserIdAsync(userId, RecentlyViewedConsts.MaxItemsPerUser);
            var existingProductIds = existingViews
                .Select(x => x.ProductId)
                .ToHashSet();

            var toProcess = input.ProductIds
                .Where(id => validIds.Contains(id))
                .Reverse()
                .ToList();

            foreach (var productId in toProcess)
            {
                if (existingProductIds.Contains(productId))
                {
                    var existing = existingViews.First(x => x.ProductId == productId);
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

                    existingProductIds.Add(productId);
                }
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

            var existing = await _recentlyViewedRepository.FindByUserAndProductAsync(
                userId, productId);

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
    }
}