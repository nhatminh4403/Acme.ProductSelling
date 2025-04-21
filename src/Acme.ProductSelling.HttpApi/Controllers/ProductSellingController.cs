using Acme.ProductSelling.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class ProductSellingController : AbpControllerBase
{
    protected ProductSellingController()
    {
        LocalizationResource = typeof(ProductSellingResource);
    }
}
