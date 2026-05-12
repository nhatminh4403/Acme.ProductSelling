using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Acme.ProductSelling.Web.Pages.Admin.Orders.Fulfillment
{
    [Authorize(ProductSellingPermissions.Orders.ViewAll)]
    [Authorize(ProductSellingPermissions.Orders.Fulfill)]
    [Authorize(ProductSellingPermissions.Orders.Edit)]
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
                Response.Redirect(GetUrl("/orders/fulfillment"));
            }
        }
        public async Task<IActionResult> OnPostAsync(Guid orderId)
        {
            try
            {
                var result = await _inStoreOrderAppService.FulfillInStoreOrderAsync(orderId);
                return new JsonResult(new { success = true, data = result });
            }
            catch (UserFriendlyException ex)
            {
                return new JsonResult(new { success = false, error = ex.Message });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to fulfill order {OrderId}", orderId);
                return new JsonResult(new { success = false, error = "An error occurred while fulfilling the order" });
            }
        }
    }
}
