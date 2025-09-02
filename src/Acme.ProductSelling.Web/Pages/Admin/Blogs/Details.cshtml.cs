using Acme.ProductSelling.Blogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Admin.Blogs
{
    public class DetailsModel : ProductSellingPageModel
    {

        [BindProperty]
        public Guid Id { get; set; }

        private readonly IBlogAppService _blogAppService;
        public BlogDto Blog { get; set; }
        public DetailsModel(IBlogAppService blogAppService)
        {
            _blogAppService = blogAppService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Blog = await _blogAppService.GetAsync(Id);

            if (Blog == null)
            {
                throw new Exception("Blog not found");

            }
            return Page();
        }
    }
}
