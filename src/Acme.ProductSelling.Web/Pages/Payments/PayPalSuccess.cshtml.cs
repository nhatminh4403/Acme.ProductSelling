using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.PaymentGateway.PayPal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaypalServerSdk.Standard.Models;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Payments
{
    public class PayPalSuccessModel : AbpPageModel
    {
        // IMPORTANT: PayPal now returns 'token' which is the Order ID
        // The old SDK returned PaymentId and PayerID, the new SDK only needs the token (Order ID)
        [BindProperty(SupportsGet = true)]
        public string token { get; set; }

        // Our custom orderId parameter
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
                // Validate that we have the required parameters
                if (string.IsNullOrWhiteSpace(token))
                {
                    Logger.LogError("PayPal callback missing token parameter for orderId: {orderId}", orderId);
                    Alerts.Danger(L["AnErrorOccurredWhileProcessingPayment"]);
                    return RedirectToPage("/Orders/OrderDetail", new { id = orderId });
                }

                if (orderId == Guid.Empty)
                {
                    Logger.LogError("PayPal callback missing orderId parameter. Token: {token}", token);
                    Alerts.Danger(L["AnErrorOccurredWhileProcessingPayment"]);
                    return RedirectToPage("/Orders/Index");
                }

                Logger.LogInformation(
                    "Processing PayPal callback. Token (PayPal Order ID): {token}, Our Order ID: {orderId}",
                    token, orderId
                );

                // Execute/Capture the PayPal order
                // Note: In the new SDK, we only need the token (Order ID), not PayerID
                var executedOrder = await _payPalService.ExecutePaymentAsync(token);

                // Check order status
                // The new SDK uses OrderStatus enum instead of string "state"
                if (executedOrder.Status == OrderStatus.Completed)
                {
                    Logger.LogInformation(
                        "PayPal order completed successfully. Token: {token}, Our Order ID: {orderId}",
                        token, orderId
                    );

                    await _orderAppService.ConfirmPayPalOrderAsync(orderId);
                    Alerts.Success(L["PaymentSuccessfulThankYou"]);
                }
                else
                {
                    Logger.LogWarning(
                        "PayPal order not completed. Status: {Status}, Token: {token}, Our Order ID: {orderId}",
                        executedOrder.Status, token, orderId
                    );
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