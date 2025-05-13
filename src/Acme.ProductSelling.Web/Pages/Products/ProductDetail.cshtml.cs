using Acme.ProductSelling.Products;
using Acme.ProductSelling.Utils;
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
        //[BindProperty(SupportsGet = true)]
        //public Guid Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; }

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
                if (string.IsNullOrWhiteSpace(this.Slug))
                {
                    return NotFound();
                }
                //Product = await _productAppService.GetAsync(Id);
                Product = await _productAppService.GetProductBySlug(Slug);
                return Page();
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
