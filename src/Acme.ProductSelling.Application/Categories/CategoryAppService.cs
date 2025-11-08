using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
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
        private readonly IRepository<Manufacturer, Guid> _manufacturerRepository;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        public CategoryAppService(
                 ICategoryRepository categoryRepository,
                 IRepository<Product, Guid> productRepository,
                 IGuidGenerator guidGenerator,
                 ILogger<CategoryAppService> logger,
                 IRepository<Manufacturer, Guid> manufacturerRepository,
                 IStringLocalizer<ProductSellingResource> localizer)
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
            _manufacturerRepository = manufacturerRepository;
            _localizer = localizer;
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
        [AllowAnonymous]
        public async Task<GroupedCategoriesResultDto> GetGroupedCategoriesAsync()
        {
            var categories = await _categoryRepository.GetListAsync();

            // Step 1: Efficiently create a lookup that maps CategoryId to a list of its Manufacturers
            var manufacturersByCategory = await GetCategoryManufacturerLookupAsync();

            // Step 2: Group categories by CategoryGroup
            var groupedCategories = categories
                .Where(c => c.CategoryGroup != CategoryGroup.Individual)
                .GroupBy(c => c.CategoryGroup)
                .Select(g => new CategoryGroupDto
                {
                    GroupType = g.Key,
                    GroupName = _localizer[GetGroupLocalizationKey(g.Key)],
                    GroupIcon = g.First().IconCssClass,
                    DisplayOrder = g.First().DisplayOrder,
                    Categories = g.Select(c => new CategoryInGroupDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        UrlSlug = c.UrlSlug,
                        SpecificationType = c.SpecificationType,
                        // Step 3: Use the lookup to get manufacturers
                        Manufacturers = manufacturersByCategory.TryGetValue(c.Id, out var manufs)
                            ? ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(manufs)
                            : new List<ManufacturerDto>()
                    }).ToList()
                })
                .OrderBy(g => g.DisplayOrder)
                .ToList();

            // Step 4: Get individual categories (not in groups)
            var individualCategories = categories
                .Where(c => c.CategoryGroup == CategoryGroup.Individual)
                .Select(c => new CategoryInGroupDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    UrlSlug = c.UrlSlug,
                    SpecificationType = c.SpecificationType,
                    // Step 5: Use the same lookup for individual categories
                    Manufacturers = manufacturersByCategory.TryGetValue(c.Id, out var manufs)
                        ? ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(manufs)
                        : new List<ManufacturerDto>()
                })
                .OrderBy(c => c.Name)
                .ToList();

            return new GroupedCategoriesResultDto
            {
                Groups = groupedCategories,
                IndividualCategories = individualCategories
            };
        }
        [AllowAnonymous]
        public async Task<ListResultDto<CategoryWithManufacturersDto>> GetCategoriesWithManufacturersAsync()
        {
            var categories = await _categoryRepository.GetListAsync();

            // Step 1: Efficiently create a lookup that maps CategoryId to a list of its Manufacturers
            var manufacturersByCategory = await GetCategoryManufacturerLookupAsync();

            // Step 2: Map categories to DTOs, populating manufacturers from the lookup
            var categoryDtos = categories.Select(c =>
            {
                manufacturersByCategory.TryGetValue(c.Id, out var manufacturers);
                var manufacturerList = manufacturers ?? new List<Manufacturer>();

                return new CategoryWithManufacturersDto
                {
                    Id = c.Id,
                    CategoryName = c.Name,
                    CategoryUrlSlug = c.UrlSlug,
                    CategoryGroup = c.CategoryGroup,
                    IconCssClass = c.IconCssClass,
                    // Use the retrieved list to populate Manufacturers and their count
                    Manufacturers = ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(manufacturerList),
                    ManufacturerCount = manufacturerList.Count
                };
            })
            .OrderBy(c => c.CategoryName)
            .ToList();

            return new ListResultDto<CategoryWithManufacturersDto>(categoryDtos);
        }

        private async Task<Dictionary<Guid, List<Manufacturer>>> GetCategoryManufacturerLookupAsync()
        {
            var productQueryable = await _productRepository.GetQueryableAsync();

            var productsWithManufacturers = await AsyncExecuter.ToListAsync(
                productQueryable
                   .Include(p => p.Manufacturer)
                   .Where(p => p.ManufacturerId != null) // Ensure there's a manufacturer
                   .Select(p => new { p.CategoryId, p.Manufacturer })
            );

            return productsWithManufacturers
                   .GroupBy(p => p.CategoryId)
                   .ToDictionary(
                       g => g.Key,
                       g => g.Select(x => x.Manufacturer)
                             .Where(m => m != null)
                             .DistinctBy(m => m.Id) // Ensure each manufacturer is listed only once per category
                             .OrderBy(m => m.Name)
                             .ToList()
                   );
        }
        private string GetGroupLocalizationKey(CategoryGroup group)
        {
            return group switch
            {
                CategoryGroup.MainCpuVga => "CategoryGroup:MainCpuVga",
                CategoryGroup.CaseAndCooling => "CategoryGroup:CaseAndCooling",
                CategoryGroup.StorageRamMemory => "CategoryGroup:StorageRamMemory",
                CategoryGroup.AudioVideo => "CategoryGroup:AudioVideo",
                CategoryGroup.MouseAndPad => "CategoryGroup:MouseAndPad",
                CategoryGroup.Furniture => "CategoryGroup:Furniture",
                CategoryGroup.SoftwareNetwork => "CategoryGroup:SoftwareNetwork",
                CategoryGroup.HandheldConsole => "CategoryGroup:HandheldConsole",
                CategoryGroup.Accessories => "CategoryGroup:Accessories",
                CategoryGroup.Individual => "CategoryGroup:Individual",
                _ => group.ToString()
            };
        }
    }
}