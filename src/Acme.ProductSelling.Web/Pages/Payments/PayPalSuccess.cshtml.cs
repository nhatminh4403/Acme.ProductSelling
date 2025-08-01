using Acme.ProductSelling.Orders;
using Acme.ProductSelling.PaymentGateway.PayPal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Payments
{
    public class PayPalSuccessModel : AbpPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string paymentId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string token { get; set; }
        [BindProperty(SupportsGet = true)]
        public string PayerID { get; set; }

        // OrderId chúng ta đã tự truyền đi
        [BindProperty(SupportsGet = true)]
        public Guid orderId { get; set; }


        private readonly IPayPalService _payPalService;
        private readonly IOrderAppService _orderAppService;

        public PayPalSuccessModel(IPayPalService payPalService, IOrderAppService orderAppService)
        {
            _payPalService = payPalService;
            _orderAppService = orderAppService;
        }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                var executedPayment = _payPalService.ExecutePayment(paymentId, PayerID); 
                if(executedPayment.state.Equals("approved", StringComparison.OrdinalIgnoreCase))
                {

                    await _orderAppService.ConfirmPayPalOrderAsync(orderId);

                    Alerts.Success(L["PaymentSuccessfulThankYou"]);
                }
                else
                {
                    Logger.LogWarning("PayPal payment not approved. State: {state}", executedPayment.state);
                    Alerts.Warning(L["PaymentNotApproved"]);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Lỗi khi xử lý callback từ PayPal cho orderId: {orderId}", orderId);
                Alerts.Danger(L["AnErrorOccurredWhileProcessingPayment"]);
            }
            return RedirectToPage("/Orders/OrderDetail", new { id = orderId });
        }
    }
}
