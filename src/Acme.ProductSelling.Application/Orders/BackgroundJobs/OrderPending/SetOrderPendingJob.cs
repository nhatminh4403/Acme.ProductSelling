using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.Hubs;
using Acme.ProductSelling.Orders.Services;
using Google.Apis.Util;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace Acme.ProductSelling.Orders.BackgroundJobs.OrderPending
{
    public class SetOrderPendingJob : IAsyncBackgroundJob<SetOrderBackgroundJobArgs>, ITransientDependency
    {
        private readonly IRepository<Order, Guid> _orderRepository;
        private readonly IHubContext<OrderHub, IOrderClient> _orderHubContext;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly IOrderNotificationService _orderNotificationService;
        private readonly ILogger<SetOrderPendingJob> _logger;
        private readonly IDistributedEventBus _distributedEventBus;
        private readonly IClock _clock;
        public SetOrderPendingJob(
                  IRepository<Order, Guid> orderRepository,
                  IHubContext<OrderHub, IOrderClient> orderHubContext,
                   ILogger<SetOrderPendingJob> logger,
                  IStringLocalizer<ProductSellingResource> localizer,
                  IDistributedEventBus distributedEventBus,
                  IOrderNotificationService orderNotificationService,
                  IClock clock)
        {
            _orderRepository = orderRepository;
            _orderHubContext = orderHubContext;
            _localizer = localizer;
            _logger = logger;
            _distributedEventBus = distributedEventBus;
            _orderNotificationService = orderNotificationService;
            _clock = clock;
        }

        [UnitOfWork]
        public async Task ExecuteAsync(SetOrderBackgroundJobArgs args)
        {

            try
            {
                var order = await _orderRepository.GetAsync(args.OrderId);

                if (order == null)
                {
                    throw new EntryPointNotFoundException($"Order {order.Id} not found");
                }

                if (order.OrderStatus == OrderStatus.Placed)
                {
                    var oldStatus = order.OrderStatus;
                    order.SetStatus(OrderStatus.Pending);

                    await _orderRepository.UpdateAsync(order, autoSave: true);
                    await _distributedEventBus.PublishAsync(new OrderStatusChangedEto
                    {
                        OrderId = order.Id,
                        CustomerId = order.CustomerId
                    });
                }
                else
                {
                    throw new Exception("Order status is not in a valid state to be set to pending. Current status: " + order.OrderStatus);
                }
            }
            catch (Exception ex)
            {
                
                throw new Exception("Failed to set order as pending.", ex);
            }
        }
    }
}
