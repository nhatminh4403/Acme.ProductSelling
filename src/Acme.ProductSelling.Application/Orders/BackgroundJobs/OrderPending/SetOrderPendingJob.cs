using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.Hubs;
using Acme.ProductSelling.Orders.Services;
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

        public SetOrderPendingJob(
                  IRepository<Order, Guid> orderRepository,
                  IHubContext<OrderHub, IOrderClient> orderHubContext,
                   ILogger<SetOrderPendingJob> logger,
                  IStringLocalizer<ProductSellingResource> localizer,
                  IDistributedEventBus distributedEventBus,
                  IOrderNotificationService orderNotificationService)
        {
            _orderRepository = orderRepository;
            _orderHubContext = orderHubContext;
            _localizer = localizer;
            _logger = logger;
            _distributedEventBus = distributedEventBus;
            _orderNotificationService = orderNotificationService;
        }

        [UnitOfWork]
        public async Task ExecuteAsync(SetOrderBackgroundJobArgs args)
        {
            _logger.LogInformation("[SetOrderPendingJob] START - OrderId: {OrderId}, Execution started", args.OrderId);

            try
            {
                var order = await _orderRepository.GetAsync(args.OrderId);

                _logger.LogDebug("[SetOrderPendingJob] Order retrieved - OrderId: {OrderId}, OrderNumber: {OrderNumber}, CurrentStatus: {CurrentStatus}, PaymentStatus: {PaymentStatus}, PaymentMethod: {PaymentMethod}",
                    order.Id, order.OrderNumber, order.OrderStatus, order.PaymentStatus, order.PaymentMethod);

                if (order == null)
                {
                    _logger.LogWarning("[SetOrderPendingJob] SKIP - Order not found. OrderId: {OrderId}", args.OrderId);
                    return;
                }

                if (order.OrderStatus == OrderStatus.Placed)
                {
                    var oldStatus = order.OrderStatus;
                    order.SetStatus(OrderStatus.Pending);

                    _logger.LogInformation("[SetOrderPendingJob] Status updated - OrderId: {OrderId}, OldStatus: {OldStatus}, NewStatus: {NewStatus}",
                        order.Id, oldStatus, order.OrderStatus);

                    await _orderRepository.UpdateAsync(order, autoSave: true);
                    _logger.LogDebug("[SetOrderPendingJob] Order saved to database - OrderId: {OrderId}", order.Id);

                    await _distributedEventBus.PublishAsync(new OrderStatusChangedEto
                    {
                        OrderId = order.Id,
                        CustomerId = order.CustomerId
                    });
                    _logger.LogDebug("[SetOrderPendingJob] Distributed event published - OrderId: {OrderId}", order.Id);

                    await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
                    _logger.LogDebug("[SetOrderPendingJob] Notification sent - OrderId: {OrderId}", order.Id);

                    _logger.LogInformation("[SetOrderPendingJob] COMPLETED successfully - OrderId: {OrderId}, OrderNumber: {OrderNumber}, NewStatus: {NewStatus}",
                        order.Id, order.OrderNumber, order.OrderStatus);
                }
                else
                {
                    _logger.LogWarning("[SetOrderPendingJob] SKIP - Order not in 'Placed' status. OrderId: {OrderId}, CurrentStatus: {CurrentStatus}, Expected: Placed",
                        args.OrderId, order.OrderStatus);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SetOrderPendingJob] FAILED - OrderId: {OrderId}, Error: {ErrorMessage}, StackTrace: {StackTrace}",
                    args.OrderId, ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}
