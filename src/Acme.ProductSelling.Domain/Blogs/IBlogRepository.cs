using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Blogs
{
    public interface IBlogRepository : IRepository<Blog, Guid>
    {
        Task<List<Blog>> GetListAsync();
    }
}
