using Acme.ProductSelling.Categories.Services;
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
            var groupedCategories = await _categoryAppService.GetGroupedCategoriesAsync();


            return View("/Pages/Components/Home/HeroSection/Default.cshtml", groupedCategories);
        }
    }
}
