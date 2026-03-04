using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Admin.Shared.Components.Chatbot.Bubble
{
    public class ChatbotAdminBubbleViewComponent : AbpViewComponent
    {
        public ChatbotAdminBubbleViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            return View("/Pages/Admin/Shared/Components/Chatbot/Bubble/Default.cshtml");
        }
    }
}
