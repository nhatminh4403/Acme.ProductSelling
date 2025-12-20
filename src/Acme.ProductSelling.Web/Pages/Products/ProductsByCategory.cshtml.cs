// Location: Web/Pages/Products/ProductsByCategory.cshtml.cs
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
    public class ProductsByCategoryModel : ProductSellingPageModel
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly IProductLookupAppService _productLookupAppService;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryAppService _categoryAppService;

        public ProductsByCategoryModel(
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

        //[BindProperty(SupportsGet = true)]
        //public string PriceRange { get; set; }

        public PagedResultDto<ProductDto> Products { get; set; }
        public string CategoryName { get; set; }
        public SpecificationType CategorySpecType { get; set; }
        public Guid CategoryId { get; set; }
        public decimal MinPriceBound { get; set; }
        public decimal MaxPriceBound { get; set; }

        public decimal CurrentMinPrice { get; set; }
        public decimal CurrentMaxPrice { get; set; }
        public bool ShowManufacturerFilter { get; set; } = true;
        public List<PriceRangeDto> AvailablePriceRanges { get; set; } = new();
        public List<ManufacturerLookupDto> AvailableManufacturers { get; set; }

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

            await CalculateActualPriceBoundsAsync(CategoryId);

            // Check if this category supports price filtering
            if (CategoryPriceRangeConfiguration.HasPriceRanges(CategorySpecType))
            {
                AvailablePriceRanges = GeneratePriceRangesForCategory(CategorySpecType);
            }

            CurrentMinPrice = MinPriceBound;
            CurrentMaxPrice = MaxPriceBound;

            if (ShowManufacturerFilter)
            {
                AvailableManufacturers = await _categoryAppService.GetManufacturersInCategoryAsync(CategoryId);
            }
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
                    MaxPriceBound = 10000000; // 10M default
                    Logger.LogWarning($"No products found in category {categoryId}, using default bounds");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error calculating price bounds for category {CategoryId}", categoryId);
                // Fallback to safe defaults
                MinPriceBound = 0;
                MaxPriceBound = 100000000;
            }
        }

        private decimal RoundDownToNearestThousand(decimal value)
        {
            if (value >= 1000000)
            {
                // Round down to nearest million for values >= 1M
                return Math.Floor(value / 1000000) * 1000000;
            }
            else if (value >= 100000)
            {
                // Round down to nearest 100K for values >= 100K
                return Math.Floor(value / 100000) * 100000;
            }
            else if (value >= 10000)
            {
                // Round down to nearest 10K for values >= 10K
                return Math.Floor(value / 10000) * 10000;
            }
            else
            {
                // Round down to nearest 1K for smaller values
                return Math.Floor(value / 1000) * 1000;
            }
        }
        private decimal RoundUpToNearestThousand(decimal value)
        {
            if (value >= 1000000)
            {
                // Round up to nearest million for values >= 1M
                return Math.Ceiling(value / 1000000) * 1000000;
            }
            else if (value >= 100000)
            {
                // Round up to nearest 100K for values >= 100K
                return Math.Ceiling(value / 100000) * 100000;
            }
            else if (value >= 10000)
            {
                // Round up to nearest 10K for values >= 10K
                return Math.Ceiling(value / 10000) * 10000;
            }
            else
            {
                // Round up to nearest 1K for smaller values
                return Math.Ceiling(value / 1000) * 1000;
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