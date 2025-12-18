// Location: Web/Pages/Products/ProductsByCategory.cshtml.cs
using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Configurations;
using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Products;
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
    public class ProductsByCategoryModel : ProductSellingPageModel
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly IProductLookupAppService _productLookupAppService;
        private readonly IRepository<Product, Guid> _productRepository;

        public ProductsByCategoryModel(
            ICategoryRepository categoryRepository,
            IStringLocalizer<ProductSellingResource> localizer,
            IProductLookupAppService productLookupAppService,
            IRepository<Product, Guid> productRepository)
        {
            _categoryRepository = categoryRepository;
            _localizer = localizer;
            _productLookupAppService = productLookupAppService;
            _productRepository = productRepository;
        }

        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PriceRange { get; set; }

        public PagedResultDto<ProductDto> Products { get; set; }
        public string CategoryName { get; set; }
        public SpecificationType CategorySpecType { get; set; }
        public Guid CategoryId { get; set; }

        // ACTUAL price bounds from products in this category
        public decimal MinPriceBound { get; set; }
        public decimal MaxPriceBound { get; set; }

        // Current filter values (if any)
        public decimal CurrentMinPrice { get; set; }
        public decimal CurrentMaxPrice { get; set; }

        public List<PriceRangeDto> AvailablePriceRanges { get; set; } = new();

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

            // Calculate ACTUAL price bounds from products in this category
            await CalculateActualPriceBoundsAsync(CategoryId);

            // Check if this category supports price filtering
            if (CategoryPriceRangeConfiguration.HasPriceRanges(CategorySpecType))
            {
                AvailablePriceRanges = GeneratePriceRangesForCategory(CategorySpecType);
            }

            // Set current filter values (initially same as bounds)
            CurrentMinPrice = MinPriceBound;
            CurrentMaxPrice = MaxPriceBound;

            // Fetch products
            var input = new GetProductsByCategoryInput
            {
                CategoryId = CategoryId,
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = "ProductName"
            };

            Products = await _productLookupAppService.GetListByCategoryAsync(input);

            // Setup pagination
            var routeValues = new Dictionary<string, string>
            {
                { "slug", Slug }
            };

            PagerModel = new PagerModel(
                Products.TotalCount,
                3,
                CurrentPage,
                PageSize,
                Url.Page("./ProductsByCategory", routeValues),
                "ProductName"
            );

            Logger.LogInformation(
                $"Category '{CategoryName}': Actual price bounds {MinPriceBound:N0} - {MaxPriceBound:N0}"
            );

            return Page();
        }

        /// <summary>
        /// Calculate the actual minimum and maximum prices from products in this category
        /// This gives us the real price range for the slider
        /// </summary>
        private async Task CalculateActualPriceBoundsAsync(Guid categoryId)
        {
            try
            {
                var queryable = await _productRepository.GetQueryableAsync();

                var pricesInCategory = await queryable
                    .Where(p => p.CategoryId == categoryId)
                    .Where(p => p.IsActive) // Only active products
                    .Select(p => p.DiscountedPrice ?? p.OriginalPrice)
                    .ToListAsync();

                if (pricesInCategory.Any())
                {
                    MinPriceBound = pricesInCategory.Min();
                    MaxPriceBound = pricesInCategory.Max();

                    // Round to nice numbers for better UX
                    MinPriceBound = RoundDownToNearestThousand(MinPriceBound);
                    MaxPriceBound = RoundUpToNearestThousand(MaxPriceBound);
                }
                else
                {
                    // No products in category - use reasonable defaults
                    MinPriceBound = 0;
                    MaxPriceBound = 10_000_000; // 10M default
                    Logger.LogWarning($"No products found in category {categoryId}, using default bounds");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error calculating price bounds for category {CategoryId}", categoryId);
                // Fallback to safe defaults
                MinPriceBound = 0;
                MaxPriceBound = 100_000_000;
            }
        }

        /// <summary>
        /// Round down to nearest thousand for cleaner min values
        /// Examples: 2,290,000 → 2,000,000 or 5,450,000 → 5,000,000
        /// </summary>
        private decimal RoundDownToNearestThousand(decimal value)
        {
            if (value >= 1_000_000)
            {
                // Round down to nearest million for values >= 1M
                return Math.Floor(value / 1_000_000) * 1_000_000;
            }
            else if (value >= 100_000)
            {
                // Round down to nearest 100K for values >= 100K
                return Math.Floor(value / 100_000) * 100_000;
            }
            else if (value >= 10_000)
            {
                // Round down to nearest 10K for values >= 10K
                return Math.Floor(value / 10_000) * 10_000;
            }
            else
            {
                // Round down to nearest 1K for smaller values
                return Math.Floor(value / 1_000) * 1_000;
            }
        }

        /// <summary>
        /// Round up to nearest thousand for cleaner max values
        /// Examples: 6,580,000 → 7,000,000 or 15,200,000 → 16,000,000
        /// </summary>
        private decimal RoundUpToNearestThousand(decimal value)
        {
            if (value >= 1_000_000)
            {
                // Round up to nearest million for values >= 1M
                return Math.Ceiling(value / 1_000_000) * 1_000_000;
            }
            else if (value >= 100_000)
            {
                // Round up to nearest 100K for values >= 100K
                return Math.Ceiling(value / 100_000) * 100_000;
            }
            else if (value >= 10_000)
            {
                // Round up to nearest 10K for values >= 10K
                return Math.Ceiling(value / 10_000) * 10_000;
            }
            else
            {
                // Round up to nearest 1K for smaller values
                return Math.Ceiling(value / 1_000) * 1_000;
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