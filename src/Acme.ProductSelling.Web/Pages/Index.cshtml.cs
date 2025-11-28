using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Categories.Services;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;
namespace Acme.ProductSelling.Web.Pages;

public class IndexModel : ProductSellingPageModel
{
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 24;

    private readonly IStringLocalizer<ProductSellingResource> _localizer;
    private readonly IProductAppService _productAppService;
    private readonly IProductRepository _productRepository;
    private readonly ICategoryAppService _categoryAppService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IManufacturerAppService _manufacturerAppService;
    private readonly IManufacturerRepository _manufacturerRepository;
    public PagedResultDto<ProductDto> ProductList { get; set; }

    public PagedResultDto<CategoryDto> CategoryList { get; set; }
    public PagedResultDto<ManufacturerDto> ManufacturerList { get; set; }

    public PagerModel PagerModel { get; set; }

    public List<FeaturedCategoryProductsDto> FeaturedProductCarousels { get; set; }

    private readonly ProductToProductDtoMapper ProductMapper;
    private readonly CategoryToCategoryDtoMapper CategoryMapper;
    public IndexModel(IProductAppService productAppService,
                      ICategoryAppService categoryAppService,
                      IStringLocalizer<ProductSellingResource> localizer,
                      ICategoryRepository categoryRepository,
                      IManufacturerAppService manufacturerAppService,
                      IManufacturerRepository manufacturerRepository,
                      IProductRepository productRepository,
                      ProductToProductDtoMapper productMapper,
                      CategoryToCategoryDtoMapper categoryMapper)
    {
        _productAppService = productAppService;
        _localizer = localizer;
        _categoryRepository = categoryRepository;
        _categoryAppService = categoryAppService;
        _manufacturerAppService = manufacturerAppService;
        _manufacturerRepository = manufacturerRepository;
        _productRepository = productRepository;
        ProductMapper = productMapper;
        CategoryMapper = categoryMapper;
    }

    public async Task OnGetAsync()
    {
        var input = new PagedAndSortedResultRequestDto
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = "ProductName",

        };
        ViewData["Title"] = _localizer["Menu:Home"];

        var productList = await _productAppService.GetListAsync(input);

        ProductList = new PagedResultDto<ProductDto>
        {
            Items = productList.Items,
            TotalCount = productList.TotalCount
        };


        var categoryLookup = await _categoryRepository.GetListAsync();

        CategoryList = new PagedResultDto<CategoryDto>
        {
            //Items = ObjectMapper.Map<List<Category>, List<CategoryDto>>(categoryLookup),
            Items = categoryLookup.Select(c => CategoryMapper.Map(c)).ToList(),
            TotalCount = categoryLookup.Count
        };


        FeaturedProductCarousels = new List<FeaturedCategoryProductsDto>();
        int numberOfFeaturedCategories = 4;
        int productsPerCarousel = 10;
        var featuredSpecTypes = new[]
        {
            SpecificationType.Mouse,
            SpecificationType.Laptop,
            SpecificationType.Monitor,
            SpecificationType.Keyboard
        };

        var categoriesToFeature = CategoryList.Items
            .Where(c => featuredSpecTypes.Contains(c.SpecificationType))
            .Take(numberOfFeaturedCategories)
            .ToList();

        if (!categoriesToFeature.Any())
            return;
        var categoryIds = categoriesToFeature.Select(c => c.Id).ToList();
        var allProducts = productList.Items.Where(p => categoryIds.Contains(p.CategoryId)).ToList();

        foreach (var category in categoriesToFeature)
        {
            var productsInCategory = allProducts
                .Where(p => p.CategoryId == category.Id)
                .ToList();

            if (!productsInCategory.Any())
                continue;

            var randomProducts = productsInCategory.Shuffle().Take(productsPerCarousel).ToList();

            FeaturedProductCarousels.Add(new FeaturedCategoryProductsDto
            {
                Category = category,
                //Products = ObjectMapper.Map<List<Product>, List<ProductDto>>(randomProducts)
                Products = randomProducts
            });
        }

        PagerModel = new PagerModel(ProductList.TotalCount, 3, CurrentPage, PageSize, "/");
    }

    public class FeaturedCategoryProductsDto
    {
        public CategoryDto Category { get; set; }
        public List<ProductDto> Products { get; set; }

        public FeaturedCategoryProductsDto()
        {
            Products = new List<ProductDto>();
        }
    }
}
