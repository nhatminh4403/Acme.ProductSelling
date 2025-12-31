using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Data.BaseSeeder
{
    public abstract class ProductSeederBase : ITransientDependency
    {
        protected readonly IProductRepository ProductRepository;
        protected readonly ProductManager ProductManager;

        protected ProductSeederBase(
            IProductRepository productRepository,
            ProductManager productManager)
        {
            ProductRepository = productRepository;
            ProductManager = productManager;
        }

        /// <summary>
        /// Creates a product using ProductManager with validation
        /// </summary>
        protected async Task<Product> CreateProductAsync(
            Guid categoryId,
            Guid manufacturerId,
            decimal price,
            double discount,
            string name,
            string description,
            int stock,
            bool isActive,
            DateTime releaseDate,
            string imageUrl)
        {
            // Use ProductManager to create product with built-in validation
            var product = await ProductManager.CreateAsync(
                categoryId: categoryId,
                manufacturerId: manufacturerId,
                productName: name,
                description: description,
                originalPrice: price,
                discountPercent: discount,
                stockCount: stock,
                isActive: isActive,
                releaseDate: releaseDate,
                imageUrl: imageUrl
            );

            // Insert and return the product
            return await ProductRepository.InsertAsync(product, autoSave: true);
        }
    }
}
