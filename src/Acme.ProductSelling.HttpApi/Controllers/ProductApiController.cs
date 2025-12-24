using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Configurations;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Helpers;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductApiController : AbpController
    {
        private readonly IProductLookupAppService _productLookupAppService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IManufacturerRepository _manufacturerRepository;

        public ProductApiController(
            IProductLookupAppService productLookupAppService,
            ICategoryRepository categoryRepository,
            IManufacturerRepository manufacturerRepository)
        {
            _productLookupAppService = productLookupAppService;
            _categoryRepository = categoryRepository;
            _manufacturerRepository = manufacturerRepository;
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetProducts(
     [FromQuery] string categorySlug = null,
     [FromQuery] string manufacturerIds = null, 
     [FromQuery] string searchKeyword = null,
     [FromQuery] decimal? minPrice = null,
     [FromQuery] decimal? maxPrice = null,
     [FromQuery] string sortBy = "featured",
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 12)
        {
            try
            {
                var abpSorting = ProductSortMapper.MapToAbpSorting(sortBy);

                var manufacturerIdsList = ParseManufacturerIds(manufacturerIds);

                // Scenario 1: Search
                if (!string.IsNullOrWhiteSpace(searchKeyword))
                {
                    var searchInput = new GetProductByNameWithPriceDto
                    {
                        Filter = searchKeyword,
                        MinPrice = minPrice ?? 0,
                        MaxPrice = maxPrice ?? decimal.MaxValue,
                        ManufacturerIds = manufacturerIdsList, // ✅ PASS LIST
                        Sorting = abpSorting,
                        MaxResultCount = pageSize,
                        SkipCount = (page - 1) * pageSize
                    };

                    var searchResults = await _productLookupAppService.GetProductsByNameWithPrice(searchInput);

                    return Ok(new
                    {
                        success = true,
                        data = searchResults.Items,
                        totalCount = searchResults.TotalCount,
                        currentPage = page,
                        pageSize = pageSize,
                        totalPages = (int)Math.Ceiling(searchResults.TotalCount / (double)pageSize),
                        filterType = "search",
                        appliedManufacturers = manufacturerIdsList.Count // Debug info
                    });
                }

                // Validate category
                if (string.IsNullOrWhiteSpace(categorySlug))
                {
                    return BadRequest(new { success = false, error = "Category slug or search keyword is required" });
                }

                var category = await _categoryRepository.GetBySlugAsync(categorySlug);
                if (category == null)
                {
                    return NotFound(new { success = false, error = "Category not found" });
                }

                // Scenario 2: Category + Price (with optional multiple manufacturers)
                if (minPrice.HasValue && maxPrice.HasValue)
                {
                    var input = new GetProductsByPriceDto
                    {
                        CategoryId = category.Id,
                        MinPrice = minPrice.Value,
                        MaxPrice = maxPrice.Value,
                        ManufacturerIds = manufacturerIdsList, // ✅ PASS LIST
                        Sorting = abpSorting,
                        MaxResultCount = pageSize,
                        SkipCount = (page - 1) * pageSize
                    };

                    var products = await _productLookupAppService.GetListByProductPrice(input);

                    return Ok(new
                    {
                        success = true,
                        data = products.Items,
                        totalCount = products.TotalCount,
                        currentPage = page,
                        pageSize = pageSize,
                        totalPages = (int)Math.Ceiling(products.TotalCount / (double)pageSize),
                        filterType = manufacturerIdsList.Any() ? "category-price-manufacturers" : "category-price",
                        appliedManufacturers = manufacturerIdsList.Count
                    });
                }

                // Scenario 3: Category only
                var categoryInput = new GetProductsByCategoryInput
                {
                    CategoryId = category.Id,
                    Sorting = abpSorting,
                    MaxResultCount = pageSize,
                    SkipCount = (page - 1) * pageSize
                };

                var categoryProducts = await _productLookupAppService.GetListByCategoryAsync(categoryInput);

                return Ok(new
                {
                    success = true,
                    data = categoryProducts.Items,
                    totalCount = categoryProducts.TotalCount,
                    currentPage = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(categoryProducts.TotalCount / (double)pageSize),
                    filterType = "category"
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting products. ManufacturerIds: {ManufacturerIds}", manufacturerIds);
                return StatusCode(500, new { success = false, error = "An error occurred while loading products" });
            }
        }
        [HttpGet("price-bounds/{categorySlug}")]
        public async Task<IActionResult> GetPriceBounds(
            string categorySlug,
            [FromQuery] string priceRange = null)
        {
            try
            {
                var category = await _categoryRepository.GetBySlugAsync(categorySlug);
                if (category == null)
                {
                    return NotFound(new { success = false, error = "Category not found" });
                }

                var bounds = GetPriceBoundsForRange(category.SpecificationType, priceRange);

                return Ok(new
                {
                    success = true,
                    minPrice = bounds.Min,
                    maxPrice = bounds.Max,
                    categoryName = category.Name,
                    hasPriceRanges = CategoryPriceRangeConfiguration.HasPriceRanges(category.SpecificationType)
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting price bounds for category");
                return StatusCode(500, new { success = false, error = "An error occurred while getting price bounds" });
            }
        }

        #region Private Helper Methods

        private (decimal Min, decimal Max) GetPriceBoundsForRange(
            SpecificationType specType,
            string priceRangeUrl)
        {
            if (!CategoryPriceRangeConfiguration.HasPriceRanges(specType))
            {
                return (0, CategoryPriceRangeConfiguration.GetOpenEndedMaxValue());
            }

            var ranges = CategoryPriceRangeConfiguration.GetPriceRangesForCategory(specType);

            if (!string.IsNullOrEmpty(priceRangeUrl))
            {
                foreach (var kvp in ranges)
                {
                    var urlValue = FormatPriceRangeUrl(kvp.Value.Min, kvp.Value.Max);
                    if (urlValue.Equals(priceRangeUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        var maxValue = CategoryPriceRangeConfiguration.IsOpenEndedRange(kvp.Value.Max)
                            ? CategoryPriceRangeConfiguration.GetOpenEndedMaxValue()
                            : kvp.Value.Max;
                        return (kvp.Value.Min, maxValue);
                    }
                }
            }

            var allBounds = ranges.Values.ToList();
            var min = allBounds.Min(b => b.Min);
            var max = allBounds
                .Where(b => !CategoryPriceRangeConfiguration.IsOpenEndedRange(b.Max))
                .DefaultIfEmpty()
                .Max(b => b.Max);

            if (max == 0)
                max = CategoryPriceRangeConfiguration.GetOpenEndedMaxValue();

            return (min, max);
        }

        private string FormatPriceRangeUrl(decimal min, decimal max)
        {
            if (CategoryPriceRangeConfiguration.IsOpenEndedRange(max))
            {
                return $"over-{FormatPriceForUrl(min)}";
            }
            else if (min == 0)
            {
                return $"under-{FormatPriceForUrl(max + 1)}";
            }
            else
            {
                return $"{FormatPriceForUrl(min)}-to-{FormatPriceForUrl(max)}";
            }
        }
        private List<Guid> ParseManufacturerIds(string manufacturerIdsParam)
        {
            if (string.IsNullOrWhiteSpace(manufacturerIdsParam))
            {
                return new List<Guid>();
            }

            var ids = new List<Guid>();
            var parts = manufacturerIdsParam.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                if (Guid.TryParse(part.Trim(), out var guid))
                {
                    ids.Add(guid);
                }
            }

            return ids;
        }
        private string FormatPriceForUrl(decimal price)
        {
            if (price >= 1000000)
            {
                var millions = price / 1000000;
                return millions % 1 == 0 ? $"{(int)millions}m" : $"{millions:0.#}m";
            }
            else if (price >= 1000)
            {
                var thousands = price / 1000;
                return thousands % 1 == 0 ? $"{(int)thousands}k" : $"{thousands:0.#}k";
            }
            return price.ToString();
        }

        #endregion
    }
}