using Acme.ProductSelling.Blogs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Components.RelatedBlogs
{
    public class RelatedBlogsViewComponent : AbpViewComponent
    {
        private readonly IBlogAppService _blogAppService;

        public RelatedBlogsViewComponent(IBlogAppService blogAppService)
        {
            _blogAppService = blogAppService;
        }
        public async Task<IViewComponentResult> InvokeAsync(
           List<BlogDto> blogs = null,
           string title = null,
           int count = 4)
        {

            if (blogs == null)
            {

                var result = await _blogAppService.GetPublicLatestBlogsAsync(count);
                blogs = result.Items.ToList();
            }
            else
            {
                blogs = blogs.Take(count).ToList();
            }

            var model = new RelatedBlogsViewModel
            {
                Title = title ?? "UI:Title:RelatedBlogs",
                Blogs = blogs
            };

            return View(model);
        }
    }
    public class RelatedBlogsViewModel
    {
        public string Title { get; set; }
        public List<BlogDto> Blogs { get; set; }
    }
}
