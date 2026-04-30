using System;
using System.Threading.Tasks;
using Acme.ProductSelling.Manufacturers;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace Acme.ProductSelling.Manufacturers
{
    public class ManufacturerAppServiceTests : ProductSellingApplicationTestBase<ProductSellingApplicationTestModule>
    {
        private readonly IManufacturerAppService _manufacturerAppService;

        public ManufacturerAppServiceTests()
        {
            _manufacturerAppService = GetRequiredService<IManufacturerAppService>();
        }

        [Fact]
        public async Task Should_Create_A_Valid_Manufacturer()
        {
            // Act
            var result = await _manufacturerAppService.CreateAsync(new CreateUpdateManufacturerDto 
            {
                Name = "Test Manufacturer",
                UrlSlug = "test-manufacturer",
                Description = "Test Description",
                ContactInfo = "Test Contact Info"
            });

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Name.ShouldBe("Test Manufacturer");
        }

        [Fact]
        public async Task Should_Get_A_Manufacturer()
        {
            // Arrange
            var createResult = await _manufacturerAppService.CreateAsync(new CreateUpdateManufacturerDto 
            {
                Name = "Get Manufacturer",
                UrlSlug = "get-manufacturer"
            });

            // Act
            var result = await _manufacturerAppService.GetAsync(createResult.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createResult.Id);
            result.Name.ShouldBe("Get Manufacturer");
        }

        [Fact]
        public async Task Should_Update_A_Manufacturer()
        {
            // Arrange
            var createResult = await _manufacturerAppService.CreateAsync(new CreateUpdateManufacturerDto 
            {
                Name = "Update Manufacturer",
                UrlSlug = "update-manufacturer"
            });

            var updateDto = new CreateUpdateManufacturerDto
            {
                Name = "Updated Manufacturer",
                UrlSlug = "updated-manufacturer"
            };

            // Act
            var updateResult = await _manufacturerAppService.UpdateAsync(createResult.Id, updateDto);

            // Assert
            updateResult.Name.ShouldBe("Updated Manufacturer");
            updateResult.UrlSlug.ShouldBe("updated-manufacturer");
        }

        [Fact]
        public async Task Should_Delete_A_Manufacturer()
        {
            // Arrange
            var createResult = await _manufacturerAppService.CreateAsync(new CreateUpdateManufacturerDto 
            {
                Name = "Delete Manufacturer",
                UrlSlug = "delete-manufacturer"
            });

            // Act
            await _manufacturerAppService.DeleteAsync(createResult.Id);

            // Assert
            var result = await _manufacturerAppService.GetListAsync(new PagedAndSortedResultRequestDto());
            result.Items.ShouldNotContain(m => m.Id == createResult.Id);
        }
    }
}
