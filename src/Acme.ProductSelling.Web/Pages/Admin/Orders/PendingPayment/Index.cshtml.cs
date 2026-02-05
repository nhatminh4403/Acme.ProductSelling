using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Admin.Orders.PendingPayment
{
    [Authorize(ProductSellingPermissions.Orders.ViewAll)]
    [Authorize(ProductSellingPermissions.Orders.Edit)]
    [Authorize(ProductSellingPermissions.Orders.Fulfill)]
    public class IndexModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }

        //private readonly IOrderAppService _orderAppService;
        private readonly IOrderQueryAppService _orderQueryAppService;

        public IndexModel(IOrderQueryAppService orderQueryAppService)
        {
            _orderQueryAppService = orderQueryAppService;
        }

        public void OnGet()
        {
            if (Prefix != RoleBasedPrefix)
            {
                Response.Redirect($"/{RoleBasedPrefix}/orders/pending-payment");
            }
        }
    }
}
