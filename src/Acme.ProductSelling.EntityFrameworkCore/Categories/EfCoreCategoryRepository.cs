using Acme.ProductSelling.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.ProductSelling.Categories
{
    public class EfCoreCategoryRepository : EfCoreRepository<ProductSellingDbContext, Category, Guid>, ICategoryRepository
    {
        public EfCoreCategoryRepository(IDbContextProvider<ProductSellingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
        public async Task<Category> FindByNameAsync(string name)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(c => c.Name == name);
        }
        public async Task<List<Category>> GetListAsync()
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.ToListAsync();
        }
    }
   
}
