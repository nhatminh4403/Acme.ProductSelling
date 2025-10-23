using Acme.ProductSelling.Blogs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Admin.Blogs
{
    public class UpdateBlogModel : AbpPageModel
    {
        private readonly IBlogAppService _blogAppService;

        public UpdateBlogModel(IBlogAppService blogAppService)
        {
            _blogAppService = blogAppService;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(Guid id, string title, string content)
        {
            await _blogAppService.UpdateAsync(id, new CreateAndUpdateBlogDto
            {
                Title = title,
                Content = content
            });
            return RedirectToPage("/Admin/Blogs/Index");
        }
    }
}
