using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
namespace Acme.ProductSelling.Products
{
    public interface IProductRepository : IRepository<Product, Guid>
    {
        Task<Product> FindByNameAsync(string name);
        Task<List<Product>> GetListAsync(int skipCount,
            int maxResultCount,
            string sorting,
            string filter = null);
    }
}
