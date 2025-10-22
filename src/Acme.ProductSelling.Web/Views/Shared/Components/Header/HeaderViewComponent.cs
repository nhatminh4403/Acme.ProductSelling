using Microsoft.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Views.Shared.Components.Header
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Views/Shared/Components/Header/Default.cshtml");
        }
    }
}
