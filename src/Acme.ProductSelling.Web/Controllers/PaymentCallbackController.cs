using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Acme.ProductSelling.Payments;


namespace Acme.ProductSelling.Web.Controllers
{
    [Route("/api/payment")]
    public class PaymentCallbackController : Controller  
    {
        private readonly IPaymentCallbackAppService _callbackAppService;

        public PaymentCallbackController(IPaymentCallbackAppService callbackAppService)
        {
            _callbackAppService = callbackAppService;
        }

        [HttpGet("vnpay-ipn")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> VnPayIpn()
        {
            var result = await _callbackAppService.ProcessVnPayIpnAsync(Request.Query);

            // VNPay yêu cầu phải trả về một chuỗi JSON với RspCode và Message
            // để xác nhận đã nhận được IPN.
            // Nếu không trả về đúng định dạng, VNPay sẽ gửi lại IPN nhiều lần.
            if (result != null && !string.IsNullOrEmpty(result.RspCode))
            {
                // Trả về nội dung chính xác mà VNPay mong đợi
                return Content($"{{\"RspCode\":\"{result.RspCode}\",\"Message\":\"{result.Message}\"}}", "application/json");
            }

            // Trường hợp lỗi không xác định
            return Content("{\"RspCode\":\"99\",\"Message\":\"Unknown error\"}", "application/json");
        }
    }
}
