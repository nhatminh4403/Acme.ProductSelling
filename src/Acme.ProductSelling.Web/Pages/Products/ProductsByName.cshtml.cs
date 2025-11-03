using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;

namespace Acme.ProductSelling.Web.Pages.Products
{
    [AllowAnonymous]
    public class ProductsByNameModel : ProductSellingPageModel
    {
        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "name")]
        public string Name { get; set; }
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public PagedResultDto<ProductDto> ProductList { get; set; }
        public int PageSize = 12;
        private readonly IProductLookupAppService _productLookupAppService;
        public PagerModel PagerModel { get; set; }
        //PagerModel = new PagerModel(ProductList.TotalCount, 3, CurrentPage, PageSize, "/");
        public ProductsByNameModel(IProductLookupAppService productLookupAppService)
        {
            ProductList = new PagedResultDto<ProductDto>();
            _productLookupAppService = productLookupAppService;
        }


        public async Task<IActionResult> OnGet()
        {
            var input = new GetProductByNameDto
            {
                Filter = Name,
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = "ProductName",
            };
            ProductList = await _productLookupAppService.GetProductsByName(input);
            PagerModel = new PagerModel(ProductList.TotalCount, 3, CurrentPage, PageSize, "/");

            return Page();
        }
    }
}
