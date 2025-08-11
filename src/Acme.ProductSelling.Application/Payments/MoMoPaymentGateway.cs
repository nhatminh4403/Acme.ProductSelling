using Acme.ProductSelling.Orders;
using Acme.ProductSelling.PaymentGateway.MoMo.Models;
using Acme.ProductSelling.PaymentGateway.MoMo.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Payments
{
    public class MoMoPaymentGateway : IPaymentGateway, ITransientDependency
    {
        public string Name => PaymentMethods.MoMo;
        private readonly IMoMoService _moMoService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public MoMoPaymentGateway(IMoMoService moMoService, IHttpContextAccessor httpContextAccessor)
        {
            _moMoService = moMoService;
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
            if (paymentUrl.ErrorCode == 0)
            {
                return new PaymentGatewayResult
                {
                    RedirectUrl = paymentUrl.PayUrl,
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
