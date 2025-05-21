using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Acme.ProductSelling.Manufacturers;

namespace Acme.ProductSelling.Categories
{
    public class CategoryAppService : CrudAppService<Category, CategoryDto,
        Guid, PagedAndSortedResultRequestDto, CreateUpdateCategoryDto>, ICategoryAppService
    {
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public CategoryAppService(
                 ICategoryRepository categoryRepository,
                 IRepository<Product, Guid> productRepository)
                 : base(categoryRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            GetPolicyName = ProductSellingPermissions.Categories.Default;
            CreatePolicyName = ProductSellingPermissions.Categories.Create;
            UpdatePolicyName = ProductSellingPermissions.Categories.Edit;
            DeletePolicyName = ProductSellingPermissions.Categories.Delete;
        }
        [AllowAnonymous]
        public async Task<ListResultDto<CategoryLookupDto>> GetCategoryLookupAsync()
        {
            var categories = await Repository.GetListAsync();
            return new ListResultDto<CategoryLookupDto>(
                ObjectMapper.Map<List<Category>, List<CategoryLookupDto>>(categories)
            );
        }
        public async Task<ListResultDto<CategoryWithManufacturersDto>> GetListWithManufacturersAsync()
        {
            var categories = await Repository.GetListAsync();
            var categoryWithManufacturersDtos = new List<CategoryWithManufacturersDto>();
            var manufacturers = (await _productRepository.GetQueryableAsync()).Include(p => p.Manufacturer);

            foreach (var category in categories)
            {
                var manufacturersInCategory = manufacturers
                    .Where(item => item.CategoryId == category.Id && item.Manufacturer != null)
                    .Select(item => item.Manufacturer).Distinct().OrderBy(item => item.Name);
                var manufacturersInCategoryList = await AsyncExecuter.ToListAsync(manufacturersInCategory);

                var categoryWithManufacturersDto = new CategoryWithManufacturersDto
                {
                    Id = category.Id,
                    CategoryName = category.Name,
                    ManufacturerCount = manufacturersInCategoryList.Count(),
                    CategoryUrlSlug = category.UrlSlug,
                    Manufacturers = ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(manufacturersInCategoryList)
                };

                categoryWithManufacturersDtos.Add(categoryWithManufacturersDto);
            }
            return new ListResultDto<CategoryWithManufacturersDto>(categoryWithManufacturersDtos);
        }

    }
}
