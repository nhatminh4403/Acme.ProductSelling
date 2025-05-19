using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Themes.LeptonXLite.Components.Brand;
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
