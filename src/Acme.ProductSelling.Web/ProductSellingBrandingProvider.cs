using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using Acme.ProductSelling.Localization;

namespace Acme.ProductSelling.Web;

[Dependency(ReplaceServices = true)]
public class ProductSellingBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<ProductSellingResource> _localizer;

    public ProductSellingBrandingProvider(IStringLocalizer<ProductSellingResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
