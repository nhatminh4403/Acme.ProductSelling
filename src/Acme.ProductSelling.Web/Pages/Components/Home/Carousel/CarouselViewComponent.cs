using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Components.Home.Carousel
{
    public class CarouselViewComponent : AbpViewComponent
    {
        public CarouselViewComponent()
        {
        }
        public IViewComponentResult Invoke()
        {
            return View("/Pages/Components/Home/Carousel/Default.cshtml");
        }
    }
}
