using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Blogs
{
    public interface IBlog : IApplicationService
    {
        Task<BlogDto> CreateAsync(CreateAndUpdateBlogDto input);
    }
}
