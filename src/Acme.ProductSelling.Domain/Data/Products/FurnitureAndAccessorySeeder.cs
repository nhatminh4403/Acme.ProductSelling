using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Data.BaseSeeder;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Specifications.Models;
using Acme.ProductSelling.Specifications.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Data.Products
{
    public class FurnitureAndAccessorySeeder : ProductSeederBase, IDataSeederContributor
    {
        private readonly ISpecificationRepository _specificationRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;

        public FurnitureAndAccessorySeeder(
            IProductRepository productRepository,
            ProductManager productManager,
            ISpecificationRepository specificationRepository)
            : base(productRepository, productManager)
        {
            _specificationRepository = specificationRepository;
        }

        public void SetDependencies(
            Dictionary<string, Category> categories,
            Dictionary<string, Manufacturer> manufacturers)
        {
            _categories = categories;
            _manufacturers = manufacturers;
        }

        public async Task SeedAsync()
        {
            await SeedCablesAsync();
        }


        private async Task SeedCablesAsync()
        {
            var cable1 = await CreateProductAsync(
                _categories["Cables"].Id, _manufacturers["UGREEN"].Id, 250000, 5,
                "UGREEN USB-C to USB-C 100W Cable 2m", "<p>Cáp USB-C chất lượng cao hỗ trợ <strong>sạc nhanh 100W</strong> và truyền dữ liệu.</p><ul><li>Công suất sạc lên đến 100W</li><li>Chiều dài 2m tiện lợi</li><li>Vỏ bọc bền bỉ, chống rối</li><li>Tương thích với laptop, điện thoại, tablet</li><li>Truyền dữ liệu USB 2.0</li></ul>",
                200, true, DateTime.Now.AddDays(1),
                "https://i5.walmartimages.com/asr/c6f8f7c8-c0f5-4c8e-9d0e-1f7e0e0e0e0e.jpg");

            await _specificationRepository.InsertAsync(new CableSpecification
            {
                ProductId = cable1.Id,
                CableType = CableType.USB_C_to_USB_C,
                Length = 2.0f,
                MaxPower = "100W",
                DataTransferSpeed = "480 Mbps (USB 2.0)",
                Connector1 = "USB Type-C",
                Connector2 = "USB Type-C",
                IsBraided = true,
                Color = "Black",
                Warranty = "24"
            }, autoSave: true);
        }



    }
}
