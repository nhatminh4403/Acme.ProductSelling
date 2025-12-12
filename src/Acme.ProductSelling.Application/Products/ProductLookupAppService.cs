using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Configurations;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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
        private readonly ProductToProductDtoMapper _productMapper;
        private readonly ICategoryRepository _categoryRepository; // Inject This

        public ProductLookupAppService(IProductRepository productRepository, ProductToProductDtoMapper productMapper, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _productMapper = productMapper;
            _categoryRepository = categoryRepository;
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
            var category = await _categoryRepository.GetAsync(input.CategoryId);

            // 2. Get the hardcoded limits based on Spec Type and Enum
            decimal? min = null;
            decimal? max = null;

            if (input.PriceRange.HasValue)
            {
                var limits = ProductPriceConfiguration.GetLimits(category.SpecificationType, input.PriceRange.Value);
                min = limits.Min;
                max = limits.Max;
            }

            return await ExecutePagedQueryAsync(
                query =>
                {
                    // Filter Category
                    query = query.Where(p => p.CategoryId == input.CategoryId);

                    // Filter Price (Priority: DiscountedPrice -> OriginalPrice)
                    if (min.HasValue)
                    {
                        query = query.Where(p => (p.DiscountedPrice ?? p.OriginalPrice) >= min.Value);
                    }

                    if (max.HasValue)
                    {
                        query = query.Where(p => (p.DiscountedPrice ?? p.OriginalPrice) <= max.Value);
                    }

                    return query;
                },
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
            Func<IQueryable<Product>, IQueryable<Product>> queryBuilder,
            TInput input)
            where TInput : PagedAndSortedResultRequestDto
        {
            var queryable = await _productRepository.GetQueryableAsync();

            queryable = queryable
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .AsNoTracking();

            queryable = queryBuilder(queryable);

            var totalCount = await AsyncExecuter.CountAsync(queryable);

            
            if (string.IsNullOrWhiteSpace(input.Sorting))
            {
                input.Sorting = nameof(Product.ProductName);
            }

            queryable = queryable.OrderBy(input.Sorting).PageBy(input);

            var products = await AsyncExecuter.ToListAsync(queryable);
            var productDtos = products.Select(p => _productMapper.Map(p)).ToList();

            return new PagedResultDto<ProductDto>(totalCount, productDtos);
        }

        
    }
}