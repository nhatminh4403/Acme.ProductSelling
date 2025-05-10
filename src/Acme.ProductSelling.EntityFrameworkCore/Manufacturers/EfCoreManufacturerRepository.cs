using Acme.ProductSelling.Categories;
using Acme.ProductSelling.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.ProductSelling.Manufacturers
{
    public class EfCoreManufacturerRepository : EfCoreRepository<ProductSellingDbContext, Manufacturer, Guid>, IManufacturerRepository
        
    {
        public EfCoreManufacturerRepository(IDbContextProvider<ProductSellingDbContext> dbContext) : base(dbContext)
        {
        }
        public async Task<List<Manufacturer>> GetListAsync()
        {
            var manufacturers = await GetDbSetAsync();
            return await manufacturers.ToListAsync();
        }
       
    }
}
