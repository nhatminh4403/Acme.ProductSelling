using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Acme.ProductSelling.Web.Pages.Admin.Orders.PendingPayment
{
    [Authorize(ProductSellingPermissions.Orders.Default)]
    public class IndexModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }

        private readonly IOrderQueryAppService _orderQueryAppService;
        private readonly IInStoreOrderAppService _inStoreOrderAppService;

        public IndexModel(
            IOrderQueryAppService orderQueryAppService,
            IInStoreOrderAppService inStoreOrderAppService)
        {
            _orderQueryAppService = orderQueryAppService;
            _inStoreOrderAppService = inStoreOrderAppService;
        }

        public void OnGet()
        {
            if (Prefix != RoleBasedPrefix)
            {
                Response.Redirect($"/{RoleBasedPrefix}/orders/pending-payment");
            }
        }

        [Authorize(ProductSellingPermissions.Orders.Complete)]
        public async Task<IActionResult> OnPostCompletePaymentAsync(Guid orderId, [FromBody] CompleteInStorePaymentDto input)
        {
            try
            {
                var result = await _inStoreOrderAppService.CompleteInStorePaymentAsync(orderId, input);
                return new JsonResult(new { success = true, data = result });
            }
            catch (UserFriendlyException ex)
            {
                return new JsonResult(new { success = false, error = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to complete payment for order {OrderId}", orderId);
                return new JsonResult(new { success = false, error = "An error occurred while processing payment" });
            }
        }
    }
}
