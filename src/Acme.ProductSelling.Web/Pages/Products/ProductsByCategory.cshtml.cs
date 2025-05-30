using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Products
{
    [AllowAnonymous]
    public class ProductsByCategoryModel : AbpPageModel
    {
        private readonly IProductAppService _productAppService;
        private readonly ICategoryAppService _categoryAppService;
        private readonly ICategoryRepository _categoryRepository;
        // Nhận CategoryId từ route
        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; }

        // Nhận tham số phân trang từ query string
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        // Giữ kết quả sản phẩm
        public PagedResultDto<ProductDto> ProductList { get; set; }
        public PagerModel PagerModel { get; set; }

        // (Optional) Giữ tên Category để hiển thị
        public string CategoryName { get; set; }
        public int PageSize = 12;
        public ProductsByCategoryModel(IProductAppService productAppService,
            ICategoryAppService categoryAppService, ICategoryRepository categoryRepository)
        {
            _productAppService = productAppService;
            _categoryAppService = categoryAppService;
            ProductList = new PagedResultDto<ProductDto>();
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var category = await _categoryRepository.GetBySlugAsync(Slug);
            try
            {

                CategoryName = category.Name;
                ViewData["Title"] = CategoryName; // Đặt tiêu đề trang
            }
            catch (Volo.Abp.Domain.Entities.EntityNotFoundException)
            {
                //Xử lý nếu CategoryId không hợp lệ
                return NotFound();
            }

            // Tạo input cho service
            var input = new GetProductsByCategoryInput
            {
                CategoryId = category.Id,
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = "ProductName"
            };

            // Gọi service để lấy danh sách sản phẩm theo category
            ProductList = await _productAppService.GetListByCategoryAsync(input);
            PagerModel = new PagerModel(ProductList.TotalCount, 3, CurrentPage, PageSize, "/");
            return Page();
        }
    }
}
