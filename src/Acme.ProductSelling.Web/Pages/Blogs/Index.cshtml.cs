using Acme.ProductSelling.Blogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Blogs
{
    public class IndexModel : AbpPageModel
    {
        private readonly IBlogAppService _blogAppService;
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 24;

        public PagedResultDto<BlogDto> BlogList { get; set; }

        public IndexModel(IBlogAppService blogAppService)
        {
            _blogAppService = blogAppService;
            BlogList = new PagedResultDto<BlogDto>();
        }
        public async Task OnGetAsync()
        {
            BlogList = await _blogAppService.GetListAsync(new PagedAndSortedResultRequestDto
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = "CreationTime DESC"
            });
        }
    }
}
