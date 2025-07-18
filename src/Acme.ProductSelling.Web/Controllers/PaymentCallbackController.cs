using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Acme.ProductSelling.Payments;


namespace Acme.ProductSelling.Web.Controllers
{
    [Route("/api/payment")]
    public class PaymentCallbackController : AbpController  
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

            return Content($"{{\"RspCode\":\"{result.RspCode}\",\"Message\":\"{result.Message}\"}}");
        }
    }
}
