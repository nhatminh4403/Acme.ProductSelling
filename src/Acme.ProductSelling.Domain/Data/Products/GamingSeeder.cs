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
    public class GamingSeeder : ProductSeederBase, IDataSeederContributor
    {
        //private readonly IRepository<LaptopSpecification, Guid> _laptopSpecRepository;
        //private readonly IRepository<HandheldSpecification, Guid> _handheldSpecRepository;
        //private readonly IRepository<ConsoleSpecification, Guid> _consoleSpecRepository;

        private readonly ISpecificationRepository _specificationRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;

        public GamingSeeder(
            IProductRepository productRepository,
            ProductManager productManager,

            //IRepository<LaptopSpecification, Guid> laptopSpecRepository,
            //IRepository<HandheldSpecification, Guid> handheldSpecRepository,
            //IRepository<ConsoleSpecification, Guid> consoleSpecRepository,
            ISpecificationRepository specificationRepository)
            : base(productRepository, productManager)
        {
            //_laptopSpecRepository = laptopSpecRepository;
            //_handheldSpecRepository = handheldSpecRepository;
            //_consoleSpecRepository = consoleSpecRepository;
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
            await SeedLaptopsAsync();
        }

        private async Task SeedLaptopsAsync()
        {
            var laptop1 = await CreateProductAsync(
                _categories["Laptops"].Id, _manufacturers["ASUS"].Id, 25000000, 12,
                "ASUS ROG Zephyrus G16 GU605CX QR083W", "Powerful gaming laptop",
                10, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/zephyrus_g16_gu605_grey_03_rgb_1_b58d513a9306445daf2980232fe2544b_grande.png");

            await _specificationRepository.InsertAsync(new LaptopSpecification
            {
                ProductId = laptop1.Id,
                CPU = "Intel® Core™ Ultra 9",
                RAM = "64GB LPDDR5X",
                Storage = "NVMe SSD",
                Display = "16-inch 2.5K 240Hz OLED",
                GraphicsCard = "NVIDIA GeForce RTX 4080",
                OperatingSystem = "Windows 11 Home",
                BatteryLife = "Up to 5 hours",
                Weight = "1.95 Kg",
                Warranty = "2 years",
                IsForGaming = true

            }, autoSave: true);

            var laptop2 = await CreateProductAsync(
                _categories["Laptops"].Id, _manufacturers["Dell"].Id, 60000000, 10,
                "Dell XPS 13 9310", "Ultra-portable 13\" laptop",
                8, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/51529_laptop_dell_xps_9350_xps93_1d46c518185a488a92c40932dd4d5cf6_grande.png");

            await _specificationRepository.InsertAsync(new LaptopSpecification
            {
                ProductId = laptop2.Id,
                CPU = "Intel® Core™ Ultra 5",
                RAM = "16GB LPDDR4x",
                Storage = "PCIe SSD",
                Display = "13.4-inch FHD+ 60Hz",
                GraphicsCard = "Intel Arc Graphics",
                OperatingSystem = "Windows 11 Pro",
                BatteryLife = "Up to 12 hours",
                Weight = "1.2 Kg",
                Warranty = "1 year",
                IsForGaming = false
            }, autoSave: true);
        }

    }
}
