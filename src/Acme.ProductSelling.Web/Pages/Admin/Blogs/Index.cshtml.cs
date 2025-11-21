using Acme.ProductSelling.Blogs;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Admin.Blogs
{
    [Authorize(ProductSellingPermissions.Blogs.Default)]
    public class IndexModel : AdminPageModelBase
    {
        private readonly IBlogAppService _blogAppService;

        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }

        public IndexModel(IBlogAppService blogAppService)
        {
            _blogAppService = blogAppService;
        }

        public void OnGet()
        {
            // Validate that the prefix matches user's role
            if (Prefix != RoleBasedPrefix)
            {
                // Redirect to correct URL
                Response.Redirect($"/{RoleBasedPrefix}/blogs");
            }
        }
    }
}