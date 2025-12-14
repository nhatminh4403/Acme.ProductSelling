// Location: Web/Pages/Products/ProductsByCategory.cshtml.cs
using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Configurations;
using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Web.Pages.Products
{
    public class ProductsByCategoryModel : ProductSellingPageModel
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductLookupAppService _productLookupAppService;

        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PriceRange { get; set; }

        public PagedResultDto<ProductDto> Products { get; set; }
        public string CategoryName { get; set; }
        public SpecificationType CategorySpecType { get; set; }
        public List<PriceRangeDto> AvailablePriceRanges { get; set; } = new();
        public string SelectedPriceRange { get; set; }

        // Price slider bounds
        public decimal MinPriceBound { get; set; }
        public decimal MaxPriceBound { get; set; }
        public decimal CurrentMinPrice { get; set; }
        public decimal CurrentMaxPrice { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        public ProductsByCategoryModel(
            ICategoryRepository categoryRepository,
            IProductLookupAppService productLookupAppService)
        {
            _categoryRepository = categoryRepository;
            _productLookupAppService = productLookupAppService;
        }

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

            // Load available price ranges for this category
            if (CategoryPriceRangeConfiguration.HasPriceRanges(CategorySpecType))
            {
                AvailablePriceRanges = GeneratePriceRangesForCategory(CategorySpecType);

                // Set price bounds based on selected range or overall category bounds
                SetPriceBounds(CategorySpecType, PriceRange);
            }

            // Initial product load
            if (!string.IsNullOrEmpty(PriceRange) &&
                CategoryPriceRangeConfiguration.HasPriceRanges(CategorySpecType))
            {
                var parsedRange = ParsePriceRangeFromUrl(PriceRange, CategorySpecType);
                if (parsedRange.HasValue)
                {
                    var input = new GetProductsByPriceDto
                    {
                        CategoryId = category.Id,
                        MinPrice = parsedRange.Value.Min,
                        MaxPrice = parsedRange.Value.Max,
                        MaxResultCount = PageSize,
                        SkipCount = (CurrentPage - 1) * PageSize
                    };
                    Products = await _productLookupAppService.GetListByProductPrice(input);
                }
                else
                {
                    // Invalid range, load all products
                    Products = await LoadAllProducts(category.Id);
                }
            }
            else
            {
                // No price filter, load all products
                Products = await LoadAllProducts(category.Id);
            }

            SelectedPriceRange = PriceRange;

            return Page();
        }

        private async Task<PagedResultDto<ProductDto>> LoadAllProducts(System.Guid categoryId)
        {
            var input = new GetProductsByCategoryInput
            {
                CategoryId = categoryId,
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize
            };
            return await _productLookupAppService.GetListByCategoryAsync(input);
        }

        private void SetPriceBounds(SpecificationType specType, string priceRangeUrl)
        {
            var ranges = CategoryPriceRangeConfiguration.GetPriceRangesForCategory(specType);

            if (!string.IsNullOrEmpty(priceRangeUrl))
            {
                // Set bounds based on selected range
                foreach (var kvp in ranges)
                {
                    var urlValue = FormatPriceRangeUrl(kvp.Value.Min, kvp.Value.Max);
                    if (urlValue.Equals(priceRangeUrl, System.StringComparison.OrdinalIgnoreCase))
                    {
                        MinPriceBound = kvp.Value.Min;
                        MaxPriceBound = kvp.Value.Max == decimal.MaxValue ? 999_999_999 : kvp.Value.Max;
                        CurrentMinPrice = MinPriceBound;
                        CurrentMaxPrice = MaxPriceBound;
                        return;
                    }
                }
            }

            // Set overall bounds for category
            var allBounds = ranges.Values.ToList();
            MinPriceBound = allBounds.Min(b => b.Min);
            MaxPriceBound = allBounds.Where(b => b.Max != decimal.MaxValue)
                                     .DefaultIfEmpty()
                                     .Max(b => b.Max);

            if (MaxPriceBound == 0) MaxPriceBound = 999_999_999;

            CurrentMinPrice = MinPriceBound;
            CurrentMaxPrice = MaxPriceBound;
        }

        // Helper methods (GeneratePriceRangesForCategory, ParsePriceRangeFromUrl, etc.)
        // ... same as in previous implementation

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
                UrlValue = FormatPriceRangeUrl(kvp.Value.Min, kvp.Value.Max)
            }).ToList();
        }

        private (decimal Min, decimal Max)? ParsePriceRangeFromUrl(string urlValue, SpecificationType specificationType)
        {
            if (string.IsNullOrWhiteSpace(urlValue)) return null;

            var categoryRanges = CategoryPriceRangeConfiguration.GetPriceRangesForCategory(specificationType);

            foreach (var kvp in categoryRanges)
            {
                var expectedUrl = FormatPriceRangeUrl(kvp.Value.Min, kvp.Value.Max);
                if (expectedUrl.Equals(urlValue, System.StringComparison.OrdinalIgnoreCase))
                {
                    return kvp.Value;
                }
            }

            return null;
        }

        private string FormatPriceRangeDisplay(decimal min, decimal max)
        {
            var minFormatted = FormatPriceValue(min);
            var maxFormatted = max == decimal.MaxValue ? "" : FormatPriceValue(max);

            if (min == 0)
            {
                return L["PriceRange:Under", maxFormatted];
            }
            else if (max == decimal.MaxValue)
            {
                return L["PriceRange:Over", minFormatted];
            }
            else
            {
                return L["PriceRange:Between", minFormatted, maxFormatted];
            }
        }

        private string FormatPriceValue(decimal price)
        {
            if (price >= 1_000_000)
            {
                return $"{price / 1_000_000:0.#}M VND";
            }
            else if (price >= 1_000)
            {
                return $"{price / 1_000:0.#}K VND";
            }
            return $"{price:N0} VND";
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
            if (price >= 1_000_000)
            {
                var millions = price / 1_000_000;
                return millions % 1 == 0 ? $"{(int)millions}m" : $"{millions:0.#}m";
            }
            else if (price >= 1_000)
            {
                var thousands = price / 1_000;
                return thousands % 1 == 0 ? $"{(int)thousands}k" : $"{thousands:0.#}k";
            }
            return price.ToString();
        }
    }
}