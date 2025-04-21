using Acme.ProductSelling.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages;

public abstract class ProductSellingPageModel : AbpPageModel
{
    protected ProductSellingPageModel()
    {
        LocalizationResourceType = typeof(ProductSellingResource);
    }
}
