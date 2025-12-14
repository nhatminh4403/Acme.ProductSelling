using Acme.ProductSelling.Categories.Configurations;
using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Categories.Services;
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

        private readonly CategoryToCategoryDtoMapper _categoryToDtoMapper;
        private readonly CategoryToCategoryLookupDtoMapper _categoryToLookupMapper;
        private readonly ManufacturerToManufacturerDtoMapper _manufacturerToDtoMapper;

        public CategoryAppService(ICategoryRepository categoryRepository,
                                  IRepository<Product, Guid> productRepository,
                                  IGuidGenerator guidGenerator,
                                  ILogger<CategoryAppService> logger,
                                  IRepository<Manufacturer, Guid> manufacturerRepository,
                                  IStringLocalizer<ProductSellingResource> localizer,
                                  CategoryToCategoryDtoMapper categoryToDtoMapper,
                                  CategoryToCategoryLookupDtoMapper categoryToLookupMapper,
                                  ManufacturerToManufacturerDtoMapper manufacturerToDtoMapper)
                 : base(categoryRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;

            _guidGenerator = guidGenerator;
            _logger = logger;
            _manufacturerRepository = manufacturerRepository;
            _localizer = localizer;
            ConfigurePolicies();
            _manufacturerToDtoMapper = manufacturerToDtoMapper;
            _categoryToDtoMapper = categoryToDtoMapper;
            _categoryToLookupMapper = categoryToLookupMapper;
            _categoryToDtoMapper = categoryToDtoMapper;
            _categoryToLookupMapper = categoryToLookupMapper;
            _manufacturerToDtoMapper = manufacturerToDtoMapper;
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

                var result = _categoryToDtoMapper.Map(insertedCategory);
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
                return _categoryToDtoMapper.Map(updatedCategory);
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
            var categories = await _categoryRepository.GetListAsync();
            var lookupDtos = categories.Select(c => _categoryToLookupMapper.Map(c)).ToList();

            return new ListResultDto<CategoryLookupDto>(lookupDtos);
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
            var manufacturers = manufacturersByCategory.TryGetValue(category.Id, out var manufs)
           ? manufs
           : new List<Manufacturer>();

            // Generate price ranges for this category
            var priceRanges = GeneratePriceRangesForCategory(category.SpecificationType);

            return new CategoryInGroupDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlSlug = category.UrlSlug,
                SpecificationType = category.SpecificationType,
                Manufacturers = manufacturers.Select(m => _manufacturerToDtoMapper.Map(m)).ToList(),
                PriceRanges = priceRanges
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
                Manufacturers = manufacturers.Select(m => _manufacturerToDtoMapper.Map(m)).ToList(),
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


        /// <summary>
        /// Generates localized price range DTOs for a category based on its configuration
        /// </summary>
        private List<PriceRangeDto> GeneratePriceRangesForCategory(SpecificationType specificationType)
        {
            if (!CategoryPriceRangeConfiguration.HasPriceRanges(specificationType))
            {
                return new List<PriceRangeDto>();
            }

            var ranges = CategoryPriceRangeConfiguration.GetPriceRangesForCategory(specificationType);

            return ranges.Select(kvp => new PriceRangeDto
            {
                Range = kvp.Key,
                MinPrice = kvp.Value.Min,
                MaxPrice = kvp.Value.Max,
                LocalizationKey = $"PriceRange:{kvp.Key}",
                DisplayText = GetLocalizedPriceRangeText(kvp.Key, kvp.Value.Min, kvp.Value.Max),
                UrlValue = GetUrlValueForPriceRange(kvp.Key, kvp.Value.Min, kvp.Value.Max)
            }).ToList();
        }

        /// <summary>
        /// Gets localized display text for price range with formatted values
        /// </summary>
        private string GetLocalizedPriceRangeText(PriceRangeEnum range, decimal min, decimal max)
        {
            var formattedMin = FormatPriceForDisplay(min);
            var formattedMax = FormatPriceForDisplay(max);

            if (CategoryPriceRangeConfiguration.IsOpenEndedRange(max))
            {
                // Open-ended range: "Over 20M"
                return _localizer["PriceRange:Over", formattedMin];
            }
            else if (min == 0)
            {
                // Start-bounded range: "Under 5M"
                return _localizer["PriceRange:Under", formattedMax];
            }
            else
            {
                // Bounded range: "5M - 10M"
                return _localizer["PriceRange:Between", formattedMin, formattedMax];
            }
        }

        /// <summary>
        /// Formats price value for display (e.g., 5000000 -> "5M")
        /// </summary>
        private string FormatPriceForDisplay(decimal price)
        {
            if (price >= 1000000)
            {
                var millions = price / 1000000;
                return millions % 1 == 0
                    ? $"{(int)millions}M"
                    : $"{millions:0.#}M";
            }
            else if (price >= 1000)
            {
                var thousands = price / 1000;
                return thousands % 1 == 0
                    ? $"{(int)thousands}K"
                    : $"{thousands:0.#}K";
            }
            return price.ToString("N0");
        }

        /// <summary>
        /// Generates URL-friendly string for price range routing
        /// </summary>
        private string GetUrlValueForPriceRange(PriceRangeEnum range, decimal min, decimal max)
        {
            // Create semantic URL values based on the range
            // This could also be stored in localization resources for multi-language support

            if (CategoryPriceRangeConfiguration.IsOpenEndedRange(max))
            {
                // Open-ended ranges (e.g., "over-20m")
                return $"over-{FormatPriceForUrl(min)}";
            }
            else if (min == 0)
            {
                // Start-bounded ranges (e.g., "under-5m")
                return $"under-{FormatPriceForUrl(max + 1)}";
            }
            else
            {
                // Bounded ranges (e.g., "5m-to-10m")
                return $"{FormatPriceForUrl(min)}-to-{FormatPriceForUrl(max)}";
            }
        }

        /// <summary>
        /// Formats price value for URL (e.g., 5000000 -> "5m")
        /// </summary>
        private string FormatPriceForUrl(decimal price)
        {
            if (price >= 1000000)
            {
                var millions = price / 1000000;
                return millions % 1 == 0
                    ? $"{(int)millions}m"
                    : $"{millions:0.#}m";
            }
            else if (price >= 1000)
            {
                var thousands = price / 1000;
                return thousands % 1 == 0
                    ? $"{(int)thousands}k"
                    : $"{thousands:0.#}k";
            }
            return price.ToString();
        }
    }
}