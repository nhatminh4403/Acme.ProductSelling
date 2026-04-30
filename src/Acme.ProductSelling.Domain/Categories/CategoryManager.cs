using Acme.ProductSelling.Utils;
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
                throw new BusinessException(ProductSellingDomainErrorCodes.CategoryNameAlreadyExists)
                    .WithData("Name", name);
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
    }
}