using Acme.ProductSelling.Categories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Components.Home.HeroSection
{
    public class HeroSectionViewComponent : AbpViewComponent
    {
        private readonly ICategoryAppService _categoryAppService;

        public HeroSectionViewComponent(ICategoryAppService categoryAppService)
        {
            _categoryAppService = categoryAppService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var brandsWithAssociatedCategory = await _categoryAppService.GetListWithManufacturersAsync();


            return View("/Pages/Components/Home/HeroSection/Default.cshtml", brandsWithAssociatedCategory);
        }
    }
}
