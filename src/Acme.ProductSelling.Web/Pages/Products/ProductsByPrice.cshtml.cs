using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Products;
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
        private readonly IProductAppService _productAppService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly IStringLocalizerFactory _localizerFactory;
        public ProductByPriceModel(IProductAppService productAppService,
            ICategoryRepository categoryRepository, IStringLocalizer<ProductSellingResource> localizer, IStringLocalizerFactory localizerFactory)
        {
            _productAppService = productAppService;
            _categoryRepository = categoryRepository;
            _localizer = localizer;
            _localizerFactory = localizerFactory;
        }
        public PagedResultDto<ProductDto> Products { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string Filter { get; set; }
        public string Sorting { get; set; } = "ProductName";
        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; }
        public string CategoryName { get; set; }
        public int PageSize = 6;
        public int CurrentPage { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public string PriceRangeAlias { get; set; }
        public PagerModel PagerModel { get; set; }
        public string DisplayPriceRangeName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                string incomingAliasLower = this.PriceRangeAlias?.ToLowerInvariant();
                var cultureEn = new CultureInfo("en");
                var cultureVi = new CultureInfo("vi");

                var aliasUnder1M_en = GetLocalizedString("PriceRangeAlias:Under1Million", cultureEn).ToLowerInvariant();
                var aliasUnder1M_vi = GetLocalizedString("PriceRangeAlias:Under1Million", cultureVi).ToLowerInvariant();

                var alias1Mto5M_en = GetLocalizedString("PriceRangeAlias:From1MillionTo5Million", cultureEn).ToLowerInvariant();
                var alias1Mto5M_vi = GetLocalizedString("PriceRangeAlias:From1MillionTo5Million", cultureVi).ToLowerInvariant();

                var alias5Mto20M_en = GetLocalizedString("PriceRangeAlias:From5MillionTo20Million", cultureEn).ToLowerInvariant();
                var alias5Mto20M_vi = GetLocalizedString("PriceRangeAlias:From5MillionTo20Million", cultureVi).ToLowerInvariant();

                var aliasOver20M_en = GetLocalizedString("PriceRangeAlias:Over20Million", cultureEn).ToLowerInvariant();
                var aliasOver20M_vi = GetLocalizedString("PriceRangeAlias:Over20Million", cultureVi).ToLowerInvariant();
                bool matched = false;

                if (incomingAliasLower == aliasUnder1M_en || incomingAliasLower == aliasUnder1M_vi)
                {
                    MinPrice = 0; MaxPrice = 999999;
                    DisplayPriceRangeName = _localizer["PriceRangeAlias:Under1Million"];
                    matched = true;
                }
                else if (incomingAliasLower == alias1Mto5M_en || incomingAliasLower == alias1Mto5M_vi)
                {
                    MinPrice = 1000000; MaxPrice = 4999999;
                    DisplayPriceRangeName = _localizer["PriceRangeAlias:From1MillionTo5Million"];
                    matched = true;
                }
                else if (incomingAliasLower == alias5Mto20M_en || incomingAliasLower == alias5Mto20M_vi)
                {
                    MinPrice = 5000000; MaxPrice = 19999999;
                    DisplayPriceRangeName = _localizer["PriceRangeAlias:From5MillionTo20Million"];
                    matched = true;
                }
                else if (incomingAliasLower == aliasOver20M_en || incomingAliasLower == aliasOver20M_vi)
                {
                    MinPrice = 20000000; MaxPrice = decimal.MaxValue;
                    DisplayPriceRangeName = _localizer["PriceRangeAlias:Over20Million"];
                    matched = true;
                }
                else
                {
                    DisplayPriceRangeName = this.PriceRangeAlias;
                    if (!string.IsNullOrEmpty(this.PriceRangeAlias))
                    {
                        Logger.LogWarning($"Unknown PriceRangeAlias: {this.PriceRangeAlias}");
                    }
                }

                if (string.IsNullOrEmpty(Slug))
                {
                    return NotFound("Category slug is required.");
                }

                var category = await _categoryRepository.GetBySlugAsync(Slug);
                CategoryName = category.Name;
                var input = new GetProductsByPrice
                {
                    MinPrice = MinPrice,
                    MaxPrice = MaxPrice,
                    Filter = Filter,
                    Sorting = Sorting,
                    CategoryId = category.Id,
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                };
                var routeValues = new Dictionary<string, string>
                {
                    { "slug", Slug },
                    { "priceRangeAlias", PriceRangeAlias }
                };

                Products = await _productAppService.GetListByProductPrice(input);

                PagerModel = new PagerModel(Products.TotalCount, 3, CurrentPage, PageSize, Url.Page("./ProductByPrice", routeValues), Sorting);

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
