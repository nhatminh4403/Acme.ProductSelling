using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Acme.ProductSelling.Web.Pages;

public class IndexModel : ProductSellingPageModel
{
    private readonly IStringLocalizer<ProductSellingResource> _localizer;
    private readonly IProductLookupAppService _productLookupAppService;

    public List<FeaturedCategoryProductsDto> FeaturedProductCarousels { get; set; }
    public IndexModel(IProductLookupAppService productLookupAppService,
                      IStringLocalizer<ProductSellingResource> localizer)
    {
        _productLookupAppService = productLookupAppService;
        _localizer = localizer;
    }

    public async Task OnGetAsync()
    {
        //var cacheKey = "home:featured-carousels";
        //FeaturedProductCarousels = await _cache.GetOrCreateAsync(
        //    cacheKey,
        //    async () => await _productAppService.GetFeaturedProductCarouselsAsync(),
        //    TimeSpan.FromHours(1)
        //);
        FeaturedProductCarousels = await _productLookupAppService.GetFeaturedProductCarouselsAsync();
    }
}
