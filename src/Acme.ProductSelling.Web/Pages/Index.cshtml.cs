using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Extensions;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    private readonly HttpClient _httpClient;
    public PagedResultDto<ProductDto> ProductList { get; set; }

    public PagedResultDto<CategoryDto> CategoryList { get; set; }
    public PagedResultDto<ManufacturerDto> ManufacturerList { get; set; }
    //public ListResultDto<CategoryWithManufacturersDto> BrandsWithAssociatedCategory { get; set; }

    public PagerModel PagerModel { get; set; }

    public List<FeaturedCategoryProductsDto> FeaturedProductCarousels { get; set; }
    public IndexModel(IProductAppService productAppService, ICategoryAppService categoryAppService,
        IStringLocalizer<ProductSellingResource> localizer, ICategoryRepository categoryRepository,
        IManufacturerAppService manufacturerAppService, IManufacturerRepository manufacturerRepository,
        IProductRepository productRepository)
    {
        _productAppService = productAppService;
        _localizer = localizer;
        _categoryRepository = categoryRepository;
        _categoryAppService = categoryAppService;
        _manufacturerAppService = manufacturerAppService;
        _manufacturerRepository = manufacturerRepository;
        _productRepository = productRepository;
        _httpClient = new HttpClient();
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




        var manufacturerLookup = await _manufacturerRepository.GetListAsync();

        ManufacturerList = new PagedResultDto<ManufacturerDto>
        {
            Items = ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(manufacturerLookup),
            TotalCount = manufacturerLookup.Count
        };
        //var brandsWithAssociatedCategory = await _categoryAppService.GetListWithManufacturersAsync();
        //BrandsWithAssociatedCategory = new ListResultDto<CategoryWithManufacturersDto>
        //{
        //    Items = brandsWithAssociatedCategory.Items
        //};
        var categoryLookup = await _categoryRepository.GetListAsync();

        CategoryList = new PagedResultDto<CategoryDto>
        {
            Items = ObjectMapper.Map<List<Category>, List<CategoryDto>>(categoryLookup),
            TotalCount = categoryLookup.Count
        };
        FeaturedProductCarousels = new List<FeaturedCategoryProductsDto>();
        int numberOfFeaturedCategories = 4; // Display carousels for 4 categories
        int productsPerCarousel = 8;
        var categoriesToFeature = CategoryList.Items.Take(numberOfFeaturedCategories).ToList();


        foreach (var category in categoriesToFeature)
        {
            var allProductsInCategory = await _productRepository.GetListAsync(p => p.CategoryId == category.Id);

            // 2. Use your extension method to shuffle the list, then take the desired number of products.
            var randomProducts = allProductsInCategory.Shuffle().Take(productsPerCarousel).ToList();

            if (randomProducts.Any())
            {
                FeaturedProductCarousels.Add(new FeaturedCategoryProductsDto
                {
                    Category = category,
                    Products = ObjectMapper.Map<List<Product>, List<ProductDto>>(randomProducts)
                });
            }
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
