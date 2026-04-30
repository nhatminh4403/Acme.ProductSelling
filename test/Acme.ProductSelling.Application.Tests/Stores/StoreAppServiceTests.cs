using System;
using System.Threading.Tasks;
using Acme.ProductSelling.Stores;
using Acme.ProductSelling.Stores.Dtos;
using Acme.ProductSelling.Stores.Services;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace Acme.ProductSelling.Stores
{
    public class StoreAppServiceTests : ProductSellingApplicationTestBase<ProductSellingApplicationTestModule>
    {
        private readonly IStoreAppService _storeAppService;

        public StoreAppServiceTests()
        {
            _storeAppService = GetRequiredService<IStoreAppService>();
        }

        [Fact]
        public async Task Should_Create_A_Valid_Store()
        {
            // Act
            var result = await _storeAppService.CreateAsync(new CreateUpdateStoreDto 
            {
                Name = "Test Store",
                Code = "TEST01",
                Address = "123 Test St",
                PhoneNumber = "123-456-7890",
                City = "Testville",
                State = "TS",
                Email = "test@store.local"
            });

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.Name.ShouldBe("Test Store");
        }

        [Fact]
        public async Task Should_Get_A_Store()
        {
            // Arrange
            var createResult = await _storeAppService.CreateAsync(new CreateUpdateStoreDto 
            {
                Name = "Get Store",
                Code = "GET01",
                PhoneNumber = "123-456-7890"
            });

            // Act
            var result = await _storeAppService.GetAsync(createResult.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createResult.Id);
            result.Name.ShouldBe("Get Store");
        }

        [Fact]
        public async Task Should_Update_A_Store()
        {
            // Arrange
            var createResult = await _storeAppService.CreateAsync(new CreateUpdateStoreDto 
            {
                Name = "Update Store",
                Code = "UPD01",
                PhoneNumber = "111-222-3333"
            });

            var updateDto = new CreateUpdateStoreDto
            {
                Name = "Updated Store",
                Code = "UPD01",
                PhoneNumber = "999-888-7777"
            };

            // Act
            var updateResult = await _storeAppService.UpdateAsync(createResult.Id, updateDto);

            // Assert
            updateResult.Name.ShouldBe("Updated Store");
            updateResult.PhoneNumber.ShouldBe("999-888-7777");
        }

        [Fact]
        public async Task Should_Delete_A_Store()
        {
            // Arrange
            var createResult = await _storeAppService.CreateAsync(new CreateUpdateStoreDto 
            {
                Name = "Delete Store",
                Code = "DEL01",
                PhoneNumber = "000-000-0000"
            });

            // Act
            await _storeAppService.DeleteAsync(createResult.Id);

            // Assert
            var result = await _storeAppService.GetListAsync(new PagedAndSortedResultRequestDto());
            result.Items.ShouldNotContain(s => s.Id == createResult.Id);
        }
    }
}
