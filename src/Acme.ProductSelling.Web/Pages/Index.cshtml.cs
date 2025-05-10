using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Web.Pages;

public class IndexModel : ProductSellingPageModel
{
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    public int PageSize = 30;

    private readonly IStringLocalizer<ProductSellingResource> _localizer;
    private readonly IProductAppService _productAppService;
    private readonly ICategoryAppService _categoryAppService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IManufacturerAppService _manufacturerAppService;
    private readonly IManufacturerRepository _manufacturerRepository;
    public PagedResultDto<ProductDto> ProductList { get; set; } 
    
    public PagedResultDto<CategoryDto> CategoryList { get; set; }
    public PagedResultDto<ManufacturerDto> ManufacturerList { get; set; }
    public ListResultDto<CategoryWithManufacturersDto> BrandsWithAssociatedCategory { get; set; }

    public IndexModel(IProductAppService productAppService, ICategoryAppService categoryAppService,
        IStringLocalizer<ProductSellingResource> localizer, ICategoryRepository categoryRepository,
        IManufacturerAppService manufacturerAppService, IManufacturerRepository manufacturerRepository)
    {
        _productAppService = productAppService;
        _localizer = localizer;
        _categoryRepository = categoryRepository;
        _categoryAppService = categoryAppService;
        _manufacturerAppService = manufacturerAppService;
        _manufacturerRepository = manufacturerRepository;
     
    }

    public async Task OnGetAsync()
    {
        var input = new PagedAndSortedResultRequestDto
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = "ProductName" // Sắp xếp theo tên (hoặc trường khác)
        };
        ViewData["Title"] = _localizer["Menu:Home"];
        ProductList = await _productAppService.GetListAsync(input);

        var categoryLookup = await _categoryRepository.GetListAsync();

        CategoryList = new PagedResultDto<CategoryDto>
        {
            Items = ObjectMapper.Map<List<Category>, List<CategoryDto>>(categoryLookup),
            TotalCount = categoryLookup.Count
        };

        var manufacturerLookup = await _manufacturerRepository.GetListAsync();

        ManufacturerList = new PagedResultDto<ManufacturerDto>
        {
            Items = ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(manufacturerLookup),
            TotalCount = categoryLookup.Count
        };
        var brandsWithAssociatedCategory = await _categoryAppService.GetListWithManufacturersAsync();
        BrandsWithAssociatedCategory = new ListResultDto<CategoryWithManufacturersDto>
        {
            Items = brandsWithAssociatedCategory.Items
        };
    }
}
