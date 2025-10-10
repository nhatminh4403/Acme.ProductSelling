using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Blogs
{
    public interface IBlogRepository : IRepository<Blog, Guid>
    {
        Task<List<Blog>> GetListAsync();
    }
}
