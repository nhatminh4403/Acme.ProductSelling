using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.EntityFrameworkCore;
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

    }
}