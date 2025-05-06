using Acme.ProductSelling.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if(product == null)
            {
                throw new Exception($"Product with name {name} not found");
            }
            return product;
        }
        public Task<List<Product>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null)
        {
            throw new NotImplementedException();
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
