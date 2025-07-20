using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Payments
{
    public class CallbackModel : AbpPageModel
    {

        [BindProperty(SupportsGet = true)]
        public string vnp_ResponseCode { get; set; }

        public string Message { get; private set; }
        public bool IsSuccess { get; private set; }

        

        public void OnGet()
        {
            if (vnp_ResponseCode == "00")
            {
                // Thanh toán thành công từ phía người dùng
                Message = L["PaymentSuccessfulThankYou"];
                IsSuccess = true;
                Alerts.Success(Message);
            }
            else
            {
                // Thanh toán thất bại hoặc bị hủy
                Message = L["PaymentFailedPleaseTryAgain"];
                IsSuccess = false;
                Alerts.Warning(Message);
            }
        }
    }
}
