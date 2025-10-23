using Acme.ProductSelling.Blogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Blogs
{
    [AllowAnonymous]
    public class IndexModel : AbpPageModel
    {
        private readonly IBlogAppService _blogAppService;
        private readonly IBlogRepository _blogRepository;
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 24;

        public PagedResultDto<BlogDto> BlogList { get; set; }

        public IndexModel(IBlogAppService blogAppService, IBlogRepository blogRepository)
        {
            _blogAppService = blogAppService;
            _blogRepository = blogRepository;
        }
        public async Task OnGetAsync()
        {
            var blogs = await _blogRepository.GetListAsync();

            BlogList = new PagedResultDto<BlogDto>
            {
                Items = ObjectMapper.Map<List<Blog>, List<BlogDto>>(blogs),
                TotalCount = blogs.Count
            };
        }
    }
}
