using Acme.ProductSelling.Categories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Components.CategoriesMenu
{
    public class CategoriesMenuViewComponent :AbpViewComponent
    {
        private readonly ICategoryAppService _categoryAppService;

        public CategoriesMenuViewComponent(ICategoryAppService categoryAppService)
        {
            _categoryAppService = categoryAppService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {

            var categoryLookup = await _categoryAppService.GetCategoryLookupAsync();

            return View(categoryLookup.Items); 
        }

    }
}
