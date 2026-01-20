using Acme.ProductSelling.Orders.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace Acme.ProductSelling.Orders.EventHandlers
{
    public class OrderStatusNotificationHandler : IDistributedEventHandler<OrderStatusChangedEto>, ITransientDependency
    {
        private readonly IOrderNotificationService _orderNotificationService;
        private readonly IRepository<Order, Guid> _orderRepository;
        private readonly ILogger<OrderStatusNotificationHandler> _logger;

        public OrderStatusNotificationHandler(
            IOrderNotificationService orderNotificationService,
            IRepository<Order, Guid> orderRepository,
            ILogger<OrderStatusNotificationHandler> logger)
        {
            _orderNotificationService = orderNotificationService;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        // Run this in a new Unit of Work so it doesn't block the HTTP request transaction
        [UnitOfWork]
        public async Task HandleEventAsync(OrderStatusChangedEto eventData)
        {
            _logger.LogDebug("[EventHandle] Handling OrderStatusChanged for OrderId: {OrderId}", eventData.OrderId);

            try
            {
                // We reload the order to get the full rich entity data (Customer Name, etc) 
                // required by OrderNotificationService logic
                var order = await _orderRepository.FindAsync(eventData.OrderId);

                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found during notification event.", eventData.OrderId);
                    return;
                }

                // Call the existing service that handles logic for Admins/Sellers/User groups
                await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            }
            catch (Exception ex)
            {
                // If SignalR fails, we just log it. The Order data remains safe.
                _logger.LogError(ex, "Failed to send SignalR notification for Order {OrderId}", eventData.OrderId);
            }
        }
    }
}
