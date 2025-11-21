
// ===== OrderNotificationService.cs =====
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.Hubs;
using Acme.ProductSelling.Orders.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Orders
{
    public class OrderNotificationService : IOrderNotificationService, ITransientDependency
    {
        private readonly IHubContext<OrderHub, IOrderClient> _orderHubContext;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly ILogger<OrderNotificationService> _logger;

        public OrderNotificationService(
         IHubContext<OrderHub, IOrderClient> orderHubContext,
         IStringLocalizer<ProductSellingResource> localizer,
         ILogger<OrderNotificationService> logger)
        {
            _orderHubContext = orderHubContext;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task NotifyOrderStatusChangeAsync(Order order)
        {
            _logger.LogInformation("[NotifyOrderStatus] START - OrderId: {OrderId}, OrderNumber: {OrderNumber}, OrderStatus: {OrderStatus}, PaymentStatus: {PaymentStatus}, CustomerId: {CustomerId}",
                order.Id, order.OrderNumber, order.OrderStatus, order.PaymentStatus, order.CustomerId);

            try
            {
                var newOrderStatusString = order.OrderStatus.ToString();
                var ordersStatusTextLocalized = _localizer[$"Enum:OrderStatus.{newOrderStatusString}"];
                var paymentStatusString = order.PaymentStatus.ToString();
                var paymentStatusText = _localizer[$"Enum:PaymentStatus.{paymentStatusString}"];

                _logger.LogDebug("[NotifyOrderStatus] Localized texts - OrderStatusText: {OrderStatusText}, PaymentStatusText: {PaymentStatusText}",
                    ordersStatusTextLocalized, paymentStatusText);

                // Notify admins
                await _orderHubContext.Clients.Group("Admins")
                    .ReceiveOrderStatusUpdate(order.Id, newOrderStatusString, ordersStatusTextLocalized,
                                             paymentStatusString, paymentStatusText);

                _logger.LogDebug("[NotifyOrderStatus] Notification sent to Admins group");

                // Notify managers
                await _orderHubContext.Clients.Group("Managers")
                    .ReceiveOrderStatusUpdate(order.Id, newOrderStatusString, ordersStatusTextLocalized,
                                             paymentStatusString, paymentStatusText);

                _logger.LogDebug("[NotifyOrderStatus] Notification sent to Managers group");

                // Notify sellers (for in-store orders)
                if (order.OrderType == OrderType.InStore)
                {
                    await _orderHubContext.Clients.Group("Sellers")
                        .ReceiveOrderStatusUpdate(order.Id, newOrderStatusString, ordersStatusTextLocalized,
                                                 paymentStatusString, paymentStatusText);

                    _logger.LogDebug("[NotifyOrderStatus] Notification sent to Sellers group (in-store order)");

                    // Also notify warehouse staff for fulfillment
                    await _orderHubContext.Clients.Group("WarehouseStaffs")
                        .ReceiveOrderStatusUpdate(order.Id, newOrderStatusString, ordersStatusTextLocalized,
                                                 paymentStatusString, paymentStatusText);

                    _logger.LogDebug("[NotifyOrderStatus] Notification sent to WarehouseStaffs group (in-store order)");

                    // Notify cashiers for payment processing
                    await _orderHubContext.Clients.Group("Cashiers")
                        .ReceiveOrderStatusUpdate(order.Id, newOrderStatusString, ordersStatusTextLocalized,
                                                 paymentStatusString, paymentStatusText);

                    _logger.LogDebug("[NotifyOrderStatus] Notification sent to Cashiers group (in-store order)");
                }

                // Notify the customer if they exist
                if (order.CustomerId.HasValue)
                {
                    var userGroupName = $"User_{order.CustomerId.Value}";
                    await _orderHubContext.Clients.Group(userGroupName)
                     .ReceiveOrderStatusUpdate(order.Id, newOrderStatusString, ordersStatusTextLocalized,
                                                        paymentStatusString, paymentStatusText);

                    _logger.LogDebug("[NotifyOrderStatus] Notification sent to customer - CustomerId: {CustomerId}, Group: {GroupName}",
                        order.CustomerId.Value, userGroupName);
                }
                else
                {
                    _logger.LogDebug("[NotifyOrderStatus] No CustomerId - skipping customer notification");
                }

                _logger.LogInformation("[NotifyOrderStatus] COMPLETED - OrderId: {OrderId}, Notifications sent successfully",
                    order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[NotifyOrderStatus] FAILED - OrderId: {OrderId}, Error: {ErrorMessage}",
                    order.Id, ex.Message);
                // Don't throw - notification failure shouldn't break the order flow
            }
        }
    }
}