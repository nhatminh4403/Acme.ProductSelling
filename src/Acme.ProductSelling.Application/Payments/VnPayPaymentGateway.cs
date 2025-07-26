using Acme.ProductSelling.Orders;
using Acme.ProductSelling.VNPay.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Payments
{
    public class VnPayPaymentGateway : IPaymentGateway, ITransientDependency
    {
        public string Name => PaymentConst.VnPay;
        private readonly IVnPayService _vnPayService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VnPayPaymentGateway(IVnPayService vnPayService, IHttpContextAccessor httpContextAccessor)
        {
            _vnPayService = vnPayService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<PaymentGatewayResult> ProcessAsync(Order order)
        {
            var model = new PaymentGateway.VNPay.Dtos.VnPaymentRequestModel
            {
                OrderId = order.Id.ToString(),
                Price = (double)order.TotalAmount,
                Description = $"Thanh toan don hang {order.OrderNumber}",
                CreatedDate = DateTime.Now
            };
            var paymentUrl = _vnPayService.CreatePaymentUrl(_httpContextAccessor.HttpContext, model);
            return Task.FromResult(new PaymentGatewayResult
            {
                RedirectUrl = paymentUrl,
                NextOrderStatus = OrderStatus.PendingPayment // Assuming the next status is Pending after redirection to payment gateway
            });
        }
    }
}
