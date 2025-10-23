using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Components.ChatWidget
{
    public class ChatWidgetViewComponent : AbpViewComponent
    {
        public IViewComponentResult Invoke(bool isAdmin = false)
        {
            return View(new ChatWidgetViewModel
            {
                IsAdmin = isAdmin
            });
        }
    }

    public class ChatWidgetViewModel
    {
        public bool IsAdmin { get; set; }
    }
}
