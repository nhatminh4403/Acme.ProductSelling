using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Payments
{
    public class PayPalCancelModel : AbpPageModel
    {
        // PayPal returns 'token' (Order ID) even on cancellation
        [BindProperty(SupportsGet = true)]
        public string token { get; set; }

        // Our custom orderId parameter
        [BindProperty(SupportsGet = true)]
        public Guid orderId { get; set; }

        public PayPalCancelModel()
        {
        }

        public async Task<IActionResult> OnGet()
        {
            Logger.LogInformation(
                "PayPal payment cancelled by user. Token: {token}, Our Order ID: {orderId}",
                token, orderId
            );

            Alerts.Warning(L["PaymentCancelledByUser"]);

            if (orderId != Guid.Empty)
            {
                return RedirectToPage("/Orders/OrderDetail", new { id = orderId });
            }

            return RedirectToPage("/Orders/Index");
        }
    }
}