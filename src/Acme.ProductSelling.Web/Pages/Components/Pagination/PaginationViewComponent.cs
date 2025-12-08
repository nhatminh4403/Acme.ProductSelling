using Acme.ProductSelling.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Components.Pagination
{
    public class PaginationViewComponent : AbpViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(PaginationViewModel model)
        {
            if (model == null)
            {
                model = new PaginationViewModel(1, 0, 10, "/");
            }

            return View(model);
        }
    }
}
