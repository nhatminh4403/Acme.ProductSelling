using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Admin.Manufacturers
{
    [Authorize(ProductSellingPermissions.Manufacturers.Default)]
    public class IndexModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }

        public void OnGet()
        {
            if (Prefix != RoleBasedPrefix)
            {
                Response.Redirect(GetUrl($"/categories"));
            }
        }
    }
}
