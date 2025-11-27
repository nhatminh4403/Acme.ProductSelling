using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Admin.Orders.Fulfillment
{
    [Authorize(ProductSellingPermissions.Orders.ViewAll)]
    [Authorize(ProductSellingPermissions.Orders.Fulfill)]
    [Authorize(ProductSellingPermissions.Orders.Edit)]

    public class IndexModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }

        private readonly IOrderAppService _orderAppService;
        public IndexModel(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
        }
        public void OnGet()
        {
            if (Prefix != RoleBasedPrefix)
            {
                Response.Redirect(GetUrl("/orders/fulfillment"));
            }
        }
    }
}
