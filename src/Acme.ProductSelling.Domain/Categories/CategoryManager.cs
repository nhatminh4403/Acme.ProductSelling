using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        public async Task<Category> CreateAsync(string name, string description)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            // Kiểm tra xem Category đã tồn tại chưa
            var existingCategory = await _categoryRepository.FindByNameAsync(name);
            if (existingCategory != null)
            {
                throw new Exception($"Category with name '{name}' already exists.");
            }
            // Tạo Category mới
            return new Category(GuidGenerator.Create(), name, description);

        }
        public async Task<Category> UpdateAsync(Guid id, string name, string description)
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
            return existingCategory;
        }
    }
}
