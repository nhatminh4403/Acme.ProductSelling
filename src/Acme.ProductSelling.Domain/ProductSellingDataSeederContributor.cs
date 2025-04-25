using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Acme.ProductSelling
{
    public class ProductSellingDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<Category, Guid> _categoryRepository;
        private readonly CategoryManager _categoryManager;
        public ProductSellingDataSeederContributor(
            IRepository<Product, Guid> productRepository,
            IRepository<Category, Guid> categoryRepository, CategoryManager categoryManager)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _categoryManager = categoryManager;
        }
        public async Task SeedAsync(DataSeedContext context)
        {
            // --- Seed Categories ---
            // Chỉ seed nếu chưa có Category nào trong DB
            if (await _categoryRepository.GetCountAsync() == 0)
            {
                var screens = await _categoryRepository.InsertAsync(
                    await _categoryManager.CreateAsync("Màn hình", "Devices and gadgets"));

                var laptops = await _categoryRepository.InsertAsync(
                   await _categoryManager.CreateAsync("Laptop", "Devices and gadgets"));
                var mice = await _categoryRepository.InsertAsync(
                   await _categoryManager.CreateAsync("Chuột", "Devices and gadgets"));


                // Thêm các category vào DB
                await _categoryRepository.InsertManyAsync(
                    new[] { screens, laptops, mice },
                    autoSave: true // Tự động gọi SaveChangesAsync
                );

                // --- Seed Products ---
                // Chỉ seed Product nếu chưa có Product nào VÀ đã có Category
                // (Đảm bảo category đã được tạo ở bước trên)
                await _productRepository.InsertAsync(
                    new Product
                    {
                        CategoryId = laptops.Id,
                        Price = 2000000,
                        ProductName = "Dell Alienware 15",
                        Description = "Laptop gaming mạnh mẽ với card đồ họa rời",
                        StockCount = 10
                    },
                    autoSave: true
                );
                await _productRepository.InsertAsync(
                   new Product
                   {
                       CategoryId = screens.Id,
                       Price = 2000000,
                       ProductName = "Dell Alienware 15",
                       Description = "Laptop gaming mạnh mẽ với card đồ họa rời",
                       StockCount = 10
                   },
                   autoSave: true
               );
                await _productRepository.InsertAsync(
                   new Product
                   {
                       CategoryId = mice.Id,
                       Price = 2000000,
                       ProductName = "Dell Alienware 15",
                       Description = "Laptop gaming mạnh mẽ với card đồ họa rời",
                       StockCount = 10
                   },
                   autoSave: true
               );
            }

        }
    }
}

