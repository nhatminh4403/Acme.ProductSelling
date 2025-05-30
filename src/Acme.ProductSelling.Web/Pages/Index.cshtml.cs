using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
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
    public ListResultDto<CategoryWithManufacturersDto> BrandsWithAssociatedCategory { get; set; }

    public PagerModel PagerModel { get; set; }

    public List<FeaturedCategoryProductsDto> FeaturedProductCarousels { get; set; }
    public IndexModel(IProductAppService productAppService, ICategoryAppService categoryAppService,
        IStringLocalizer<ProductSellingResource> localizer, ICategoryRepository categoryRepository,
        IManufacturerAppService manufacturerAppService, IManufacturerRepository manufacturerRepository, IProductRepository productRepository)
    {
        _productAppService = productAppService;
        _localizer = localizer;
        _categoryRepository = categoryRepository;
        _categoryAppService = categoryAppService;
        _manufacturerAppService = manufacturerAppService;
        _manufacturerRepository = manufacturerRepository;
        _productRepository = productRepository;
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
            Items = ObjectMapper.Map<List<Category>, List<CategoryDto>>(categoryLookup),
            TotalCount = categoryLookup.Count
        };

        var manufacturerLookup = await _manufacturerRepository.GetListAsync();

        ManufacturerList = new PagedResultDto<ManufacturerDto>
        {
            Items = ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(manufacturerLookup),
            TotalCount = manufacturerLookup.Count
        };
        var brandsWithAssociatedCategory = await _categoryAppService.GetListWithManufacturersAsync();
        BrandsWithAssociatedCategory = new ListResultDto<CategoryWithManufacturersDto>
        {
            Items = brandsWithAssociatedCategory.Items
        };

        FeaturedProductCarousels = new List<FeaturedCategoryProductsDto>();
        int numberOfFeaturedCategories = 4; // Display carousels for 4 categories
        int productsPerCarousel = 8;
        var categoriesToFeature = CategoryList.Items.Take(numberOfFeaturedCategories).ToList();


        foreach (var category in categoriesToFeature)
        {
            long totalProductsInCategory = await _productRepository.GetCountAsync();
            totalProductsInCategory = await _productRepository.GetListAsync(p => p.CategoryId == category.Id).ContinueWith(t => t.Result.Count);
            int skipCountForNewest = (int)Math.Max(0, totalProductsInCategory - productsPerCarousel);

            List<Product> productsFromRepo = await _productRepository.GetListAsync(
                skipCount: skipCountForNewest,
                maxResultCount: productsPerCarousel,
                sorting: "CreationTime",
                filter: category.Id.ToString()
            );

            List<Product> correctlySortedProducts = productsFromRepo.OrderByDescending(p => p.CreationTime).ToList();

            if (correctlySortedProducts.Any())
            {
                FeaturedProductCarousels.Add(new FeaturedCategoryProductsDto
                {
                    Category = category,
                    Products = ObjectMapper.Map<List<Product>, List<ProductDto>>(correctlySortedProducts)
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
