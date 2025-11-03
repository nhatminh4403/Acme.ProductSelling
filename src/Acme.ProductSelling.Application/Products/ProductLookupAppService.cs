using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products
{
    public class ProductLookupAppService : ProductSellingAppService, IProductLookupAppService
    {
        private readonly IRepository<Product, Guid> _productRepository;
        public ProductLookupAppService(IRepository<Product, Guid> productRepository)
        {
            _productRepository = productRepository;
        }
        public virtual async Task<PagedResultDto<ProductDto>> GetListByCategoryAsync(GetProductsByCategoryInput input)
        {
            var queryable = await BuildCategoryQueryAsync(input.CategoryId);
            return await ExecutePagedQueryAsync(queryable, input);
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetListByProductPrice(GetProductsByPriceDto input)
        {
            var queryable = await BuildPriceRangeQueryAsync(input);
            return await ExecutePagedQueryAsync(queryable, input);
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetProductsByName(GetProductByNameDto input)
        {
            var queryable = await BuildNameSearchQueryAsync(input.Filter);
            return await ExecutePagedQueryAsync(queryable, input);
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetProductByManufacturer(GetProductsByManufacturerDto input)
        {
            var queryable = await BuildManufacturerQueryAsync(input);
            return await ExecutePagedQueryAsync(queryable, input);
        }

        private async Task<IQueryable<Product>> BuildCategoryQueryAsync(Guid categoryId)
        {
            var queryable = await _productRepository.GetQueryableAsync();
            return queryable
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId);
        }

        private async Task<IQueryable<Product>> BuildPriceRangeQueryAsync(GetProductsByPriceDto input)
        {
            var queryable = await _productRepository.GetQueryableAsync();
            return queryable
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .AsNoTracking()
                .Where(p => p.CategoryId == input.CategoryId)
                .Where(p => (p.DiscountedPrice ?? p.OriginalPrice) >= input.MinPrice &&
                           (p.DiscountedPrice ?? p.OriginalPrice) <= input.MaxPrice);
        }

        private async Task<IQueryable<Product>> BuildNameSearchQueryAsync(string searchTerm)
        {
            var queryable = await _productRepository.GetQueryableAsync();
            return queryable
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .AsNoTracking()
                .Where(p => p.ProductName.Contains(searchTerm));
        }

        private async Task<IQueryable<Product>> BuildManufacturerQueryAsync(GetProductsByManufacturerDto input)
        {
            var queryable = await _productRepository.GetQueryableAsync();
            return queryable
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Where(p => p.CategoryId == input.CategoryId && p.ManufacturerId == input.ManufacturerId);
        }


        public async Task<ProductDto> GetProductBySlug(string slug)
        {
            var query = await _productRepository.GetQueryableAsync();
            var product = await query
                .AsNoTracking()
                .IncludeAllRelations()
                .FirstOrDefaultAsync(p => p.UrlSlug.ToLower() == slug.ToLower());

            if (product == null)
            {
                throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Product), slug);
            }
            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        private async Task<PagedResultDto<ProductDto>> ExecutePagedQueryAsync<TInput>(
            IQueryable<Product> queryable,
            TInput input)
            where TInput : PagedAndSortedResultRequestDto
        {
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var products = await AsyncExecuter.ToListAsync(
                queryable
                    .OrderBy(p => EF.Property<object>(p, input.Sorting ?? nameof(Product.ProductName)))
                    //.OrderBy(input.Sorting ?? nameof(Product.ProductName))
                    .PageBy(input));

            var productDtos = ObjectMapper.Map<List<Product>, List<ProductDto>>(products);

            return new PagedResultDto<ProductDto>(totalCount, productDtos);
        }
    }
}
