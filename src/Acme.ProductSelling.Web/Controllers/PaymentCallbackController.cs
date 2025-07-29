using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Acme.ProductSelling.Payments;
using Acme.ProductSelling.Orders;
using System;


namespace Acme.ProductSelling.Web.Controllers
{
    [Route("/api/payment")]
    public class PaymentCallbackController : Controller
    {
        private readonly IPaymentCallbackAppService _callbackAppService;
        private readonly IOrderAppService _orderAppService;
        public PaymentCallbackController(IPaymentCallbackAppService callbackAppService,
                                            IOrderAppService orderAppService)
        {
            _callbackAppService = callbackAppService;
            _orderAppService = orderAppService;
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
            if (result != null && !string.IsNullOrEmpty(result.VnPayResponseCode))
            {
                // Trả về nội dung chính xác mà VNPay mong đợi
                if (result.Success || result.VnPayResponseCode == "00")
                {

                    // Convert result.OrderId (string) to Guid before passing to GetAsync
                    var orderIdGuid = Guid.Parse(result.OrderId);
                    var order = await _orderAppService.GetAsync(orderIdGuid);

                    if (order == null)
                    {
                        return Content("{\"RspCode\":\"01\",\"Message\":\"Order not found\"}", "application/json");
                    }
                    //{vnp_ResponseCode}/{OrderId}/{OrderNumber}
                    RedirectToPage("/Orders/OrderConfirmation",
                        new { vnp_ResponseCode = result.VnPayResponseCode, OrderId = order.Id, OrderNumber = order.OrderNumber });
                }
                return Content($"{{\"VnPayResponseCode\":\"{result.VnPayResponseCode}\",\"OrderDescription\":\"{result.OrderDescription}\"}}", "application/json");
            }

            // Trường hợp lỗi không xác định
            return Content("{\"RspCode\":\"99\",\"Message\":\"Unknown error\"}", "application/json");
        }
    }
}
