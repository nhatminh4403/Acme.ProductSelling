using Acme.ProductSelling.Blogs;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Admin.Blogs
{
    /*    [Authorize(Policy = ProductSellingPermissions.Blogs.Default, Roles = IdentityRoleConsts.Blogger)]
        [Authorize(Policy = ProductSellingPermissions.Blogs.Default, Roles = "admin")]*/

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
