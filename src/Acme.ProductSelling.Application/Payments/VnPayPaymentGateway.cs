using Acme.ProductSelling.Orders;
using Acme.ProductSelling.PaymentGateway.VnPay.Dtos;
using Acme.ProductSelling.PaymentGateway.VnPay.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Payments
{
    public class VnPayPaymentGateway : IPaymentGateway, ITransientDependency
    {
        public string Name => PaymentMethods.VnPay;
        private readonly IVnPayService _vnPayService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VnPayPaymentGateway(IVnPayService vnPayService, IHttpContextAccessor httpContextAccessor)
        {
            _vnPayService = vnPayService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<PaymentGatewayResult> ProcessAsync(Order order)
        {
            var model = new VnPaymentRequestModel
            {
                OrderId = order.Id.ToString(),
                Price = (double)order.TotalAmount,
                Description = $"Thanh toan don hang {order.OrderNumber}",
                CreatedDate = DateTime.Now
            };
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new UserFriendlyException("Không thể truy cập HttpContext. Vui lòng kiểm tra cấu hình.");
            }

            var paymentUrl = _vnPayService.CreatePaymentUrl(httpContext, model);
            return Task.FromResult(new PaymentGatewayResult
            {
                RedirectUrl = paymentUrl,
            });
        }
    }
}
