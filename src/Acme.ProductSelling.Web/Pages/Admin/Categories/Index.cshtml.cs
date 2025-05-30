using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Categories
{
    [Authorize(ProductSellingPermissions.Categories.Default)]
    public class IndexModel : AbpPageModel
    {
        public void OnGet()
        {
        }
    }
}
