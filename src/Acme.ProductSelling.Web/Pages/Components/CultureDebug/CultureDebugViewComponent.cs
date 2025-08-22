using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Components.CultureDebug
{
    public class CultureDebugViewComponent : AbpViewComponent
    {
        public CultureDebugViewComponent()
        {
        }
        public virtual async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new CultureDebugModel
            {
                CurrentCulture = CultureInfo.CurrentCulture.Name,
                CurrentUICulture = CultureInfo.CurrentUICulture.Name,
                RequestPath = HttpContext.Request.Path.Value,
                CultureFromCookie = HttpContext.Request.Cookies.TryGetValue("culture", out var cookieCulture) ? cookieCulture : "Not found"
            };

            return View("~/Pages/Components/CultureDebug/Default.cshtml", model);
        }
    }
    public class CultureDebugModel
    {
        public string CurrentCulture { get; set; }
        public string CurrentUICulture { get; set; }
        public string RequestPath { get; set; }
        public string CultureFromCookie { get; set; }
    }
}
