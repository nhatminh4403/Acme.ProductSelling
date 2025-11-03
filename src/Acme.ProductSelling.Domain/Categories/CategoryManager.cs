using Acme.ProductSelling.Utils;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Acme.ProductSelling.Categories
{
    public class CategoryManager : DomainService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryManager(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> CreateAsync(
            string name,
            string description,
            SpecificationType specificationType,
            CategoryGroup categoryGroup = CategoryGroup.Individual,
            int displayOrder = 0,
            string iconCssClass = "fas fa-box")
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            // Kiểm tra xem Category đã tồn tại chưa
            var existingCategory = await _categoryRepository.FindByNameAsync(name);
            if (existingCategory != null)
            {
                throw new Exception($"Category with name '{name}' already exists.");
            }

            // Tạo Category mới
            return new Category(
                GuidGenerator.Create(),
                name,
                description,
                UrlHelperMethod.RemoveDiacritics(name),
                specificationType,
                categoryGroup,
                displayOrder,
                iconCssClass
            );
        }

        public async Task<Category> UpdateAsync(
            Guid id,
            string name,
            string description,
            CategoryGroup? categoryGroup = null,
            int? displayOrder = null,
            string iconCssClass = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            // Kiểm tra xem Category đã tồn tại chưa
            var existingCategory = await _categoryRepository.GetAsync(id);
            if (existingCategory == null)
            {
                throw new Exception($"Category with id '{id}' does not exist.");
            }

            // Cập nhật Category
            existingCategory.Name = name;
            existingCategory.Description = description;
            existingCategory.UrlSlug = UrlHelperMethod.RemoveDiacritics(name);

            if (categoryGroup.HasValue)
                existingCategory.CategoryGroup = categoryGroup.Value;

            if (displayOrder.HasValue)
                existingCategory.DisplayOrder = displayOrder.Value;

            if (!string.IsNullOrWhiteSpace(iconCssClass))
                existingCategory.IconCssClass = iconCssClass;

            return existingCategory;
        }
    }
}