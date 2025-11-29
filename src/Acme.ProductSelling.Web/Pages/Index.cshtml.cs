using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Web.Pages;

public class IndexModel : ProductSellingPageModel
{
    private readonly IStringLocalizer<ProductSellingResource> _localizer;
    private readonly IProductAppService _productAppService;
    private int CurrentPage { get; set; } = 1;
    private int PageSize { get; set; } = 24;
    public PagedResultDto<ProductDto> ProductList { get; set; }
    public PagedResultDto<ManufacturerDto> ManufacturerList { get; set; }
    public List<FeaturedCategoryProductsDto> FeaturedProductCarousels { get; set; }

    public IndexModel(IProductAppService productAppService,
                      IStringLocalizer<ProductSellingResource> localizer)
    {
        _productAppService = productAppService;
        _localizer = localizer;
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

        FeaturedProductCarousels = await _productAppService.GetFeaturedProductCarouselsAsync();
    }
}
