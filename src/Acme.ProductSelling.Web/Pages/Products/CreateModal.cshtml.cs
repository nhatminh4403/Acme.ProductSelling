using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Products
{
    public class CreateModalModel : AbpPageModel
    {
        [BindProperty]
        public CreateUpdateProductDto Product { get; set; }

        // Danh sách Category để đổ vào dropdown
        public List<SelectListItem> Categories { get; set; }

        private readonly IProductAppService _productAppService;
        private readonly ICategoryAppService _categoryAppService; // Inject Category service
        public CreateModalModel(
           IProductAppService productAppService,
           ICategoryAppService categoryAppService)
        {
            _productAppService = productAppService;
            _categoryAppService = categoryAppService;
            Product = new CreateUpdateProductDto(); // Khởi tạo
        }
        public async Task OnGet()
        {
            Product = new CreateUpdateProductDto(); // Đảm bảo là object mới

            var categoryLookup = await _categoryAppService.GetCategoryLookupAsync();
            Categories = categoryLookup.Items
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToList();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            // Quyền Create được kiểm tra bởi ProductAppService
            await _productAppService.CreateAsync(Product);
            return NoContent();
        }
    }
}
