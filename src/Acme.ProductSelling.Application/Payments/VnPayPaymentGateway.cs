using Acme.ProductSelling.Orders;
using Acme.ProductSelling.PaymentGateway.VnPay.Dtos;
using Acme.ProductSelling.PaymentGateway.VnPay.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Acme.ProductSelling.Payments
{
    public class VnPayPaymentGateway : IPaymentGateway
    {
        public string Name => PaymentMethods.VnPay;
        private readonly IVnPayService _vnPayService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<VnPayPaymentGateway> _logger;

        public VnPayPaymentGateway(IVnPayService vnPayService, IHttpContextAccessor httpContextAccessor, ILogger<VnPayPaymentGateway> logger)
        {
            _vnPayService = vnPayService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public Task<PaymentGatewayResult> ProcessAsync(Order order)
        {
            try
            {
                ValidateOrder(order);

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogError("HttpContext is null when processing VNPay payment for order {OrderId}", order.Id);
                    throw new UserFriendlyException("Không thể khởi tạo thanh toán. Vui lòng thử lại.");
                }

                var model = new VnPaymentRequestModel
                {
                    OrderId = order.Id.ToString(),
                    Price = (double)order.TotalAmount,
                    Description = $"Thanh toan don hang {order.OrderNumber} - {order.CustomerName}",
                    CreatedDate = DateTime.Now,
                    ExpireDate = DateTime.Now.AddHours(1)
                };

                _logger.LogInformation(
                    "Creating VNPay payment URL for Order {OrderNumber} (ID: {OrderId}), Amount: {Amount}",
                    order.OrderNumber, order.Id, order.TotalAmount
                );

                var paymentUrl = _vnPayService.CreatePaymentUrl(httpContext, model);

                // IMPROVEMENT 3: Validate payment URL
                if (string.IsNullOrWhiteSpace(paymentUrl))
                {
                    _logger.LogError("VNPay service returned empty payment URL for order {OrderId}", order.Id);
                    throw new UserFriendlyException("Không thể tạo liên kết thanh toán VNPay. Vui lòng thử lại.");
                }

                _logger.LogInformation("VNPay payment URL created successfully for order {OrderId}", order.Id);

                return Task.FromResult(new PaymentGatewayResult
                {
                    RedirectUrl = paymentUrl,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý thanh toán VnPay cho đơn hàng {OrderId}", order.Id);
                throw;
            }
        }
        private void ValidateOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (order.TotalAmount <= 0)
            {
                _logger.LogWarning("Invalid order amount {Amount} for order {OrderId}", order.TotalAmount, order.Id);
                throw new UserFriendlyException("Số tiền đơn hàng không hợp lệ.");
            }

            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                _logger.LogWarning("Attempting to pay for already paid order {OrderId}", order.Id);
                throw new UserFriendlyException("Đơn hàng này đã được thanh toán.");
            }

            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                _logger.LogWarning("Attempting to pay for cancelled order {OrderId}", order.Id);
                throw new UserFriendlyException("Không thể thanh toán cho đơn hàng đã hủy.");
            }
        }
    }
}
