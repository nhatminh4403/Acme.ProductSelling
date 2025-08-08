using Acme.ProductSelling.Orders;
using Acme.ProductSelling.PaymentGateway.PayPal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
namespace Acme.ProductSelling.Payments
{
    public class PayPalPaymentGateway : IPaymentGateway, ITransientDependency
    {
        private readonly IPayPalService _payPalService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IExchangeCurrencyService _exchangeCurrencyService;
        public string Name => PaymentConst.PayPal;


        public PayPalPaymentGateway(IPayPalService payPalService,IHttpContextAccessor httpContextAccessor, IExchangeCurrencyService exchangeCurrencyService)
        {
            _payPalService = payPalService;
            _exchangeCurrencyService = exchangeCurrencyService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaymentGatewayResult> ProcessAsync(Order order)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                throw new UserFriendlyException("Không thể truy cập HttpContext. Vui lòng kiểm tra cấu hình.");
            }
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            var returnUrl = $"{baseUrl}/thanh-toan/paypal-success?orderId={order.Id}";
            var cancelUrl = $"{baseUrl}/thanh-toan/paypal-cancel?orderId={order.Id}";

            try
            {
                decimal totalPriceUSD =
                    await _exchangeCurrencyService.ConvertCurrencyAsync((long)order.TotalAmount,
                                                                            "USD",
                                                                            "VND"
                                                                         );

                totalPriceUSD = Math.Round(totalPriceUSD, 2);

                var paymentUrl = _payPalService.CreatePayment(
                    totalPriceUSD,
                    "USD",
                    returnUrl,
                    cancelUrl
                );
                var result = new PaymentGatewayResult
                {
                    RedirectUrl = paymentUrl,
                    NextOrderStatus = OrderStatus.PendingPayment
                };
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PaypalGateway] Error processing payment: {ex.Message}");

                throw new
                    UserFriendlyException("Đã có lỗi xảy ra khi kết nối với PayPal. " +
                                            "Vui lòng thử lại sau.", ex.Message);
            }
        }
    }

}
