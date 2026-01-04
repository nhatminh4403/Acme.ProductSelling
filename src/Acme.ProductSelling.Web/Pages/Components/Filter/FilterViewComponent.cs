using Acme.ProductSelling.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Components.Filter
{
    public class FilterViewComponent : AbpViewComponent
    {
        public IViewComponentResult Invoke(FilterViewModel model)
        {
            return View("~/Pages/Components/Filter/Default.cshtml", model);
        }
    }
}
