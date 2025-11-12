using Acme.ProductSelling.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
namespace Acme.ProductSelling.Products
{
    public class EfCoreProductRepository : EfCoreRepository<ProductSellingDbContext, Product, Guid>, IProductRepository
    {
        public EfCoreProductRepository(IDbContextProvider<ProductSellingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
        public async Task<Product> FindByNameAsync(string name)
        {
            var dbSet = await GetDbSetAsync();
            var product = await dbSet.FirstOrDefaultAsync(c => c.ProductName == name);
            if (product == null)
            {
                throw new Exception($"Product with name {name} not found");
            }
            return product;
        }
        public async Task<List<Product>> GetListAsync(int skipCount,
            int maxResultCount, string sorting, string filter = null)
        {
            var dbSet = await GetDbSetAsync();
            var query = dbSet.AsQueryable();
            if (!filter.IsNullOrWhiteSpace())
            {
                query = query.Where(c => c.CategoryId.ToString() == filter);
            }

            return await query.OrderBy(c => EF.Property<object>(c, sorting))
                              .Skip(skipCount)
                              .Take(maxResultCount)
                              .ToListAsync();
        }

        public async Task<List<Product>> GetListAsync()
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.ToListAsync();
        }
        public async Task<Product> FindByIdAsync(Guid id)
        {
            var dbSet = await GetDbSetAsync();
            var product = await dbSet.FirstOrDefaultAsync(c => c.Id == id);
            if (product == null)
            {
                throw new Exception($"Product with name {id} not found");
            }
            return product;
        }
    }
}
