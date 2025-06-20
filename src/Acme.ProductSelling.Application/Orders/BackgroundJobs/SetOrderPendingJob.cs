using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.Hubs;
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
        private readonly ILogger<SetOrderPendingJob> _logger; // Thêm logger
        private readonly IDistributedEventBus _distributedEventBus;
        public SetOrderPendingJob(
                  IRepository<Order, Guid> orderRepository,
                  IHubContext<OrderHub, IOrderClient> orderHubContext,
                   ILogger<SetOrderPendingJob> logger,
                  IStringLocalizer<ProductSellingResource> localizer,
                  IDistributedEventBus distributedEventBus)
        {
            _orderRepository = orderRepository;
            _orderHubContext = orderHubContext;
            _localizer = localizer;
            _logger = logger; 
            _distributedEventBus = distributedEventBus;
        }

        [UnitOfWork]
        public async Task ExecuteAsync(SetOrderPendingJobArgs args)
        {
            _logger.LogInformation("[JOB-START] Bắt đầu chạy SetOrderPendingJob cho OrderId: {OrderId}", args.OrderId);
            try
            {
                var order = await _orderRepository.GetAsync(args.OrderId);
                if (order == null)
                {
                    _logger.LogError("[JOB-FAIL] Không tìm thấy Order với ID {OrderId}.", args.OrderId);
                    return;
                }
                _logger.LogInformation("[JOB-INFO] Đã tìm thấy Order {OrderId}. Trạng thái hiện tại là: {Status}", args.OrderId, order.Status);

                if (order.Status == OrderStatus.Placed)
                {
                    _logger.LogInformation("[JOB-LOGIC] Trạng thái của Order là 'Placed'. Tiến hành cập nhật sang 'Pending'.");

                    order.SetPendingStatus();

                    _logger.LogInformation("[JOB-UPDATE] Chuẩn bị gọi UpdateAsync và lưu vào DB cho Order {OrderId}...", args.OrderId);

                    await _orderRepository.UpdateAsync(order, autoSave: true);

                    _logger.LogInformation("[JOB-SUCCESS] Đã cập nhật thành công và lưu vào DB. Trạng thái mới: {Status}. Bắt đầu gửi SignalR.", order.Status);
                    await _distributedEventBus.PublishAsync(new OrderStatusChangedEto
                    {
                        OrderId = order.Id,
                        CustomerId = order.CustomerId
                    });

                    await _orderHubContext.Clients.All.ReceiveOrderStatusUpdate(
                        order.Id,
                        order.Status.ToString(),
                        _localizer[order.Status.ToString()]
                    );

                    _logger.LogInformation("[JOB-COMPLETE] Đã gửi thông báo SignalR và hoàn thành job.");
                }
                else
                {
                    // Đây là một kịch bản rất có thể xảy ra
                    _logger.LogWarning("[JOB-SKIP] Bỏ qua cập nhật. Order {OrderId} không ở trạng thái 'Placed'. Trạng thái thực tế là {Status}.", args.OrderId, order.Status);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[JOB-ERROR] Lỗi khi chạy SetOrderPendingJob cho OrderId: {OrderId}", args.OrderId);
                throw; // Ném lại ngoại lệ để hệ thống có thể xử lý
            }

        }
    }

}
