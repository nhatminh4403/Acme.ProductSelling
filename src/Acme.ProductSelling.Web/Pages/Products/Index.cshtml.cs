using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Products
{
    [Authorize(ProductSellingPermissions.Products.Default)]
    public class IndexModel : AbpPageModel
    {
        public void OnGet()
        {
        }
    }
}
