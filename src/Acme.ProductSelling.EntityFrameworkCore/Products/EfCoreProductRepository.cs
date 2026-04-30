using Acme.ProductSelling.EntityFrameworkCore;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Specifications.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
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
            var products = await GetQueryableAsync();
            return await products.FirstOrDefaultAsync(c => c.ProductName == name);


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

        public async override Task<Product> GetAsync(Guid id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryableAsync();

            if (!includeDetails)
            {
                query = (await GetDbSetAsync()).AsQueryable();
            }

            return await query.FirstOrDefaultAsync(p => p.Id == id, cancellationToken) ?? throw new EntityNotFoundException(typeof(Product), id);
            //return base.GetAsync(id, includeDetails, cancellationToken);
        }

        public async override Task<IQueryable<Product>> GetQueryableAsync()
        {
            var dbSet = await GetDbSetAsync();
            return dbSet.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.StoreInventories)
                .Include(p => p.SpecificationBase).AsSplitQuery();
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

        public async Task<Product> GetBySlug(string slug)
        {
            var query = await GetQueryableAsync();

            query = query
                        .Include(p => ((MonitorSpecification)p.SpecificationBase).PanelType)
                        .Include(p => ((MotherboardSpecification)p.SpecificationBase).Socket)
                        .Include(p => ((MotherboardSpecification)p.SpecificationBase).Chipset)
                        .Include(p => ((MotherboardSpecification)p.SpecificationBase).FormFactor)
                        .Include(p => ((MotherboardSpecification)p.SpecificationBase).SupportedRamTypes)
                        .Include(p => ((CpuSpecification)p.SpecificationBase).Socket)
                        .Include(p => ((RamSpecification)p.SpecificationBase).RamType)
                        .Include(p => ((KeyboardSpecification)p.SpecificationBase).SwitchType)

                        .Include(p => ((CaseSpecification)p.SpecificationBase).FormFactor)
                        .Include(p => ((CaseSpecification)p.SpecificationBase).Materials)
                            .ThenInclude(m => m.Material)

                        .Include(p => ((CpuCoolerSpecification)p.SpecificationBase).SupportedSockets)
                            .ThenInclude(s => s.Socket);

            // Make exactly ONE call to the database.
            return await query
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UrlSlug.ToLower() == slug.ToLower())
                ?? throw new EntityNotFoundException(typeof(Product), slug);
        }

        public async Task<IQueryable<Product>> GetQueryableWithoutSpecsAsync()
        {
            var query = await GetQueryableAsync();
            return query.Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .AsNoTracking();
        }

    }
}
