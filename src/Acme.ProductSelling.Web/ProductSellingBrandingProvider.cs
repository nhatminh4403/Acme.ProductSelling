using Acme.ProductSelling.Localization;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

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
    public override string LogoUrl => "/images/monitor.png";
    public override string LogoReverseUrl => "/admin";
}
