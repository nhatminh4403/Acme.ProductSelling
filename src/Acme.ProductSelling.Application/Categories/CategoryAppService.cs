using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Acme.ProductSelling.Categories
{
    public class CategoryAppService : CrudAppService<Category, CategoryDto,
        Guid, PagedAndSortedResultRequestDto, CreateUpdateCategoryDto>, ICategoryAppService
    {
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ILogger<CategoryAppService> _logger;

        public CategoryAppService(
                 ICategoryRepository categoryRepository,
                 IRepository<Product, Guid> productRepository,
                 IGuidGenerator guidGenerator,
                 ILogger<CategoryAppService> logger)
                 : base(categoryRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            GetPolicyName = ProductSellingPermissions.Categories.Default;
            CreatePolicyName = ProductSellingPermissions.Categories.Create;
            UpdatePolicyName = ProductSellingPermissions.Categories.Edit;
            DeletePolicyName = ProductSellingPermissions.Categories.Delete;
            _guidGenerator = guidGenerator;
            _logger = logger;
        }

        // Override CreateAsync to add comprehensive debugging

        [Authorize(ProductSellingPermissions.Categories.Create)]
        public override async Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto input)
        {
            try
            {
                _logger.LogInformation("Creating new category: {Name}", input.Name);

                var categoryId = _guidGenerator.Create();
                var category = new Category(
                    categoryId,
                    input.Name,
                    input.Description,
                    input.UrlSlug,
                    SpecificationType.None
                );

                var insertedCategory = await Repository.InsertAsync(category, autoSave: true);
                _logger.LogInformation("Category created successfully: {Id}", insertedCategory.Id);

                var result = ObjectMapper.Map<Category, CategoryDto>(insertedCategory);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category: {Name}", input.Name);
                throw;
            }
        }

        [Authorize(ProductSellingPermissions.Categories.Edit)]
        public override async Task<CategoryDto> UpdateAsync(Guid id, CreateUpdateCategoryDto input)
        {
            try
            {
                _logger.LogInformation("Updating category: {Id}", id);
                var category = await Repository.GetAsync(id);
                category.Name = input.Name;
                category.Description = input.Description;
                category.UrlSlug = input.UrlSlug;
                var updatedCategory = await Repository.UpdateAsync(category, autoSave: true);
                _logger.LogInformation("Category updated successfully: {Id}", updatedCategory.Id);
                return ObjectMapper.Map<Category, CategoryDto>(updatedCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category: {Id}", id);
                throw;
            }
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