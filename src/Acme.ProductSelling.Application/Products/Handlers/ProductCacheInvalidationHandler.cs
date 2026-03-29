using Acme.ProductSelling.Products.Caching;
using Acme.ProductSelling.Products.Dtos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Acme.ProductSelling.Products.Handlers
{
    public class ProductCacheInvalidationHandler :
           ILocalEventHandler<ProductChangedEto>,
           ITransientDependency
    {
        private readonly IDistributedCache<ProductDto, Guid> _detailCache;
        private readonly IDistributedCache<ProductDto, string> _slugCache;
        private readonly IDistributedCache<List<FeaturedCategoryProductsDto>, string> _featuredCache;
        private readonly ILogger<ProductCacheInvalidationHandler> _logger;

        public ProductCacheInvalidationHandler(
            IDistributedCache<ProductDto, Guid> detailCache,
            IDistributedCache<ProductDto, string> slugCache,
            IDistributedCache<List<FeaturedCategoryProductsDto>, string> featuredCache,
            ILogger<ProductCacheInvalidationHandler> logger)
        {
            _detailCache = detailCache;
            _slugCache = slugCache;
            _featuredCache = featuredCache;
            _logger = logger;
        }

        public async Task HandleEventAsync(ProductChangedEto eventData)
        {
            _logger.LogInformation(
                "[ProductCacheInvalidation] Invalidating cache for ProductId: {ProductId}, Slug: {Slug}",
                eventData.ProductId, eventData.UrlSlug);

            try
            {
                await _detailCache.RemoveAsync(eventData.ProductId);

                if (!string.IsNullOrWhiteSpace(eventData.UrlSlug))
                    await _slugCache.RemoveAsync(ProductCacheKeys.DetailBySlug + eventData.UrlSlug.ToLower());

                // Remove featured carousels — product availability may have changed
                await _featuredCache.RemoveAsync(ProductCacheKeys.FeaturedCarousels);
            }
            catch (Exception ex)
            {
                // Cache invalidation failure must never crash the main flow
                _logger.LogError(ex,
                    "[ProductCacheInvalidation] Failed to invalidate cache for ProductId: {ProductId}",
                    eventData.ProductId);
            }
        }
    }

    /// <summary>
    /// Local ETO published whenever a product is created, updated, deleted,
    /// or its availability changes (e.g. the ProductRelease background job).
    /// </summary>
    public class ProductChangedEto
    {
        public Guid ProductId { get; set; }
        public string UrlSlug { get; set; }
    }
}
