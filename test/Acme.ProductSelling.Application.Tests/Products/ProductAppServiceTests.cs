using System;
using System.Threading.Tasks;
using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Categories.Services;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace Acme.ProductSelling.Products
{
    public class ProductAppServiceTests : ProductSellingApplicationTestBase<ProductSellingApplicationTestModule>
    {
        private readonly IProductAppService _productAppService;
        private readonly ICategoryAppService _categoryAppService;
        private readonly IManufacturerAppService _manufacturerAppService;

        public ProductAppServiceTests()
        {
            _productAppService = GetRequiredService<IProductAppService>();
            _categoryAppService = GetRequiredService<ICategoryAppService>();
            _manufacturerAppService = GetRequiredService<IManufacturerAppService>();
        }

        private async Task<(Guid categoryId, Guid manufacturerId)> CreateDependenciesAsync()
        {
            var category = await _categoryAppService.CreateAsync(new CreateUpdateCategoryDto 
            {
                Name = "Test Product Category",
                Description = "Used for testing products",
                UrlSlug = "test-product-category",
                DisplayOrder = 1,
                CategoryGroup = CategoryGroup.Individual,
                SpecificationType = SpecificationType.None
            });

            var manufacturer = await _manufacturerAppService.CreateAsync(new CreateUpdateManufacturerDto 
            {
                Name = "Test Product Manufacturer",
                UrlSlug = "test-product-manufacturer"
            });

            return (category.Id, manufacturer.Id);
        }

        [Fact]
        public async Task Should_Create_A_Valid_Product()
        {
            // Arrange
            var (categoryId, manufacturerId) = await CreateDependenciesAsync();

            // Act
            var result = await _productAppService.CreateAsync(new CreateUpdateProductDto 
            {
                ProductName = "Test Product",
                Description = "Test Product Description",
                OriginalPrice = 100m,
                DiscountedPrice = 90m,
                DiscountPercent = 10,
                StockCount = 50,
                CategoryId = categoryId,
                ManufacturerId = manufacturerId,
                UrlSlug = "test-product-with-spec",
            });

            // Assert
            result.Id.ShouldNotBe(Guid.Empty);
            result.ProductName.ShouldBe("Test Product");
            result.OriginalPrice.ShouldBe(100m);
        }

        [Fact]
        public async Task Should_Get_A_Product()
        {
            // Arrange
            var (categoryId, manufacturerId) = await CreateDependenciesAsync();
            var createResult = await _productAppService.CreateAsync(new CreateUpdateProductDto 
            {
                ProductName = "Get Product",
                Description = "Get Product Description",
                OriginalPrice = 150m,
                StockCount = 10,
                CategoryId = categoryId,
                ManufacturerId = manufacturerId,
                UrlSlug = "get-product"
            });

            // Act
            var result = await _productAppService.GetAsync(createResult.Id);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createResult.Id);
            result.ProductName.ShouldBe("Get Product");
        }

        [Fact]
        public async Task Should_Update_A_Product()
        {
            // Arrange
            var (categoryId, manufacturerId) = await CreateDependenciesAsync();
            var createResult = await _productAppService.CreateAsync(new CreateUpdateProductDto 
            {
                ProductName = "Update Product",
                Description = "Before Update",
                OriginalPrice = 200m,
                StockCount = 5,
                CategoryId = categoryId,
                ManufacturerId = manufacturerId,
                UrlSlug = "update-product"
            });

            var updateDto = new CreateUpdateProductDto
            {
                ProductName = "Updated Product",
                Description = "After Update",
                OriginalPrice = 250m,
                StockCount = 15,
                CategoryId = categoryId,
                ManufacturerId = manufacturerId,
                UrlSlug = "updated-product"
            };

            // Act
            var updateResult = await _productAppService.UpdateAsync(createResult.Id, updateDto);

            // Assert
            updateResult.ProductName.ShouldBe("Updated Product");
            updateResult.OriginalPrice.ShouldBe(250m);
            updateResult.StockCount.ShouldBe(15);
        }

        [Fact]
        public async Task Should_Delete_A_Product()
        {
            // Arrange
            var (categoryId, manufacturerId) = await CreateDependenciesAsync();
            var createResult = await _productAppService.CreateAsync(new CreateUpdateProductDto 
            {
                ProductName = "Delete Product",
                Description = "To be deleted",
                OriginalPrice = 50m,
                StockCount = 1,
                CategoryId = categoryId,
                ManufacturerId = manufacturerId,
                UrlSlug = "delete-product"
            });

            // Act
            await _productAppService.DeleteAsync(createResult.Id);

            // Assert
            var result = await _productAppService.GetListAsync(new PagedAndSortedResultRequestDto());
            result.Items.ShouldNotContain(p => p.Id == createResult.Id);
        }
    }
}
