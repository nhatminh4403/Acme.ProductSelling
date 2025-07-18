using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Orders.BackgroundJobs;
using Acme.ProductSelling.VNPay.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;

namespace Acme.ProductSelling.Payments
{
    public class PaymentCallbackAppService : IPaymentCallbackAppService
    {
        private readonly IVnPayService _vnPayService;
        private readonly IRepository<Order, Guid> _orderRepository;
        private readonly IOrderNotificationService _orderNotificationService;
        private readonly IDistributedEventBus _distributedEventBus;
        private readonly ILogger<PaymentCallbackAppService> _logger;
        public PaymentCallbackAppService(
            IVnPayService vnPayService,
            IRepository<Order, Guid> orderRepository,
            IOrderNotificationService orderNotificationService,
            ILogger<PaymentCallbackAppService> logger,
            IDistributedEventBus distributedEventBus)
        {
            _vnPayService = vnPayService;
            _orderRepository = orderRepository;
            _orderNotificationService = orderNotificationService;
            _distributedEventBus = distributedEventBus;
            _logger = logger;
        }

        public async Task<VnPayIpnProcessingResult> ProcessVnPayIpnAsync(IQueryCollection collections)
        {
            _logger.LogInformation("Bắt đầu xử lý IPN từ VNPay...");

            // Bước 1: Gọi service từ module VNPay để xác thực và trích xuất dữ liệu.
            var response = _vnPayService.PaymentExecute(collections);

            // Bước 2: Kiểm tra chữ ký (signature). Đây là bước bảo mật quan trọng nhất.
            if (!response.Success)
            {
                _logger.LogWarning("Xác thực chữ ký VNPay IPN thất bại!");
                // Phản hồi cho VNPay rằng chữ ký không hợp lệ.
                return new VnPayIpnProcessingResult { RspCode = "97", Message = "Invalid Signature" };
            }

            _logger.LogInformation("Xác thực chữ ký VNPay IPN thành công cho OrderReferenceId: {OrderRef}", response.OrderId);

            // Bước 3: Tìm đơn hàng trong hệ thống dựa vào mã tham chiếu (vnp_TxnRef).
            if (!Guid.TryParse(response.OrderId, out var orderId))
            {
                _logger.LogError("Mã tham chiếu (vnp_TxnRef) không phải là một Guid hợp lệ: {OrderRef}", response.OrderId);
                return new VnPayIpnProcessingResult { RspCode = "01", Message = "Order not found" };
            }

            var order = await _orderRepository.FindAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Không tìm thấy đơn hàng với Id: {OrderId}", orderId);
                return new VnPayIpnProcessingResult { RspCode = "01", Message = "Order not found" };
            }

            // Bước 4: Kiểm tra trạng thái giao dịch và trạng thái đơn hàng để tránh xử lý lặp lại.
            // Nếu thanh toán thành công và đơn hàng đang chờ thanh toán.
            if (response.VnPayResponseCode == "00" && order.Status == OrderStatus.PendingPayment)
            {
                _logger.LogInformation("Giao dịch thành công. Cập nhật trạng thái cho OrderId: {OrderId}", order.Id);

                order.MarkAsPaid(); // Dùng phương thức trong Entity để thay đổi trạng thái
                await _orderRepository.UpdateAsync(order, autoSave: true);
                // Gửi thông báo real-time
                await _orderNotificationService.NotifyOrderStatusChangeAsync(order);

                return new VnPayIpnProcessingResult { RspCode = "00", Message = "Confirm Success" };
            }

            // Các trường hợp khác: Đơn hàng đã được xử lý trước đó hoặc giao dịch thất bại.
            _logger.LogWarning("Bỏ qua xử lý IPN cho OrderId: {OrderId}. Lý do: Giao dịch không thành công (Code: {VnpCode}) hoặc đơn hàng không ở trạng thái PendingPayment (Trạng thái thực tế: {Status}).",
                order.Id, response.VnPayResponseCode, order.Status);

            return new VnPayIpnProcessingResult { RspCode = "02", Message = "Order already confirmed" };
        }
    }
}

