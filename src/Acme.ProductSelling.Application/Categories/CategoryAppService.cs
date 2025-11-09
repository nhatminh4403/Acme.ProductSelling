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

            _guidGenerator = guidGenerator;
            _logger = logger;
            _manufacturerRepository = manufacturerRepository;
            _localizer = localizer;
            ConfigurePolicies();
        }
        private void ConfigurePolicies()
        {
            GetPolicyName = ProductSellingPermissions.Categories.Default;
            CreatePolicyName = ProductSellingPermissions.Categories.Create;
            UpdatePolicyName = ProductSellingPermissions.Categories.Edit;
            DeletePolicyName = ProductSellingPermissions.Categories.Delete;
        }

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

        [AllowAnonymous]
        public async Task<GroupedCategoriesResultDto> GetGroupedCategoriesAsync()
        {
            var categories = await _categoryRepository.GetListAsync();

            // Single optimized query to get manufacturer lookup
            var manufacturersByCategory = await GetCategoryManufacturerLookupAsync();

            // Group categories efficiently
            var groupedCategories = categories
                .Where(c => c.CategoryGroup != CategoryGroup.Individual)
                .GroupBy(c => c.CategoryGroup)
                .Select(g => new CategoryGroupDto
                {
                    GroupType = g.Key,
                    GroupName = _localizer[GetGroupLocalizationKey(g.Key)],
                    GroupIcon = g.First().IconCssClass,
                    DisplayOrder = g.First().DisplayOrder,
                    Categories = g.Select(c => MapToCategoryInGroup(c, manufacturersByCategory)).ToList()
                })
                .OrderBy(g => g.DisplayOrder)
                .ToList();

            // Get individual categories
            var individualCategories = categories
                .Where(c => c.CategoryGroup == CategoryGroup.Individual)
                .Select(c => MapToCategoryInGroup(c, manufacturersByCategory))
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
            var manufacturersByCategory = await GetCategoryManufacturerLookupAsync();

            var categoryDtos = categories
                .Select(c => MapToCategoryWithManufacturers(c, manufacturersByCategory))
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
                   .Where(p => p.ManufacturerId != null)
                   .Select(p => new { p.CategoryId, p.Manufacturer }).AsNoTracking()
            );

            return productsWithManufacturers
                   .GroupBy(p => p.CategoryId)
                   .ToDictionary(
                       g => g.Key,
                       g => g.Select(x => x.Manufacturer)
                             .Where(m => m != null)
                             .DistinctBy(m => m.Id)
                             .OrderBy(m => m.Name)
                             .ToList()
                   );
        }


        private CategoryInGroupDto MapToCategoryInGroup(
            Category category,
            Dictionary<Guid, List<Manufacturer>> manufacturersByCategory)
        {
            return new CategoryInGroupDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlSlug = category.UrlSlug,
                SpecificationType = category.SpecificationType,
                Manufacturers = manufacturersByCategory.TryGetValue(category.Id, out var manufs)
                    ? ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(manufs)
                    : new List<ManufacturerDto>()
            };
        }

        private CategoryWithManufacturersDto MapToCategoryWithManufacturers(
            Category category,
            Dictionary<Guid, List<Manufacturer>> manufacturersByCategory)
        {
            var manufacturers = manufacturersByCategory.TryGetValue(category.Id, out var manufs)
                ? manufs
                : new List<Manufacturer>();

            return new CategoryWithManufacturersDto
            {
                Id = category.Id,
                CategoryName = category.Name,
                CategoryUrlSlug = category.UrlSlug,
                CategoryGroup = category.CategoryGroup,
                IconCssClass = category.IconCssClass,
                Manufacturers = ObjectMapper.Map<List<Manufacturer>, List<ManufacturerDto>>(manufacturers),
                ManufacturerCount = manufacturers.Count
            };
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