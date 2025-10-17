using Microsoft.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Admin.Shared.Components.Header
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("/Pages/Admin/Shared/Components/Header/Default.cshtml");
        }
    }
}
