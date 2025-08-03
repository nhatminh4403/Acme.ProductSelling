using Acme.ProductSelling.Orders;
using Acme.ProductSelling.PaymentGateway.MoMo.Models;
using Acme.ProductSelling.PaymentGateway.MoMo.Services;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Payments
{
    public class MoMoGateway : IPaymentGateway, ITransientDependency
    {
        public string Name => PaymentConst.MoMo;
        private readonly IMoMoService _moMoService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public MoMoGateway(IMoMoService moMoService, IHttpContextAccessor httpContextAccessor)
        {
            _moMoService = moMoService;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<PaymentGatewayResult> ProcessAsync(Order order)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            var returnUrl = $"{baseUrl}/thanh-toan/momo-callback";
            var notifyUrl = $"{baseUrl}/api/payment/momo-ipn";

            var request = new MoMoPaymentRequest
            {
                OrderId = order.Id.ToString(),
                Amount = (long)order.TotalAmount,
                OrderInfo = $"Thanh toan don hang {order.OrderNumber}",
                RedirectUrl = returnUrl,
                IpnUrl = notifyUrl, // MoMo sẽ gọi lại URL này khi có IPN
                ExtraData = "", // Thông tin bổ sung nếu cần
                Lang = "vi", // Ngôn ngữ hiển thị
            };


            var paymentUrl = await _moMoService.CreatePaymentAsync(request);
            if(paymentUrl.ErrorCode == 0)
            {
                return new PaymentGatewayResult
                {
                    RedirectUrl = paymentUrl.PayUrl,
                    NextOrderStatus = OrderStatus.PendingPayment
                };
            }
            else
            {
                throw new UserFriendlyException("Đã có lỗi xảy ra khi kết nối với MoMo. " +
                                                "Vui lòng thử lại sau.", paymentUrl.Message);
            }
        }
    }
}
