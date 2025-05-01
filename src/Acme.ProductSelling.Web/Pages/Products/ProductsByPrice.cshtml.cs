using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Products
{

    public class ProductByPriceModel : AbpPageModel
    {
        private readonly IProductAppService _productAppService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        public ProductByPriceModel(IProductAppService productAppService, ICategoryRepository categoryRepository, IStringLocalizer<ProductSellingResource> localizer)
        {
            _productAppService = productAppService;
            _categoryRepository = categoryRepository;
            _localizer = localizer;
        }
        public PagedResultDto<ProductDto> Products { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string Filter { get; set; }
        public string Sorting { get; set; } = "ProductName";
        [BindProperty(SupportsGet = true)]
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int PageSize = 6;
        public int CurrentPage { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public string PriceRangeAlias { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Console.WriteLine($"PriceRangeAlias: {PriceRangeAlias}");
                var aliasUnder1M = _localizer["PriceRangeAlias:Under1Million"].Value;
                Console.WriteLine(aliasUnder1M);
                var alias1Mto5M = _localizer["PriceRangeAlias:From1MillionTo5Million"].Value;
                Console.WriteLine(alias1Mto5M);
                var alias5Mto20M = _localizer["PriceRangeAlias:From5MillionTo20Million"].Value;
                Console.WriteLine(alias5Mto20M);
                var aliasOver20M = _localizer["PriceRangeAlias:Over20Million"].Value;
                Console.WriteLine(aliasOver20M);

                switch (this.PriceRangeAlias?.ToLowerInvariant())
                {
                    case var alias when alias == aliasUnder1M.ToLowerInvariant(): // So sánh với giá trị từ L
                        MinPrice = 0;
                        MaxPrice = 999999;
                        break;
                    case var alias when alias == alias1Mto5M.ToLowerInvariant():
                        MinPrice = 1000000;
                        MaxPrice = 4999999;
                        break;
                    case var alias when alias == alias5Mto20M.ToLowerInvariant():
                        MinPrice = 5000000;
                        MaxPrice = 19999999;
                        break;
                    case var alias when alias == aliasOver20M.ToLowerInvariant():
                        MinPrice = 20000000;
                        MaxPrice = 999999999999;
                        break;
                    default:
                        // Không khớp alias nào
                        break;
                }
                var category = await _categoryRepository.GetAsync(CategoryId);
                CategoryName = category.Name;
                var input = new GetProductsByPrice
                {
                    MinPrice = MinPrice,
                    MaxPrice = MaxPrice,
                    Filter = Filter,
                    Sorting = Sorting,
                    CategoryId = this.CategoryId,
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                };
                Products = await _productAppService.GetListByProductPrice(input);
                return Page();
            }
            catch (Volo.Abp.Domain.Entities.EntityNotFoundException)
            {
                //Xử lý nếu CategoryId không hợp lệ
                return NotFound();
            }
        }
    }
}
