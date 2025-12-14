using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Configurations;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public ProductApiController(IProductLookupAppService productLookupAppService, ICategoryRepository categoryRepository)
        {
            _productLookupAppService = productLookupAppService;
            _categoryRepository = categoryRepository;
        }
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
                    return BadRequest(new { error = "Category slug is required" });
                }

                var category = await _categoryRepository.GetBySlugAsync(categorySlug);
                if (category == null)
                {
                    return NotFound(new { error = "Category not found" });
                }

                // Validate price range against category configuration
                if (!ValidatePriceRange(category.SpecificationType, minPrice, maxPrice))
                {
                    return BadRequest(new { error = "Invalid price range for this category" });
                }

                var input = new GetProductsByPriceDto
                {
                    CategoryId = category.Id,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice == decimal.MaxValue ? 999999999 : maxPrice,
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
                return StatusCode(500, new { error = ex.Message });
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
                    return NotFound(new { error = "Category not found" });
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
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private bool ValidatePriceRange(SpecificationType specType, decimal min, decimal max)
        {
            if (!CategoryPriceRangeConfiguration.HasPriceRanges(specType))
            {
                return false;
            }

            var ranges = CategoryPriceRangeConfiguration.GetPriceRangesForCategory(specType);
            var allBounds = ranges.Values.ToList();

            var absoluteMin = allBounds.Min(b => b.Min);
            var absoluteMax = allBounds.Where(b => b.Max != decimal.MaxValue).DefaultIfEmpty().Max(b => b.Max);

            if (absoluteMax == 0) absoluteMax = 999999999;

            return min >= absoluteMin && max <= absoluteMax && min < max;
        }

        private (decimal Min, decimal Max) GetPriceBoundsForRange(
            SpecificationType specType,
            string priceRangeUrl)
        {
            if (!CategoryPriceRangeConfiguration.HasPriceRanges(specType))
            {
                return (0, 999_999_999);
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
                        return (kvp.Value.Min, kvp.Value.Max == decimal.MaxValue ? 999999999 : kvp.Value.Max);
                    }
                }
            }

            // Return overall bounds for category
            var allBounds = ranges.Values.ToList();
            var min = allBounds.Min(b => b.Min);
            var max = allBounds.Where(b => b.Max != decimal.MaxValue).DefaultIfEmpty().Max(b => b.Max);

            if (max == 0) max = 999999999;

            return (min, max);
        }

        private string FormatPriceRangeUrl(decimal min, decimal max)
        {
            if (max == decimal.MaxValue)
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
    }
}
