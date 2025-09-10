using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    }
}
