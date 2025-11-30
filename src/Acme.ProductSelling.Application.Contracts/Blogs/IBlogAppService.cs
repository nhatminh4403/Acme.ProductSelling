using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace Acme.ProductSelling.Blogs
{
    public interface IBlogAppService : ICrudAppService<BlogDto,
                                                            Guid,
                                                            PagedAndSortedResultRequestDto,
                                                            CreateAndUpdateBlogDto>
    {
        Task<BlogDto> GetBlogBySlug(string slug);
        string ExtractTitleFromHtml(string htmlContent);
        string GenerateUrlSlug(string title);

        Task<PagedResultDto<BlogDto>> GetBlogByBloggerAsync(PagedAndSortedResultRequestDto input);
        Task<PagedResultDto<BlogDto>> GetPublicLatestBlogsAsync(int count);
    }
}
