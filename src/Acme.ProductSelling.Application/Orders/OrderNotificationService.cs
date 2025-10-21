using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.Hubs;
using Acme.ProductSelling.Orders.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Orders
{
    public class OrderNotificationService : IOrderNotificationService, ITransientDependency
    {
        private readonly IHubContext<OrderHub, IOrderClient> _orderHubContext;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;

        public OrderNotificationService(
         IHubContext<OrderHub, IOrderClient> orderHubContext,
         IStringLocalizer<ProductSellingResource> localizer)
        {
            _orderHubContext = orderHubContext;
            _localizer = localizer;
        }


        public async Task NotifyOrderStatusChangeAsync(Order order)
        {
            var newOrderStatusString = order.OrderStatus.ToString();
            var ordersStatusTextLocalized = _localizer[$"Enum:OrderStatus.{newOrderStatusString}"];

            var paymentStatusString = order.PaymentStatus.ToString();
            var paymentStatusText = _localizer[$"Enum:PaymentStatus.{paymentStatusString}"];


            await _orderHubContext.Clients.Group("Admins")
                .ReceiveOrderStatusUpdate(order.Id, newOrderStatusString, ordersStatusTextLocalized, paymentStatusString, paymentStatusText);

            if (order.CustomerId.HasValue)
            {
                var userGroupName = $"User_{order.CustomerId.Value}";
                await _orderHubContext.Clients.Group(userGroupName)
                 .ReceiveOrderStatusUpdate(order.Id, newOrderStatusString, ordersStatusTextLocalized,
                                                    paymentStatusString, paymentStatusText);
            }
        }

    }
}
