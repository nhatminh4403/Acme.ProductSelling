using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Web.Pages.Products
{
    public class ProductDetailModel : AbpPageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        // Thuộc tính này đã chứa đủ thông tin, bao gồm cả các spec con và CategorySpecificationType
        public ProductDto Product { get; private set; }

        private readonly IProductAppService _productAppService;

        public ProductDetailModel(IProductAppService productAppService)
        {
            _productAppService = productAppService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Product = await _productAppService.GetAsync(Id);
                return Page();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
