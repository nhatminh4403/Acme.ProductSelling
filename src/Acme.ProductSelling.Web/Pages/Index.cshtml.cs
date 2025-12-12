using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Web.Pages;

public class IndexModel : ProductSellingPageModel
{
    private readonly IStringLocalizer<ProductSellingResource> _localizer;
    private readonly IProductAppService _productAppService;

    public List<FeaturedCategoryProductsDto> FeaturedProductCarousels { get; set; }
    private readonly IDistributedCache _cache;
    public IndexModel(IProductAppService productAppService,
                      IStringLocalizer<ProductSellingResource> localizer,
                      IDistributedCache cache)
    {
        _productAppService = productAppService;
        _localizer = localizer;
        _cache = cache;
    }

    public async Task OnGetAsync()
    {
        //var cacheKey = "home:featured-carousels";
        //FeaturedProductCarousels = await _cache.GetOrCreateAsync(
        //    cacheKey,
        //    async () => await _productAppService.GetFeaturedProductCarouselsAsync(),
        //    TimeSpan.FromHours(1)
        //);
        FeaturedProductCarousels = await _productAppService.GetFeaturedProductCarouselsAsync();
    }
}
