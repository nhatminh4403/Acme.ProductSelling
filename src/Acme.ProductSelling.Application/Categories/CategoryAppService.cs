using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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

        public CategoryAppService(
                 ICategoryRepository categoryRepository,
                 IRepository<Product, Guid> productRepository,
                 IGuidGenerator guidGenerator)
                 : base(categoryRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            GetPolicyName = ProductSellingPermissions.Categories.Default;
            CreatePolicyName = ProductSellingPermissions.Categories.Create;
            UpdatePolicyName = ProductSellingPermissions.Categories.Edit;
            DeletePolicyName = ProductSellingPermissions.Categories.Delete;
            _guidGenerator = guidGenerator;
        }

        // Override CreateAsync to add comprehensive debugging
        public override async Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto input)
        {
            try
            {
                Logger.LogInformation($"CreateAsync started - Name: {input.Name}, Description: {input.Description}, UrlSlug: {input.UrlSlug}");

                // Create the entity using your constructor
                var categoryId = _guidGenerator.Create();
                var category = new Category(
                    categoryId,
                    input.Name,
                    input.Description,
                    input.UrlSlug,
                    SpecificationType.None
                );

                Logger.LogInformation($"Category entity created - ID: {category.Id}, Name: {category.Name}");

                // Insert the entity
                var insertedCategory = await Repository.InsertAsync(category, autoSave: true);
                Logger.LogInformation($"Category inserted - ID: {insertedCategory.Id}");

                // Verify it was saved by retrieving it
                var retrievedCategory = await Repository.FindAsync(insertedCategory.Id);
                if (retrievedCategory != null)
                {
                    Logger.LogInformation($"Category successfully retrieved from database - ID: {retrievedCategory.Id}, Name: {retrievedCategory.Name}");
                }
                else
                {
                    Logger.LogError($"Category was not found in database after insertion - ID: {insertedCategory.Id}");
                }

                // Map to DTO and return
                var result = ObjectMapper.Map<Category, CategoryDto>(insertedCategory);
                Logger.LogInformation($"CreateAsync completed successfully - Result ID: {result.Id}");

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error in CreateAsync: {ex.Message}");
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

        // Keep the original MapToEntityAsync for reference, but it won't be used now
        protected override Task<Category> MapToEntityAsync(CreateUpdateCategoryDto createInput)
        {
            Logger.LogInformation($"MapToEntityAsync called - this should NOT be called if using overridden CreateAsync");

            var category = new Category(
                _guidGenerator.Create(),
                createInput.Name,
                createInput.Description,
                createInput.UrlSlug,
                SpecificationType.None
            );
            return Task.FromResult(category);
        }
    }
}