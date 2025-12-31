using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace Acme.ProductSelling.Products.Services
{
    public interface IProductRepository : IRepository<Product, Guid>
    {
        Task<Product> FindByNameAsync(string name);
        Task<List<Product>> GetListAsync();
        Task<List<Product>> GetListAsync(int skipCount,
            int maxResultCount,
            string sorting,
            string filter = null);
        Task<Product> FindByIdAsync(Guid id);
        Task<Product> GetByNameAsync(string name);
    }
}
