using Microsoft.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Views.Shared.Components.Footer
{
    public class FooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Views/Shared/Components/Footer/Default.cshtml");
        }

    }
}
