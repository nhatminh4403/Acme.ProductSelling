using Acme.ProductSelling.Categories.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Views.Shared.Components.CategorySidebar
{
    public class CategorySidebarViewComponent : AbpViewComponent
    {
        private readonly ICategoryAppService _categoryAppService;

        public CategorySidebarViewComponent(ICategoryAppService categoryAppService)
        {
            _categoryAppService = categoryAppService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var brandsWithAssociatedCategory = await _categoryAppService.GetCategoriesWithManufacturersAsync();
            var groupedCategories = await _categoryAppService.GetGroupedCategoriesAsync();
            return View(groupedCategories);
        }

    }
}
