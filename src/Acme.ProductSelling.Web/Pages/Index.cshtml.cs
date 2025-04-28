using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Web.Pages;

public class IndexModel : ProductSellingPageModel
{
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1; // Trang hiện tại, mặc định là 1

    public PagedResultDto<ProductDto> ProductList { get; set; } // Giữ danh sách sản phẩm
    private readonly IStringLocalizer<ProductSellingResource> _localizer;
    private readonly IProductAppService _productAppService;
    private readonly ICategoryAppService _categoryAppService;
    private readonly ICategoryRepository _categoryRepository;

    public PagedResultDto<CategoryDto> CategoryList { get; set; } 
    public int PageSize = 30; 

    public IndexModel(IProductAppService productAppService, ICategoryAppService categoryAppService,
        IStringLocalizer<ProductSellingResource> localizer, ICategoryRepository categoryRepository)
    {
        _productAppService = productAppService;
        _localizer = localizer;
        _categoryRepository = categoryRepository;
    }

    public async Task OnGetAsync()
    {
        // Tạo input DTO cho GetListAsync
        var input = new PagedAndSortedResultRequestDto
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = "ProductName" // Sắp xếp theo tên (hoặc trường khác)
        };
        ViewData["Title"] = _localizer["Menu:Home"];
        ProductList = await _productAppService.GetListAsync(input);

        var categoryLookup = await _categoryRepository.GetListAsync();

        // Map the List<Category> to PagedResultDto<CategoryDto>
        CategoryList = new PagedResultDto<CategoryDto>
        {
            Items = categoryLookup.Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            }).ToList(),
            TotalCount = categoryLookup.Count
        };
    }

}
