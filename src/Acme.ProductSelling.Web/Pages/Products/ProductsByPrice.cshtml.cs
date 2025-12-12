using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;

namespace Acme.ProductSelling.Web.Pages.Products
{

    public class ProductByPriceModel : ProductSellingPageModel
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly IProductLookupAppService _productLookupAppService;
        public ProductByPriceModel(
            ICategoryRepository categoryRepository,
            IStringLocalizer<ProductSellingResource> localizer,
            IProductLookupAppService productLookupAppService)
        {
            _categoryRepository = categoryRepository;
            _localizer = localizer;
            _productLookupAppService = productLookupAppService;
        }
        public PagedResultDto<ProductDto> Products { get; set; }
        public string CategoryName { get; set; }
        public string DisplayPriceRangeName { get; set; } // Use for Page Title
        public PagerModel PagerModel { get; set; }

        // INPUTS
        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; }

        // Automatically matches "Low", "Medium", etc. from URL
        [BindProperty(SupportsGet = true)]
        public PriceRangeEnum? PriceRange { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Sorting { get; set; } = "ProductName";

        public int PageSize = 10;
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Slug)) return NotFound();
            try
            {
                var category = await _categoryRepository.GetBySlugAsync(Slug);
                CategoryName = category.Name;

                // 1. Prepare Request DTO
                var input = new GetProductsByPriceDto
                {
                    CategoryId = category.Id,
                    PriceRange = PriceRange, // Pass the Enum directly
                    Sorting = Sorting,
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                };

                // 2. Execute
                Products = await _productLookupAppService.GetListByProductPrice(input);

                // 3. Set Display Name (Logic: "Category: Monitor" + "Enum: Price.Low")
                if (PriceRange.HasValue)
                {
                    // Construct the dynamic localization key based on Category type
                    // e.g., "Enum:PriceRange.Monitor.Low"
                    string key = $"Enum:PriceRange.{category.SpecificationType}.{PriceRange}";

                    var localizedText = _localizer[key];
                    // Fallback to generic if specific not found
                    DisplayPriceRangeName = localizedText.ResourceNotFound
                        ? _localizer[$"Enum:PriceRange.{PriceRange}"]
                        : localizedText;
                }
                else
                {
                    DisplayPriceRangeName = L["AllPrices"];
                }

                // 4. Setup Pager
                var routeValues = new Dictionary<string, string>
                {
                    { "slug", Slug },
                    { "priceRange", PriceRange?.ToString() }, // Puts "Low" in URL
                    { "sorting", Sorting }
                };

                PagerModel = new PagerModel(Products.TotalCount, PageSize, CurrentPage, PageSize, Url.Page("./ProductByPrice", routeValues));

                return Page();
            }
            catch (Volo.Abp.Domain.Entities.EntityNotFoundException)
            {
                return NotFound();
            }
        }

        private string GetLocalizedString(string key, CultureInfo culture)
        {
            var originalCulture = CultureInfo.CurrentUICulture;
            try
            {
                CultureInfo.CurrentUICulture = culture;
                return _localizer[key];
            }
            finally
            {
                CultureInfo.CurrentUICulture = originalCulture; // Quan trọng: Khôi phục culture ban đầu
            }
        }
    }
}
