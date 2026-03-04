using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Views.Shared.Components.Chatbot
{
    public class ChatWidgetViewComponent : AbpViewComponent
    {

        public IViewComponentResult Invoke()
        {
            return View("~/Views/Shared/Components/Chatbot/Default.cshtml");
        }
    }
}
