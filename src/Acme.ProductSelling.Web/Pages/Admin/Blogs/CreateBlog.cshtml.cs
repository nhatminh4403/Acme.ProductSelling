using Acme.ProductSelling.Blogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Admin.Blogs
{
    public class CreateBlogModel : ProductSellingPageModel
    {
        private readonly IBlogAppService _blogAppService;
        [BindProperty]
        public CreateAndUpdateBlogDto BlogPost { get; set; }
        [BindProperty]
        public IFormFile CoverImageFile { get; set; }
        public CreateBlogModel(IBlogAppService blogAppService)
        {
            _blogAppService = blogAppService;
        }
        public void OnGet()
        {
            BlogPost = new CreateAndUpdateBlogDto();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _blogAppService.CreateAsync(BlogPost);
            return RedirectToPage("/Admin/Blogs/Index");
        }
    }
}
