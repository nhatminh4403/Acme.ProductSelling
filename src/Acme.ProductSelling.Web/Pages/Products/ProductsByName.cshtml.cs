using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Products
{
    [AllowAnonymous]
    public class ProductsByNameModel : AbpPageModel
    {
        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "name")]
        public string Name { get; set; }
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public PagedResultDto<ProductDto> ProductList { get; set; }
        public int PageSize = 12;
        private readonly IProductAppService _productAppService;
        public ProductsByNameModel(IProductAppService productAppService)
        {
            _productAppService = productAppService;
            ProductList = new PagedResultDto<ProductDto>();
        }


        public async Task<IActionResult> OnGet()
        {
            var input = new GetProductByName
            {
                Filter = Name,
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = "ProductName",
            };
            ProductList = await _productAppService.GetProductByName(input);

            return Page();
        }
    }
}
