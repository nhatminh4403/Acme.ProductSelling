using Acme.ProductSelling.Categories;
using Acme.ProductSelling.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.ProductSelling.Blogs
{
    public class EFCoreBlogRepository : EfCoreRepository<ProductSellingDbContext, Blog, Guid>, IBlogRepository
    {
        public EFCoreBlogRepository(IDbContextProvider<ProductSellingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<Blog>> GetListAsync()
        {
            var dbSet = await GetDbSetAsync();
            return dbSet.ToList();
        }
    }
}
