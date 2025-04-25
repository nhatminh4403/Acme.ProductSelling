using Acme.ProductSelling.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Categories
{
    public class EditModalModel : AbpPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)] // Bind Id từ cả GET (khi mở modal) và POST (khi submit)
        public Guid Id { get; set; }

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
            // Map CategoryDto sang CreateUpdateCategoryDto để đổ vào form
            Category = ObjectMapper.Map<CategoryDto, CreateUpdateCategoryDto>(categoryDto);
        }
        // Hàm này được gọi khi submit form (POST request)
        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra quyền Edit ở đây hoặc để Application Service tự kiểm tra
            // await AuthorizationService.CheckAsync(MyProjectNamePermissions.Categories.Edit);

            await _categoryAppService.UpdateAsync(Id, Category); // Gọi service để cập nhật
            return NoContent(); // Báo hiệu thành công
        }
    }
}
