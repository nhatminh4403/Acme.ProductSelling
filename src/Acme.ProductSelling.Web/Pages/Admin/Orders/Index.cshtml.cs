using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Admin.Orders
{
    [Authorize(ProductSellingPermissions.Orders.Default)]

    public class IndexModel : AdminPageModelBase
    {
        private readonly IOrderAppService _orderAppService;

        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }
        public IndexModel(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
        }

        public void OnGet()
        {
            if (Prefix != RoleBasedPrefix)
            {
                Response.Redirect($"/{RoleBasedPrefix}/orders");
            }
        }
    }
}
