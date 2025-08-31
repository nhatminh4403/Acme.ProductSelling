using Acme.ProductSelling.Blogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Admin.Blogs
{
    public class CreateBlogModel : AbpPageModel
    {
        private readonly IBlogAppService _blogAppService;

        public CreateBlogModel(IBlogAppService blogAppService)
        {
            _blogAppService = blogAppService;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(CreateAndUpdateBlogDto input)
        {
            await _blogAppService.CreateAsync(input);
            return RedirectToPage("/Admin/Blogs/Index");
        }
    }
}
