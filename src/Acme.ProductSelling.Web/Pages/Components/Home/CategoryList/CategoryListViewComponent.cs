using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Components.Home.CategoryList
{
    public class CategoryListViewComponent : AbpViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDistributedCache _cache;
        private readonly IMemoryCache _memoryCache;
        public CategoryListViewComponent(ICategoryRepository categoryRepository, IDistributedCache cache, IMemoryCache memoryCache)
        {
            _categoryRepository = categoryRepository;
            _cache = cache;
            _memoryCache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categoryLookup = await _categoryRepository.GetListAsync();
            var cacheKey = "categories:all";

            var categoryList = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);
                var categories = await _categoryRepository.GetListAsync();
                return new PagedResultDto<CategoryDto>
                {
                    Items = ObjectMapper.Map<List<Category>, List<CategoryDto>>(categories),
                    TotalCount = categories.Count
                };
            });
            //var categoryList = new PagedResultDto<CategoryDto>
            //{
            //    Items = ObjectMapper.Map<List<Category>, List<CategoryDto>>(categoryLookup),
            //    TotalCount = categoryLookup.Count
            //};
            return View("/Pages/Components/Home/CategoryList/Default.cshtml", categoryList);
        }
    }
}
