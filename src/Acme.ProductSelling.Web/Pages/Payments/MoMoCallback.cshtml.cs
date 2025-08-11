using Acme.ProductSelling.PaymentGateway.MoMo.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Payments
{
    public class MoMoCallbackModel : AbpPageModel
    {
        [BindProperty(SupportsGet = true), FromQuery]
        public string partnerCode { get; set; }

        [FromQuery, BindProperty(SupportsGet = true)]
        public string orderId { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string requestId { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public long amount { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string orderInfo { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string orderType { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public long transId { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public int resultCode { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string message { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string payType { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string extraData { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public string signature { get; set; }
        [FromQuery, BindProperty(SupportsGet = true)]
        public long responseTime { get; set; }

        public bool IsSuccess { get; private set; }

        private readonly IMoMoService _moMoService;
        public string OutputMessage { get; set; }
        public MoMoCallbackModel(IMoMoService moMoService)
        {
            _moMoService = moMoService;
        }
        public void OnGet()
        {
            if (resultCode == 0)
            {
                IsSuccess = true;
                OutputMessage = "Giao dịch đã được thực hiện thành công. Chúng tôi đang xử lý đơn hàng của bạn.";
            }
            else
            {
                IsSuccess = false;
                OutputMessage = "Giao dịch không thành công. Vui lòng thử lại hoặc chọn phương thức thanh toán khác.";
            }
        }
    }
}
