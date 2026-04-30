using System;
using System.Threading.Tasks;
using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Categories.Services;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace Acme.ProductSelling.Categories
{
    public class CategoryAppServiceTests : ProductSellingApplicationTestBase<ProductSellingApplicationTestModule>
    {
        private readonly ICategoryAppService _categoryAppService;

        public CategoryAppServiceTests()
        {
            _categoryAppService = GetRequiredService<ICategoryAppService>();
        }

        [Fact]
        public async Task Should_Create_A_Valid_Category()
        {
            // Act
            var result = await _categoryAppService.CreateAsync(new CreateUpdateCategoryDto 
            {
                Name = "Test Category",
                Description = "Test Description",
                UrlSlug = "test-category",
                DisplayOrder = 1,
                CategoryGroup = CategoryGroup.Individual,
                SpecificationType = SpecificationType.None
            });

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Name.ShouldBe("Test Category");
        }

        [Fact]
        public async Task Should_Get_A_Category()
        {
            // Arrange
            var createResult = await _categoryAppService.CreateAsync(new CreateUpdateCategoryDto 
            {
                Name = "Get Category",
                Description = "Will retrieve this",
                UrlSlug = "get-category",
                DisplayOrder = 2
            });

            // Act
            var result = await _categoryAppService.GetAsync(createResult.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createResult.Id);
            result.Name.ShouldBe("Get Category");
        }

        [Fact]
        public async Task Should_Update_A_Category()
        {
            // Arrange
            var createResult = await _categoryAppService.CreateAsync(new CreateUpdateCategoryDto 
            {
                Name = "Update Category",
                Description = "Description",
                UrlSlug = "update-category",
                DisplayOrder = 1
            });

            var updateDto = new CreateUpdateCategoryDto
            {
                Name = "Updated Category",
                Description = "Updated Description",
                UrlSlug = "updated-category",
                DisplayOrder = 2
            };

            // Act
            var updateResult = await _categoryAppService.UpdateAsync(createResult.Id, updateDto);

            // Assert
            updateResult.Name.ShouldBe("Updated Category");
            updateResult.Description.ShouldBe("Updated Description");
        }

        [Fact]
        public async Task Should_Delete_A_Category()
        {
            // Arrange
            var createResult = await _categoryAppService.CreateAsync(new CreateUpdateCategoryDto 
            {
                Name = "Delete Category",
                Description = "Delete me",
                UrlSlug = "delete-category"
            });

            // Act
            await _categoryAppService.DeleteAsync(createResult.Id);

            // Assert
            var result = await _categoryAppService.GetListAsync(new PagedAndSortedResultRequestDto());
            result.Items.ShouldNotContain(c => c.Id == createResult.Id);
        }
    }
}
