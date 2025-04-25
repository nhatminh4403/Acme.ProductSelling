using Acme.ProductSelling.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Categories
{
    public class CreateModalModel : AbpPageModel
    {
        [BindProperty]
        public CreateUpdateCategoryDto Category { get; set; }

        private readonly ICategoryAppService _categoryAppService; // Inject service

        public CreateModalModel(ICategoryAppService categoryAppService)
        {
            _categoryAppService = categoryAppService; 
            Category = new CreateUpdateCategoryDto();
        }
        public void OnGet()
        {
            Category = new CreateUpdateCategoryDto();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            // Hàm này được gọi khi form trong modal được submit (POST)
            // Kiểm tra quyền tạo ở đây (hoặc để Application Service tự kiểm tra qua [Authorize])
            // await AuthorizationService.CheckAsync(MyProjectNamePermissions.Categories.Create);

            await _categoryAppService.CreateAsync(Category); // Gọi service để tạo Category
            return NoContent(); // Trả về 204 No Content để báo hiệu thành công cho abp.ModalManager
        }
    }
}
