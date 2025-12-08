using Acme.ProductSelling.Orders;
using Acme.ProductSelling.PaymentGateway.MoMo.Models;
using Acme.ProductSelling.PaymentGateway.MoMo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Acme.ProductSelling.Payments
{
    public class MoMoPaymentGateway : IPaymentGateway
    {
        public string Name => PaymentMethods.MoMo;

        private readonly IMoMoService _moMoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MoMoPaymentGateway> _logger;

        public MoMoPaymentGateway(
            IMoMoService moMoService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<MoMoPaymentGateway> logger)
        {
            _moMoService = moMoService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<PaymentGatewayResult> ProcessAsync(Order order)
        {
            try
            {
                // Validate order
                ValidateOrder(order);

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogError("HttpContext is null when processing MoMo payment for order {OrderId}", order.Id);
                    throw new UserFriendlyException("Không thể khởi tạo thanh toán. Vui lòng thử lại.");
                }

                var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

                var request = new MoMoPaymentRequest
                {
                    OrderId = order.Id.ToString(),
                    Amount = (long)order.TotalAmount,
                    OrderInfo = $"Thanh toan don hang {order.OrderNumber}",
                    RedirectUrl = $"{baseUrl}/thanh-toan/momo-callback",
                    IpnUrl = $"{baseUrl}/api/payment/momo-ipn",
                    ExtraData = "",
                    Lang = "vi",
                    // IMPROVEMENT: Add request ID for tracking
                    RequestId = Guid.NewGuid().ToString()
                };

                _logger.LogInformation(
                    "Creating MoMo payment for Order {OrderNumber} (ID: {OrderId}), Amount: {Amount}, RequestId: {RequestId}",
                    order.OrderNumber, order.Id, order.TotalAmount, request.RequestId
                );

                var paymentResponse = await _moMoService.CreatePaymentAsync(request);

                // IMPROVEMENT: More detailed error handling
                if (paymentResponse.ErrorCode != 0)
                {
                    _logger.LogError(
                        "MoMo payment creation failed for order {OrderId}. ErrorCode: {ErrorCode}, Message: {Message}",
                        order.Id, paymentResponse.ErrorCode, paymentResponse.Message
                    );

                    // Map common error codes to user-friendly messages
                    var userMessage = GetUserFriendlyMessage(paymentResponse.ErrorCode, paymentResponse.Message);
                    throw new UserFriendlyException(userMessage);
                }

                if (string.IsNullOrWhiteSpace(paymentResponse.PayUrl))
                {
                    _logger.LogError("MoMo returned empty PayUrl for order {OrderId}", order.Id);
                    throw new UserFriendlyException("Không thể tạo liên kết thanh toán MoMo. Vui lòng thử lại.");
                }

                _logger.LogInformation(
                    "MoMo payment URL created successfully for order {OrderId}. TransId: {TransId}",
                    order.Id, paymentResponse.TransId
                );

                return new PaymentGatewayResult
                {
                    RedirectUrl = paymentResponse.PayUrl,
                    Success = true,
                    TransactionId = paymentResponse.TransId.ToString(),
                };
            }
            catch (UserFriendlyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating MoMo payment for order {OrderId}", order.Id);
                throw new UserFriendlyException("Đã có lỗi xảy ra khi kết nối với MoMo. Vui lòng thử lại sau.");
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

            // IMPROVEMENT: MoMo has min/max limits
            if (order.TotalAmount < 1000)
            {
                throw new UserFriendlyException("Số tiền thanh toán tối thiểu là 1,000 VNĐ.");
            }

            //if (order.TotalAmount > 20000000) // 20 million VND
            //{
            //    throw new UserFriendlyException("Số tiền thanh toán vượt quá giới hạn cho phép của MoMo.");
            //}

            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                throw new UserFriendlyException("Đơn hàng này đã được thanh toán.");
            }

            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                throw new UserFriendlyException("Không thể thanh toán cho đơn hàng đã hủy.");
            }
        }

        private string GetUserFriendlyMessage(int errorCode, string originalMessage)
        {
            return errorCode switch
            {
                9000 => "Giao dịch thành công.",
                10 => "Hệ thống đang bảo trì. Vui lòng thử lại sau.",
                11 => "Truy cập bị từ chối. Vui lòng kiểm tra cấu hình.",
                12 => "Phiên bản API không được hỗ trợ.",
                13 => "Xác thực thất bại. Vui lòng thử lại.",
                20 => "Số tiền không hợp lệ.",
                21 => "Số tiền vượt quá giới hạn giao dịch.",
                40 => "RequestId bị trùng lặp.",
                41 => "OrderId không hợp lệ hoặc đã tồn tại.",
                42 => "OrderId không được để trống.",
                43 => "Yêu cầu bị từ chối vì một giao dịch đang được xử lý.",
                1001 => "Giao dịch thanh toán thất bại do tài khoản người dùng không đủ tiền.",
                1002 => "Giao dịch bị từ chối bởi người dùng.",
                1003 => "Giao dịch bị hủy.",
                1004 => "Giao dịch thất bại do số tiền thanh toán vượt quá hạn mức thanh toán của người dùng.",
                1005 => "Giao dịch thất bại do url hoặc QR code đã hết hạn.",
                1006 => "Giao dịch thất bại do người dùng đã từ chối xác nhận thanh toán.",
                1007 => "Giao dịch bị từ chối vì tài khoản người dùng đang ở trạng thái tạm khóa.",
                _ => $"Đã có lỗi xảy ra: {originalMessage}"
            };
        }
    }
}