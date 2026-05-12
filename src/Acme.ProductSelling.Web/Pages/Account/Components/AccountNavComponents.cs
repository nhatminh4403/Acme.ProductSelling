using Acme.ProductSelling.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Account.Components
{
    public class AccountNavComponents : AbpViewComponent
    {
        public IViewComponentResult Invoke(AccountNavActive accountNavActive)
        {
            return View("~/Pages/Account/Components/Default.cshtml",accountNavActive);
        }
    }
}
