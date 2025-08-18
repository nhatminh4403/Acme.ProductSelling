using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Admin.Products
{
    [Authorize(ProductSellingPermissions.Products.Edit)]

    public class EditModalModel : AbpPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public CreateUpdateProductDto Product { get; set; }

        public List<SelectListItem> Categories { get; set; }

        private readonly IProductAppService _productAppService;
        private readonly ICategoryAppService _categoryAppService;
        public EditModalModel(
           IProductAppService productAppService,
           ICategoryAppService categoryAppService)
        {
            _productAppService = productAppService;
            _categoryAppService = categoryAppService;
        }
        public async Task OnGetAsync()
        {
            // Lấy dữ liệu Product hiện tại
            var productDto = await _productAppService.GetAsync(Id);
            Product = ObjectMapper.Map<ProductDto, CreateUpdateProductDto>(productDto);

            // Load danh sách Categories
            var categoryLookup = await _categoryAppService.GetCategoryLookupAsync();
            Categories = categoryLookup.Items
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Cập nhật sản phẩm
            await _productAppService.UpdateAsync(Id, Product);
            return NoContent();
        }
    }
}
