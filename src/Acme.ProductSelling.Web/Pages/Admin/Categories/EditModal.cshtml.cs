using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Admin.Categories
{
    [Authorize(ProductSellingPermissions.Categories.Edit)]
    public class EditModalModel : AdminPageModelBase
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }
        [BindProperty]
        public CreateUpdateCategoryDto Category { get; set; }

        private readonly ICategoryAppService _categoryAppService;

        public EditModalModel(ICategoryAppService categoryAppService)
        {
            _categoryAppService = categoryAppService;
        }
        public async Task OnGet()
        {
            var categoryDto = await _categoryAppService.GetAsync(Id);
            Category = ObjectMapper.Map<CategoryDto, CreateUpdateCategoryDto>(categoryDto);
        }
        // Hàm này được gọi khi submit form (POST request)
        public async Task<IActionResult> OnPostAsync()
        {
            await _categoryAppService.UpdateAsync(Id, Category);
            return NoContent();
        }
    }
}
