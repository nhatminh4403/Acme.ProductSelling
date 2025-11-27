using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products
{
    public class ProductLookupAppService : ProductSellingAppService, IProductLookupAppService
    {
        private readonly IProductRepository _productRepository;
        // Inject the specific Mapperly mapper
        private readonly ProductToProductDtoMapper _productMapper;

        public ProductLookupAppService(IProductRepository productRepository, ProductToProductDtoMapper productMapper)
        {
            _productRepository = productRepository;
            _productMapper = productMapper;
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetListByCategoryAsync(GetProductsByCategoryInput input)
        {
            return await ExecutePagedQueryAsync(
                 query => query.Where(p => p.CategoryId == input.CategoryId),
                 input
             );
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetListByProductPrice(GetProductsByPriceDto input)
        {
            return await ExecutePagedQueryAsync(
                query => query
                    .Where(p => p.CategoryId == input.CategoryId)
                    .Where(p => (p.DiscountedPrice ?? p.OriginalPrice) >= input.MinPrice &&
                               (p.DiscountedPrice ?? p.OriginalPrice) <= input.MaxPrice),
                input
            );
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetProductsByName(GetProductByNameDto input)
        {
            return await ExecutePagedQueryAsync(
                query => query.Where(p => p.ProductName.Contains(input.Filter)),
                input
            );
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetProductByManufacturer(GetProductsByManufacturerDto input)
        {
            return await ExecutePagedQueryAsync(
                 query => query
                     .Where(p => p.CategoryId == input.CategoryId &&
                                p.ManufacturerId == input.ManufacturerId),
                 input
             );
        }

        public async Task<ProductDto> GetProductBySlug(string slug)
        {
            var query = await _productRepository.GetQueryableAsync();

            var product = await query.AsNoTracking().IncludeAllRelations().Where(p => p.UrlSlug.ToLower() == slug.ToLower()).FirstOrDefaultAsync();
            if (product == null)
            {
                throw new EntityNotFoundException(typeof(Product), slug);
            }

            return _productMapper.Map(product);
        }

        private async Task<PagedResultDto<ProductDto>> ExecutePagedQueryAsync<TInput>(
            Expression<Func<IQueryable<Product>,
            IQueryable<Product>>> filterExpression,
            TInput input)
            where TInput : PagedAndSortedResultRequestDto
        {
            var queryable = await _productRepository.GetQueryableAsync();

            queryable = queryable
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .AsNoTracking();

            queryable = filterExpression.Compile()(queryable);

            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var sortProperty = input.Sorting ?? nameof(Product.ProductName);
            var products = await AsyncExecuter.ToListAsync(
                queryable
                    .OrderBy(p => EF.Property<object>(p, sortProperty))
                    .PageBy(input)
            );

            // Using Mapperly in Projection
            var productDtos = products.Select(p => _productMapper.Map(p)).ToList();

            return new PagedResultDto<ProductDto>(totalCount, productDtos);
        }
        private IQueryable<Product> IncludeSpecifications(IQueryable<Product> query)
        {
            return query
                // --- Group: Monitor ---
                .Include(p => p.MonitorSpecification).ThenInclude(x => x.PanelType)

                // --- Group: Core Components ---
                .Include(p => p.CpuSpecification).ThenInclude(x => x.Socket)
                .Include(p => p.MotherboardSpecification).ThenInclude(x => x.Socket)
                .Include(p => p.MotherboardSpecification).ThenInclude(x => x.Chipset)
                .Include(p => p.MotherboardSpecification).ThenInclude(x => x.FormFactor)
                .Include(p => p.MotherboardSpecification).ThenInclude(x => x.SupportedRamTypes)
                .Include(p => p.RamSpecification).ThenInclude(x => x.RamType)
                .Include(p => p.GpuSpecification)
                .Include(p => p.StorageSpecification)
                .Include(p => p.PsuSpecification).ThenInclude(x => x.FormFactor)

                // --- Group: Case & Cooling (Complex Relations) ---
                .Include(p => p.CaseSpecification).ThenInclude(x => x.FormFactor)
                .Include(p => p.CaseSpecification).ThenInclude(x => x.Materials).ThenInclude(m => m.Material)
                .Include(p => p.CpuCoolerSpecification).ThenInclude(x => x.SupportedSockets).ThenInclude(s => s.Socket)
                .Include(p => p.CaseFanSpecification)

                // --- Group: Peripherals ---
                .Include(p => p.KeyboardSpecification).ThenInclude(x => x.SwitchType)
                .Include(p => p.MouseSpecification)
                .Include(p => p.HeadsetSpecification)
                .Include(p => p.MousepadSpecification)
                .Include(p => p.SpeakerSpecification)
                .Include(p => p.WebcamSpecification)
                .Include(p => p.MicrophoneSpecification)

                // --- Group: Laptop/Mobile/Console ---
                .Include(p => p.LaptopSpecification)
                .Include(p => p.HandheldSpecification)
                .Include(p => p.ConsoleSpecification)

                // --- Group: Accessories & Networking ---
                .Include(p => p.SoftwareSpecification)
                .Include(p => p.CableSpecification)
                .Include(p => p.HubSpecification)
                .Include(p => p.ChargerSpecification)
                .Include(p => p.PowerBankSpecification)
                .Include(p => p.MemoryCardSpecification)
                .Include(p => p.NetworkHardwareSpecification)
                .Include(p => p.ChairSpecification)
                .Include(p => p.DeskSpecification);
        }
    }
}