using Microsoft.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Views.Shared.Components.Modal
{
    public class ModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Views/Shared/Components/Modal/Default.cshtml");
        }
    }
}
