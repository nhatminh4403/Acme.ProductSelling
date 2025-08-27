using Acme.ProductSelling.Blogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Blogs
{
    public class DetailsModel : AbpPageModel
    {

        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; }

        public BlogDto Blog { get; private set; }

        

        private readonly IBlogAppService _blogAppService;
        public DetailsModel(IBlogAppService blogAppService)
        {
            _blogAppService = blogAppService;
        }

        public async Task OnGetAsync()
        {
            Blog = await _blogAppService.GetBlogBySlug(Slug);
        }
    }
}
