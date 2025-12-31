using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Utils;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Acme.ProductSelling.Products
{
    public class ProductManager : DomainService
    {
        private readonly IProductRepository _productRepository;
        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<Product> CreateAsync(
            Guid categoryId,
            Guid manufacturerId,
            string productName,
            string description,
            decimal originalPrice,
            double discountPercent = 0,
            int stockCount = 0,
            bool isActive = true,
            DateTime? releaseDate = null,
            string imageUrl = null)
        {
            // Validate required fields
            Check.NotNullOrWhiteSpace(productName, nameof(productName));
            Check.NotNull(categoryId, nameof(categoryId));
            Check.NotNull(manufacturerId, nameof(manufacturerId));

            if (originalPrice < 0)
            {
                throw new BusinessException("Product price cannot be negative.");
            }

            if (discountPercent < 0 || discountPercent > 100)
            {
                throw new BusinessException("Discount percent must be between 0 and 100.");
            }

            if (stockCount < 0)
            {
                throw new BusinessException("Stock count cannot be negative.");
            }

            // Check if product with same name already exists
            var existingProduct = await _productRepository.FindByNameAsync(productName);
            if (existingProduct != null)
            {
                throw new BusinessException($"Product with name '{productName}' already exists.");
            }

            // Create new product
            var product = new Product
            {
                CategoryId = categoryId,
                ManufacturerId = manufacturerId,
                ProductName = productName,
                Description = description,
                UrlSlug = UrlHelperMethod.RemoveDiacritics(productName),
                OriginalPrice = originalPrice,
                DiscountPercent = discountPercent,
                StockCount = stockCount,
                IsActive = isActive,
                ReleaseDate = releaseDate ?? DateTime.Now,
                ImageUrl = imageUrl
            };

            return product;
        }

    }
}
