using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
namespace Acme.ProductSelling.Web.Themes.LeptonXLite.Components.Brand
{
    public class MainNavbarBrandViewComponent : AbpViewComponent
    {
        public virtual IViewComponentResult Invoke()
        {
            return View("~/Themes/LeptonXLite/Components/Brand/Default.cshtml");
        }
    }

}
