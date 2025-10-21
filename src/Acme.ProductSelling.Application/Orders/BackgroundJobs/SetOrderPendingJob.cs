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

namespace Acme.ProductSelling.Orders.BackgroundJobs
{
    public class SetOrderPendingJob : IAsyncBackgroundJob<SetOrderPendingJobArgs>, ITransientDependency
    {
        private readonly IRepository<Order, Guid> _orderRepository;
        private readonly IHubContext<OrderHub, IOrderClient> _orderHubContext;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly IOrderNotificationService _orderNotificationService; // <-- THAY ĐỔI
        private readonly ILogger<SetOrderPendingJob> _logger; // Thêm logger
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
        public async Task ExecuteAsync(SetOrderPendingJobArgs args)
        {
            try
            {
                var order = await _orderRepository.GetAsync(args.OrderId);
                if (order == null)
                {
                    return;
                }

                if (order.OrderStatus == OrderStatus.Placed)
                {

                    order.SetStatus(OrderStatus.Pending);

                    await _orderRepository.UpdateAsync(order, autoSave: true);

                    await _distributedEventBus.PublishAsync(new OrderStatusChangedEto
                    {
                        OrderId = order.Id,
                        CustomerId = order.CustomerId
                    });

                    await _orderNotificationService.NotifyOrderStatusChangeAsync(order);

                }
                else
                {

                    _logger.LogWarning("[JOB-SKIP] Bỏ qua cập nhật." +
                        " Order {OrderId} không ở trạng thái 'Placed'." +
                        " Trạng thái thực tế là {Status}.", args.OrderId, order.OrderStatus);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[JOB-ERROR] Lỗi khi chạy SetOrderPendingJob cho OrderId: {OrderId}",
                    args.OrderId);
                throw;
            }

        }
    }

}
