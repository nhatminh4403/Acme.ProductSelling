using Acme.ProductSelling.Orders;
using Acme.ProductSelling.PaymentGateway.PayPal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp;
namespace Acme.ProductSelling.Payments
{
    public class PayPalPaymentGateway : IPaymentGateway
    {
        private readonly IPayPalService _payPalService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IExchangeCurrencyService _exchangeCurrencyService;
        private readonly ILogger<PayPalPaymentGateway> _logger;

        public string Name => PaymentMethods.PayPal;
        private const decimal MIN_USD_AMOUNT = 0.01m;
        private const decimal MAX_USD_AMOUNT = 10000m;

        public PayPalPaymentGateway(IPayPalService payPalService, IHttpContextAccessor httpContextAccessor, IExchangeCurrencyService exchangeCurrencyService, ILogger<PayPalPaymentGateway> logger)
        {
            _payPalService = payPalService;
            _exchangeCurrencyService = exchangeCurrencyService;
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
                    _logger.LogError("HttpContext is null when processing PayPal payment for order {OrderId}", order.Id);
                    throw new UserFriendlyException("Không thể khởi tạo thanh toán. Vui lòng thử lại.");
                }

                var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
                var returnUrl = $"{baseUrl}/thanh-toan/paypal-success?orderId={order.Id}";
                var cancelUrl = $"{baseUrl}/thanh-toan/paypal-cancel?orderId={order.Id}";

                _logger.LogInformation(
                    "Converting currency for PayPal payment. Order {OrderNumber} (ID: {OrderId}), VND Amount: {VndAmount}",
                    order.OrderNumber, order.Id, order.TotalAmount
                );

                // IMPROVEMENT: Better currency conversion with error handling
                decimal totalPriceUSD;
                try
                {
                    totalPriceUSD = await _exchangeCurrencyService.ConvertCurrencyAsync(
                        (long)order.TotalAmount,
                        "USD",
                        "VND"
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Currency conversion failed for order {OrderId}", order.Id);
                    throw new UserFriendlyException(
                        "Không thể chuyển đổi tiền tệ. Vui lòng thử lại sau hoặc chọn phương thức thanh toán khác."
                    );
                }

                // IMPROVEMENT: Validate converted amount
                totalPriceUSD = Math.Round(totalPriceUSD, 2);

                if (totalPriceUSD < MIN_USD_AMOUNT)
                {
                    _logger.LogWarning(
                        "Converted USD amount {UsdAmount} is below minimum for order {OrderId}",
                        totalPriceUSD, order.Id
                    );
                    throw new UserFriendlyException($"Số tiền thanh toán phải lớn hơn ${MIN_USD_AMOUNT} USD.");
                }

                if (totalPriceUSD > MAX_USD_AMOUNT)
                {
                    _logger.LogWarning(
                        "Converted USD amount {UsdAmount} exceeds maximum for order {OrderId}",
                        totalPriceUSD, order.Id
                    );
                    throw new UserFriendlyException($"Số tiền thanh toán vượt quá giới hạn ${MAX_USD_AMOUNT} USD.");
                }

                _logger.LogInformation(
                    "Currency converted successfully. VND: {VndAmount} → USD: {UsdAmount} for order {OrderId}",
                    order.TotalAmount, totalPriceUSD, order.Id
                );

                // Create PayPal payment
                var paymentUrl = _payPalService.CreatePayment(
                    totalPriceUSD,
                    "USD",
                    returnUrl,
                    cancelUrl
                );

                if (string.IsNullOrWhiteSpace(paymentUrl))
                {
                    _logger.LogError("PayPal service returned empty payment URL for order {OrderId}", order.Id);
                    throw new UserFriendlyException("Không thể tạo liên kết thanh toán PayPal. Vui lòng thử lại.");
                }

                _logger.LogInformation("PayPal payment URL created successfully for order {OrderId}", order.Id);

                return new PaymentGatewayResult
                {
                    RedirectUrl = paymentUrl,
                    Success = true,
                    ConvertedAmount = totalPriceUSD, // IMPROVEMENT: Store converted amount
                    ConvertedCurrency = "USD"
                };
            }
            catch (UserFriendlyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating PayPal payment for order {OrderId}", order.Id);
                throw new UserFriendlyException("Đã có lỗi xảy ra khi kết nối với PayPal. Vui lòng thử lại sau.");
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
                throw new UserFriendlyException("Đơn hàng này đã được thanh toán.");
            }

            if (order.OrderStatus == OrderStatus.Cancelled)
            {
                throw new UserFriendlyException("Không thể thanh toán cho đơn hàng đã hủy.");
            }
        }
    }

}
