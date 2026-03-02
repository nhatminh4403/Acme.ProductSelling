using Acme.ProductSelling.Web.Models;
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
            var model = new CarouselViewModel
            {
                IsSidebarDataEmpty = false // Set this based on your actual data
            };
            return View("/Pages/Components/Home/Carousel/Default.cshtml", model);
        }
    }
}
