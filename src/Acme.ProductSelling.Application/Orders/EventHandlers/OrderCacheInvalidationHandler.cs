using Acme.ProductSelling.Orders.Dtos;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Acme.ProductSelling.Orders
{

    public class OrderCacheInvalidationHandler : IDistributedEventHandler<OrderStatusChangedEto>, ITransientDependency
    {
        private readonly IDistributedCache<OrderDto, Guid> _cache;
        private readonly ILogger<OrderCacheInvalidationHandler> _logger;

        public OrderCacheInvalidationHandler(
            IDistributedCache<OrderDto, Guid> cache,
            ILogger<OrderCacheInvalidationHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task HandleEventAsync(OrderStatusChangedEto eventData)
        {
            _logger.LogInformation("[CacheInvalidation] START - OrderId: {OrderId}, CustomerId: {CustomerId}",
                eventData?.OrderId, eventData?.CustomerId);

            try
            {
                if (eventData == null || eventData.OrderId == Guid.Empty)
                {
                    _logger.LogWarning("[CacheInvalidation] SKIP - Invalid event data. OrderId: {OrderId}",
                        eventData?.OrderId ?? Guid.Empty);
                    return;
                }

                _logger.LogDebug("[CacheInvalidation] Removing cache entry - OrderId: {OrderId}", eventData.OrderId);

                // Invalidate the cache for the specific order
                await _cache.RemoveAsync(eventData.OrderId);

                _logger.LogInformation("[CacheInvalidation] COMPLETED - Cache invalidated for OrderId: {OrderId}",
                    eventData.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CacheInvalidation] FAILED - OrderId: {OrderId}, Error: {ErrorMessage}",
                    eventData?.OrderId, ex.Message);
                // Don't throw - cache invalidation failure shouldn't break the event flow
            }
        }
    }
}
