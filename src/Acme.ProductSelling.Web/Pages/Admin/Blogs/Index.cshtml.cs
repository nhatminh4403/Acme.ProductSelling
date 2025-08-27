using Acme.ProductSelling.Blogs;
using Acme.ProductSelling.Identity;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Admin.Blogs
{
    [Authorize(Policy = ProductSellingPermissions.Blogs.Default, Roles = IdentityRoleConsts.Blogger)]
    [Authorize(Policy = ProductSellingPermissions.Blogs.Default, Roles = IdentityRoleConsts.Admin)]

    public class IndexModel : AbpPageModel
    {
        private readonly IBlogAppService _blogAppService;

        public IndexModel(IBlogAppService blogAppService)
        {
            _blogAppService = blogAppService;
        }
        public void OnGet()
        {
        }
    }
}
