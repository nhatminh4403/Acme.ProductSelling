using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Configurations;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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

        /// <summary>
        /// Filter products by price for a category
        /// Used by: ProductsByCategory, ProductByPrice pages
        /// </summary>
        [HttpGet("filter-by-price")]
        public async Task<IActionResult> FilterByPrice(
           [FromQuery] string categorySlug,
           [FromQuery] decimal minPrice,
           [FromQuery] decimal maxPrice,
           [FromQuery] int page = 1,
           [FromQuery] int pageSize = 12,
           [FromQuery] string sortBy = "ProductName")
        {
            try
            {
                if (string.IsNullOrEmpty(categorySlug))
                {
                    return BadRequest(new { success = false, error = "Category slug is required" });
                }

                var category = await _categoryRepository.GetBySlugAsync(categorySlug);
                if (category == null)
                {
                    return NotFound(new { success = false, error = "Category not found" });
                }

                var input = new GetProductsByPriceDto
                {
                    CategoryId = category.Id,
                    MinPrice = minPrice,
                    MaxPrice = CategoryPriceRangeConfiguration.IsOpenEndedRange(maxPrice)
                        ? CategoryPriceRangeConfiguration.GetOpenEndedMaxValue()
                        : maxPrice,
                    MaxResultCount = pageSize,
                    SkipCount = (page - 1) * pageSize,
                    Sorting = sortBy
                };

                var products = await _productLookupAppService.GetListByProductPrice(input);

                return Ok(new
                {
                    success = true,
                    data = products.Items,
                    totalCount = products.TotalCount,
                    currentPage = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(products.TotalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error filtering products by price");
                return StatusCode(500, new { success = false, error = "An error occurred while filtering products" });
            }
        }

        /// <summary>
        /// Filter products by price for search results
        /// Used by: ProductsByName page (search)
        /// </summary>
        [HttpGet("search-with-price")]
        public async Task<IActionResult> SearchWithPrice(
            [FromQuery] string searchKeyword,
            [FromQuery] decimal minPrice,
            [FromQuery] decimal maxPrice,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] string sortBy = "ProductName")
        {
            try
            {
                if (string.IsNullOrEmpty(searchKeyword))
                {
                    return BadRequest(new { success = false, error = "Search keyword is required" });
                }

                var input = new GetProductByNameWithPriceDto
                {
                    Filter = searchKeyword,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    MaxResultCount = pageSize,
                    SkipCount = (page - 1) * pageSize,
                    Sorting = sortBy
                };

                var products = await _productLookupAppService.GetProductsByNameWithPrice(input);

                return Ok(new
                {
                    success = true,
                    data = products.Items,
                    totalCount = products.TotalCount,
                    currentPage = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(products.TotalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching products with price filter");
                return StatusCode(500, new { success = false, error = "An error occurred while searching products" });
            }
        }

        /// <summary>
        /// Filter products by price for manufacturer
        /// Used by: ProductsByManufacturer page
        /// </summary>
        [HttpGet("manufacturer-with-price")]
        public async Task<IActionResult> ManufacturerWithPrice(
            [FromQuery] string categorySlug,
            [FromQuery] string manufacturerSlug,
            [FromQuery] decimal minPrice,
            [FromQuery] decimal maxPrice,
            [FromQuery] Guid? manufacturerId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] string sortBy = "ProductName")
        {
            try
            {
                if (string.IsNullOrEmpty(categorySlug))
                {
                    return BadRequest(new { success = false, error = "Category slug is required" });
                }

                if (string.IsNullOrEmpty(manufacturerSlug) && manufacturerId == null)
                {
                    return BadRequest(new { success = false, error = "Manufacturer slug or ID is required" });
                }

                var category = await _categoryRepository.GetBySlugAsync(categorySlug);
                if (category == null)
                {
                    return NotFound(new { success = false, error = "Category not found" });
                }

                Guid finalManufacturerId;

                if (manufacturerId.HasValue)
                {
                    finalManufacturerId = manufacturerId.Value;
                }
                else
                {
                    var manufacturer = await _manufacturerRepository.GetBySlugAsync(manufacturerSlug);
                    if (manufacturer == null)
                    {
                        return NotFound(new { success = false, error = "Manufacturer not found" });
                    }
                    finalManufacturerId = manufacturer.Id;
                }

                var input = new GetProductsByManufacturerWithPriceDto
                {
                    CategoryId = category.Id,
                    ManufacturerId = finalManufacturerId,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    MaxResultCount = pageSize,
                    SkipCount = (page - 1) * pageSize,
                    Sorting = sortBy
                };

                var products = await _productLookupAppService.GetProductsByManufacturerWithPrice(input);

                return Ok(new
                {
                    success = true,
                    data = products.Items,
                    totalCount = products.TotalCount,
                    currentPage = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling(products.TotalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error filtering manufacturer products by price");
                return StatusCode(500, new { success = false, error = "An error occurred while filtering products" });
            }
        }

        /// <summary>
        /// Get price bounds for a specific category and optional price range
        /// </summary>
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

            // If specific range provided, use those bounds
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

            // Return overall bounds for category
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