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

        public OrderCacheInvalidationHandler(IDistributedCache<OrderDto, Guid> cache)
        {
            _cache = cache;
        }

        public async Task HandleEventAsync(OrderStatusChangedEto eventData)
        {
            if (eventData == null || eventData.OrderId == Guid.Empty)
            {
                return;
            }
            // Invalidate the cache for the specific order
            await _cache.RemoveAsync(eventData.OrderId);
        }
    }
}
