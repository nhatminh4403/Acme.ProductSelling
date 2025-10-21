using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Admin.Orders
{
    [Authorize(ProductSellingPermissions.Orders.Default)]

    public class IndexModel : AbpPageModel
    {
        private readonly IOrderAppService _orderAppService;

        public IndexModel(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
        }

        public void OnGet()
        {
        }
    }
}
