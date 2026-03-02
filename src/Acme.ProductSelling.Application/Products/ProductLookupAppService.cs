using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Products.Caching;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Helpers;
using Acme.ProductSelling.Products.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products
{
    public class ProductLookupAppService : ProductSellingAppService, IProductLookupAppService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ProductToProductDtoMapper _productMapper;

        private readonly IDistributedCache<PagedResultDto<ProductDto>, string> _pagedCache;
        private readonly IDistributedCache<ProductDto, string> _productSlugCache;
        public ProductLookupAppService(IProductRepository productRepository, ProductToProductDtoMapper productMapper, ICategoryRepository categoryRepository, IDistributedCache<PagedResultDto<ProductDto>, string> pagedCache, IDistributedCache<ProductDto, string> productSlugCache)
        {
            _productRepository = productRepository;
            _productMapper = productMapper;
            _categoryRepository = categoryRepository;
            _pagedCache = pagedCache;
            _productSlugCache = productSlugCache;
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetListByCategoryAsync(GetProductsByCategoryInput input)
        {
            var cacheKey = BuildPagedKey(ProductCacheKeys.ByCategory,
                           input.CategoryId.ToString(), input.Sorting, input.SkipCount, input.MaxResultCount);

            return await _pagedCache.GetOrAddAsync(
                cacheKey,
                () => ExecutePagedQueryAsync(
                    q => q.Where(p => p.CategoryId == input.CategoryId),
                    input),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ProductCacheKeys.LookupListMinutes)
                });

        }

        public virtual async Task<PagedResultDto<ProductDto>> GetListByProductPrice(GetProductsByPriceDto input)
        {
            var cacheKey = BuildPagedKey("product:price:",
                $"{input.CategoryId}:{input.MinPrice}:{input.MaxPrice}:{ManufacturerKey(input.ManufacturerIds)}",
                input.Sorting, input.SkipCount, input.MaxResultCount);

            return await _pagedCache.GetOrAddAsync(
                cacheKey,
                () => ExecutePagedQueryAsync(
                    q => q
                        .Where(p => p.CategoryId == input.CategoryId)
                        .Where(p => (p.DiscountedPrice ?? p.OriginalPrice) >= input.MinPrice &&
                                    (p.DiscountedPrice ?? p.OriginalPrice) <= input.MaxPrice)
                        .WhereIf(input.HasManufacturerFilter,
                            p => input.ManufacturerIds.Contains(p.ManufacturerId)),
                    input),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ProductCacheKeys.LookupListMinutes)
                });
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetProductsByName(GetProductByNameDto input)
        {
            return await ExecutePagedQueryAsync(
                query => query.Where(p => p.ProductName.Contains(input.Filter)),
                input
            );
        }

        //public virtual async 
        public virtual async Task<PagedResultDto<ProductDto>> GetProductsByManufacturer(GetProductsByManufacturerDto input)
        {
            var cacheKey = BuildPagedKey("product:manufacturer:",
                $"{input.CategoryId}:{input.ManufacturerId}",
                input.Sorting, input.SkipCount, input.MaxResultCount);

            return await _pagedCache.GetOrAddAsync(
                cacheKey,
                () => ExecutePagedQueryAsync(
                    q => q.Where(p => p.CategoryId == input.CategoryId &&
                                      p.ManufacturerId == input.ManufacturerId),
                    input),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ProductCacheKeys.LookupListMinutes)
                });
        }

        public async Task<ProductDto> GetProductBySlug(string slug)
        {
            var cacheKey = ProductCacheKeys.DetailBySlug + slug.ToLower();

            var cached = await _productSlugCache.GetAsync(cacheKey);
            if (cached != null)
                return cached;

            var query = await _productRepository.GetQueryableAsync();
            var product = await query
                .AsNoTracking()
                .IncludeAllRelations()
                .Where(p => p.UrlSlug.ToLower() == slug.ToLower())
                .FirstOrDefaultAsync();

            if (product == null)
                throw new EntityNotFoundException(typeof(Product), slug);

            var dto = _productMapper.Map(product);

            await _productSlugCache.SetAsync(
                cacheKey,
                dto,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ProductCacheKeys.DetailTtlMinutes)
                });

            return dto;
        }
        public virtual async Task<PagedResultDto<ProductDto>> GetProductsByNameWithPrice(GetProductByNameWithPriceDto input)
        {
            return await ExecutePagedQueryAsync(
                q => q
                    .Where(p => p.ProductName.Contains(input.Filter))
                    .Where(p => (p.DiscountedPrice ?? p.OriginalPrice) >= input.MinPrice &&
                                (p.DiscountedPrice ?? p.OriginalPrice) <= input.MaxPrice)
                    .WhereIf(input.HasManufacturerFilter,
                        p => input.ManufacturerIds.Contains(p.ManufacturerId)),
                input);
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetProductsByManufacturerWithPrice(GetProductsByManufacturerWithPriceDto input)
        {
            var cacheKey = BuildPagedKey("product:manufacturer-price:",
                $"{input.CategoryId}:{input.ManufacturerId}:{input.MinPrice}:{input.MaxPrice}",
                input.Sorting, input.SkipCount, input.MaxResultCount);

            return await _pagedCache.GetOrAddAsync(
                cacheKey,
                () => ExecutePagedQueryAsync(
                    q => q
                        .Where(p => p.CategoryId == input.CategoryId &&
                                    p.ManufacturerId == input.ManufacturerId)
                        .Where(p => (p.DiscountedPrice ?? p.OriginalPrice) >= input.MinPrice &&
                                    (p.DiscountedPrice ?? p.OriginalPrice) <= input.MaxPrice),
                    input),
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ProductCacheKeys.LookupListMinutes)
                });
        }

        public async Task<List<FeaturedCategoryProductsDto>> GetFeaturedProductCarouselsAsync()
        {
            var result = new List<FeaturedCategoryProductsDto>();

            // 1. Define the Spec Types we want
            var featuredSpecTypes = new[]
            {
                SpecificationType.Mouse,
                SpecificationType.Laptop,
                SpecificationType.Monitor,
                SpecificationType.Keyboard
            };
            var categories = await _categoryRepository.GetListAsync(c => featuredSpecTypes.Contains(c.SpecificationType));

            var categoriesToFeature = categories.Take(4).ToList();

            // 2. Get Categories (Lightweight query)
            foreach (var category in categoriesToFeature)
            {
                var query = await _productRepository.GetQueryableAsync();

                var rawProducts = await query
                    .AsNoTracking()
                    .Where(p => p.CategoryId == category.Id && p.StockCount > 0)
                    .OrderBy(p => Guid.NewGuid())
                    .Take(20) // Get 20, then shuffle and take 10
                              //.Select(p => new  // Project only needed fields
                              //{
                              //    p.Id,
                              //    p.ProductName,
                              //    p.UrlSlug,
                              //    p.ImageUrl,
                              //    p.OriginalPrice,
                              //    p.DiscountedPrice,
                              //    p.DiscountPercent,
                              //    CategoryName = p.Category.Name,
                              //    ManufacturerName = p.Manufacturer.Name,
                              //    p.StockCount
                              //})
                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer)
                    .Include(p => p.StoreInventories)
                    .ToListAsync();


                rawProducts = rawProducts.OrderBy(x => Guid.NewGuid()).Shuffle().ToList();
                if (!rawProducts.Any()) continue;

                // Map to DTO
                var productDtos = rawProducts.Select(p => _productMapper.Map(p)).ToList();

                result.Add(new FeaturedCategoryProductsDto
                {
                    Category = ObjectMapper.Map<Category, CategoryDto>(category),
                    Products = productDtos
                });
            }
            return result;

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

            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                if (input.Sorting == "price-asc")
                {
                    queryable = queryable.OrderBy(p => p.DiscountedPrice ?? p.OriginalPrice);
                }
                else if (input.Sorting == "price-desc")
                {
                    queryable = queryable.OrderByDescending(p => p.DiscountedPrice ?? p.OriginalPrice);
                }
                else
                {
                    queryable = queryable.OrderBy(input.Sorting);
                }
            }
            else
            {
                // Default sorting
                queryable = queryable.OrderBy(p => p.ProductName);
            }
            var products = await AsyncExecuter.ToListAsync(
                queryable
                    .PageBy(input)
            );

            // Using Mapperly in Projection
            var productDtos = products.Select(p => _productMapper.Map(p)).ToList();

            return new PagedResultDto<ProductDto>(totalCount, productDtos);
        }

        private static string BuildPagedKey(string prefix, string filter, string sorting, int skip, int max)
    => $"{prefix}{filter}:{sorting ?? "default"}:{skip}:{max}";

        private static string ManufacturerKey(System.Collections.Generic.List<Guid> ids)
            => ids == null || ids.Count == 0 ? "all" : string.Join("-", ids.OrderBy(x => x));

    }
}