using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Components.Home.CategoryList
{
    public class CategoryListViewComponent : AbpViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryListViewComponent(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categoryLookup = await _categoryRepository.GetListAsync();

            var categoryList = new PagedResultDto<CategoryDto>
            {
                Items = ObjectMapper.Map<List<Category>, List<CategoryDto>>(categoryLookup),
                TotalCount = categoryLookup.Count
            };
            return View("/Pages/Components/Home/CategoryList/Default.cshtml", categoryList);
        }
    }
}
