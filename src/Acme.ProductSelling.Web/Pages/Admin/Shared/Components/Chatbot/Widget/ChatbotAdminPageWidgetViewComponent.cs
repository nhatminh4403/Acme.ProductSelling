using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Admin.Shared.Components.Chatbot.Widget
{
    public class ChatbotAdminPageWidgetViewComponent : AbpViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Pages/Admin/Shared/Components/Chatbot/Widget/Default.cshtml");
        }
    }
}
