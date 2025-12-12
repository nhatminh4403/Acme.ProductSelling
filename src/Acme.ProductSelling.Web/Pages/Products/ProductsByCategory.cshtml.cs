using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;

namespace Acme.ProductSelling.Web.Pages.Products
{
    [AllowAnonymous]
    public class ProductsByCategoryModel : ProductSellingPageModel
    {

        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductLookupAppService _productLookupAppService;
        // Nhận CategoryId từ route
        [BindProperty(SupportsGet = true)]
        public string Slug { get; set; }
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public PagedResultDto<ProductDto> ProductList { get; set; }
        public PagerModel PagerModel { get; set; }
        public string CategoryName { get; set; }
        public int PageSize = 12;
        public ProductsByCategoryModel(
            ICategoryRepository categoryRepository,
            IProductLookupAppService productLookupAppService)
        {

            ProductList = new PagedResultDto<ProductDto>();
            _categoryRepository = categoryRepository;
            _productLookupAppService = productLookupAppService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var category = await _categoryRepository.GetBySlugAsync(Slug);
            try
            {

                CategoryName = category.Name;
                ViewData["Title"] = CategoryName;
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

            ProductList = await _productLookupAppService.GetListByCategoryAsync(input);
            PagerModel = new PagerModel(ProductList.TotalCount, 3, CurrentPage, PageSize, "/");
            return Page();
        }
    }
}
