using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Admin.Products
{
    [Authorize(ProductSellingPermissions.Products.Default)]
    public class ProductDetailModel : PageModel
    {
        private readonly IProductAppService _productAppService;

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }
        public ProductDto Product { get; set; }
        public ProductDetailModel(IProductAppService productAppService)
        {
            _productAppService = productAppService;
        }
        public async Task OnGetAsync()
        {
            var product = await _productAppService.GetAsync(Id);
            Product = product;
        }
    }
}
