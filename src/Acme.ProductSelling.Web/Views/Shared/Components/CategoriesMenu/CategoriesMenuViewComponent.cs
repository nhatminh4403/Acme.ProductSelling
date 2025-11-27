using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Views.Shared.Components.CategoriesMenu
{
    public class CategoriesMenuViewComponent : AbpViewComponent
    {
        private readonly ICategoryAppService _categoryAppService;

        public CategoriesMenuViewComponent(ICategoryAppService categoryAppService)
        {
            _categoryAppService = categoryAppService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var brandsWithAssociatedCategory = await _categoryAppService.GetCategoriesWithManufacturersAsync();

            return View(brandsWithAssociatedCategory.Items);
        }

    }
}
