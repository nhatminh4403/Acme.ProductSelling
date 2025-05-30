using Acme.ProductSelling.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public async Task<Manufacturer> GetBySlugAsync(string slug)
        {
            var manufacturers = await GetDbSetAsync();
            return await manufacturers.FirstOrDefaultAsync(c => c.UrlSlug == slug);
        }
    }
}
