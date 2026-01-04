using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Configurations;
using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Categories.Services;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Web.Pages.Products
{
    public class ProductByPriceModel : ProductSellingPageModel
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly IProductLookupAppService _productLookupAppService;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryAppService _categoryAppService;
        public ProductByPriceModel(
            ICategoryRepository categoryRepository,
            IStringLocalizer<ProductSellingResource> localizer,
            IProductLookupAppService productLookupAppService,
            IProductRepository productRepository,
            ICategoryAppService categoryAppService)
        {
            _categoryRepository = categoryRepository;
            _localizer = localizer;
            _productLookupAppService = productLookupAppService;
            _productRepository = productRepository;
            _categoryAppService = categoryAppService;
        }

        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PriceRange { get; set; }
        public List<ManufacturerLookupDto> AvailableManufacturers { get; set; }

        public PagedResultDto<ProductDto> Products { get; set; }
        public string CategoryName { get; set; }
        public SpecificationType CategorySpecType { get; set; }
        public Guid CategoryId { get; set; }

        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string DisplayPriceRangeName { get; set; }
        public List<PriceRangeDto> AvailablePriceRanges { get; set; } = new();

        public decimal ActualMinPrice { get; set; }
        public decimal ActualMaxPrice { get; set; }
        public bool ShowManufacturerFilter { get; set; } = true;

        public int PageSize { get; set; } = 12;
        public int CurrentPage { get; set; } = 1;
        public PagerModel PagerModel { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Slug))
            {
                return NotFound("Category slug is required.");
            }

            var category = await _categoryRepository.GetBySlugAsync(Slug);
            if (category == null)
            {
                return NotFound($"Category '{Slug}' not found.");
            }

            CategoryName = category.Name;
            CategorySpecType = category.SpecificationType;
            CategoryId = category.Id;

            if (!CategoryPriceRangeConfiguration.HasPriceRanges(CategorySpecType))
            {
                return RedirectToPage("/Products/ProductsByCategory", new { slug = Slug });
            }

            AvailablePriceRanges = GeneratePriceRangesForCategory(CategorySpecType);
            if (ShowManufacturerFilter)
            {
                AvailableManufacturers = await _categoryAppService.GetManufacturersInCategoryAsync(CategoryId);
            }
            var parsedRange = ParsePriceRangeFromUrl(PriceRange, CategorySpecType);

            if (!parsedRange.HasValue)
            {
                return RedirectToPage("/Products/ProductsByCategory", new { slug = Slug });
            }

            MinPrice = parsedRange.Value.Min;
            MaxPrice = parsedRange.Value.Max;
            DisplayPriceRangeName = FormatPriceRangeDisplay(MinPrice, MaxPrice);

            if (CategoryPriceRangeConfiguration.IsOpenEndedRange(MaxPrice))
            {
                await SetActualMaxPriceForOpenEndedRangeAsync(CategoryId, MinPrice);
            }
            else
            {
                ActualMaxPrice = MaxPrice;
            }

            await SetActualPriceBoundsAsync(CategoryId, MinPrice, MaxPrice == decimal.MaxValue ? ActualMaxPrice : MaxPrice);

            Logger.LogInformation(
                $"Price Range '{PriceRange}': Config bounds {MinPrice:N0}-{MaxPrice:N0}, " +
                $"Actual product prices {ActualMinPrice:N0}-{ActualMaxPrice:N0}"
            );

            // Fetch products in this price range
            // Use the effective max price (which is reasonable for SQL Server)
            var effectiveMaxPrice = CategoryPriceRangeConfiguration.IsOpenEndedRange(MaxPrice)
                ? ActualMaxPrice
                : MaxPrice;

            var input = new GetProductsByPriceDto
            {
                CategoryId = CategoryId,
                MinPrice = MinPrice,
                MaxPrice = effectiveMaxPrice,
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = "ProductName"
            };

            Products = await _productLookupAppService.GetListByProductPrice(input);

            // Setup pagination
            var routeValues = new Dictionary<string, string>
            {
                { "slug", Slug },
                { "priceRange", PriceRange }
            };

            PagerModel = new PagerModel(
                Products.TotalCount,
                3,
                CurrentPage,
                PageSize,
                Url.Page("./ProductByPrice", routeValues),
                "ProductName"
            );

            return Page();
        }

        /// <summary>
        /// For open-ended ranges (e.g., "over-3m"), find the actual highest price
        /// This is used as the MaxPrice for the slider
        /// </summary>
        private async Task SetActualMaxPriceForOpenEndedRangeAsync(Guid categoryId, decimal minPrice)
        {
            try
            {
                var queryable = await _productRepository.GetQueryableAsync();

                var productsAboveMin = await queryable
                    .Where(p => p.CategoryId == categoryId)
                    .Where(p => (p.DiscountedPrice ?? p.OriginalPrice) >= minPrice)
                    .Select(p => p.DiscountedPrice ?? p.OriginalPrice)
                    .ToListAsync();

                if (productsAboveMin.Any())
                {
                    ActualMaxPrice = productsAboveMin.Max();
                    Logger.LogInformation($"Open-ended range: Setting max to actual highest price: {ActualMaxPrice:N0}");
                }
                else
                {
                    // Fallback: use a reasonable default based on the minimum
                    ActualMaxPrice = Math.Min(minPrice * 10, 999_999_999);
                    Logger.LogWarning($"No products found above {minPrice:N0}, using default max: {ActualMaxPrice:N0}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error finding max price for open-ended range");
                ActualMaxPrice = Math.Min(minPrice * 10, 999_999_999); // Fallback
            }
        }

        /// <summary>
        /// Get actual min/max prices from products in range for display info
        /// </summary>
        private async Task SetActualPriceBoundsAsync(Guid categoryId, decimal minPrice, decimal maxPrice)
        {
            try
            {
                var queryable = await _productRepository.GetQueryableAsync();

                // Ensure maxPrice is reasonable for SQL Server
                var effectiveMaxPrice = Math.Min(maxPrice, 999_999_999);

                var productsInRange = await queryable
                    .Where(p => p.CategoryId == categoryId)
                    .Where(p => (p.DiscountedPrice ?? p.OriginalPrice) >= minPrice)
                    .Where(p => (p.DiscountedPrice ?? p.OriginalPrice) <= effectiveMaxPrice)
                    .Select(p => p.DiscountedPrice ?? p.OriginalPrice)
                    .ToListAsync();

                if (productsInRange.Any())
                {
                    ActualMinPrice = productsInRange.Min();
                    // Don't override ActualMaxPrice if it was already set for open-ended ranges
                    if (ActualMaxPrice == 0)
                    {
                        ActualMaxPrice = productsInRange.Max();
                    }
                }
                else
                {
                    // No products in range
                    ActualMinPrice = minPrice;
                    if (ActualMaxPrice == 0)
                    {
                        ActualMaxPrice = effectiveMaxPrice;
                    }
                    Logger.LogWarning($"No products found in range {minPrice:N0}-{effectiveMaxPrice:N0}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting actual price bounds");
                ActualMinPrice = minPrice;
                if (ActualMaxPrice == 0)
                {
                    ActualMaxPrice = Math.Min(maxPrice, 999_999_999);
                }
            }
        }

        private List<PriceRangeDto> GeneratePriceRangesForCategory(SpecificationType specificationType)
        {
            if (!CategoryPriceRangeConfiguration.HasPriceRanges(specificationType))
            {
                return new List<PriceRangeDto>();
            }

            var ranges = CategoryPriceRangeConfiguration.GetPriceRangesForCategory(specificationType);

            return ranges.Select(kvp => new PriceRangeDto
            {
                Range = kvp.Key,
                MinPrice = kvp.Value.Min,
                MaxPrice = kvp.Value.Max,
                DisplayText = FormatPriceRangeDisplay(kvp.Value.Min, kvp.Value.Max),
                UrlValue = FormatPriceRangeUrl(kvp.Value.Min, kvp.Value.Max),
                LocalizationKey = $"PriceRange:{kvp.Key}"
            }).ToList();
        }

        private (decimal Min, decimal Max)? ParsePriceRangeFromUrl(string urlValue, SpecificationType specificationType)
        {
            if (string.IsNullOrWhiteSpace(urlValue))
            {
                return null;
            }

            urlValue = urlValue.ToLowerInvariant().Trim();

            var categoryRanges = CategoryPriceRangeConfiguration.GetPriceRangesForCategory(specificationType);

            foreach (var kvp in categoryRanges)
            {
                var expectedUrl = FormatPriceRangeUrl(kvp.Value.Min, kvp.Value.Max);
                if (expectedUrl.Equals(urlValue, StringComparison.OrdinalIgnoreCase))
                {
                    return kvp.Value;
                }
            }

            Logger.LogWarning($"Invalid price range URL '{urlValue}' for category {specificationType}");
            return null;
        }

        private string FormatPriceRangeDisplay(decimal min, decimal max)
        {
            var minFormatted = FormatPriceValue(min);
            var maxFormatted = CategoryPriceRangeConfiguration.IsOpenEndedRange(max) ? "" : FormatPriceValue(max);

            if (min == 0)
            {
                return _localizer["PriceRange:Under", maxFormatted];
            }
            else if (CategoryPriceRangeConfiguration.IsOpenEndedRange(max))
            {
                return _localizer["PriceRange:Over", minFormatted];
            }
            else
            {
                return _localizer["PriceRange:Between", minFormatted, maxFormatted];
            }
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

        private string FormatPriceValue(decimal price)
        {
            if (price >= 1000000)
            {
                var millions = price / 1000000;
                return millions % 1 == 0
                    ? $"{(int)millions}M"
                    : $"{millions:0.#}M";
            }
            else if (price >= 1000)
            {
                var thousands = price / 1000;
                return thousands % 1 == 0
                    ? $"{(int)thousands}K"
                    : $"{thousands:0.#}K";
            }
            return $"{price:N0}";
        }

        private string FormatPriceForUrl(decimal price)
        {
            if (price >= 1000000)
            {
                var millions = price / 1000000;
                return millions % 1 == 0
                    ? $"{(int)millions}m"
                    : $"{millions:0.#}m";
            }
            else if (price >= 1000)
            {
                var thousands = price / 1000;
                return thousands % 1 == 0
                    ? $"{(int)thousands}k"
                    : $"{thousands:0.#}k";
            }
            return price.ToString();
        }
    }
}