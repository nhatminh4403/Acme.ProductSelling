using Acme.ProductSelling.Localization;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling;

/* Inherit your application services from this class.
 */
public abstract class ProductSellingAppService : ApplicationService
{
    protected ProductSellingAppService()
    {
        LocalizationResource = typeof(ProductSellingResource);
    }
}
