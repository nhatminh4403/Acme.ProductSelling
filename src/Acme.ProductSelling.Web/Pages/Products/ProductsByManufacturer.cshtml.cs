using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Products
{
    public class ProductsByManufacturerModel : AbpPageModel
    {
        private readonly IProductAppService _productAppService;
        public PagedResultDto<ProductDto> Products { get; set; }

        public string Filter { get; set; }
        public string Sorting { get; set; } = "ProductName";
        public int PageSize = 6;
        public int CurrentPage { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public Guid ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ProductsByManufacturerModel(IProductAppService productAppService)
        {
            _productAppService = productAppService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var result = await _productAppService.GetProductByManufacturer(new GetProductsByManufacturer
                {
                    ManufacturerId = this.ManufacturerId,
                    CategoryId = this.CategoryId,
                    CategoryName = this.CategoryName,
                    ManufacturerName = this.ManufacturerName,
                    Filter = this.Filter,
                    Sorting = this.Sorting,
                    MaxResultCount = 6,
                    SkipCount = (CurrentPage - 1) * 6
                });
                Products = result;
                return Page();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
