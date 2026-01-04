using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Data.BaseSeeder;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Specifications.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Data.Products
{
    public class GamingSeeder : ProductSeederBase, IDataSeederContributor
    {
        private readonly IRepository<LaptopSpecification, Guid> _laptopSpecRepository;
        private readonly IRepository<HandheldSpecification, Guid> _handheldSpecRepository;
        private readonly IRepository<ConsoleSpecification, Guid> _consoleSpecRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;

        public GamingSeeder(
            IProductRepository productRepository,
            IRepository<LaptopSpecification, Guid> laptopSpecRepository,
            ProductManager productManager,
            IRepository<HandheldSpecification, Guid> handheldSpecRepository,
            IRepository<ConsoleSpecification, Guid> consoleSpecRepository)
            : base(productRepository, productManager)
        {
            _laptopSpecRepository = laptopSpecRepository;
            _handheldSpecRepository = handheldSpecRepository;
            _consoleSpecRepository = consoleSpecRepository;
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
            await SeedHandheldsAsync();
            await SeedConsolesAsync();
        }

        private async Task SeedLaptopsAsync()
        {
            var laptop1 = await CreateProductAsync(
                _categories["Laptops"].Id, _manufacturers["ASUS"].Id, 25000000, 12,
                "ASUS ROG Zephyrus G16 GU605CX QR083W", "Powerful gaming laptop",
                10, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/zephyrus_g16_gu605_grey_03_rgb_1_b58d513a9306445daf2980232fe2544b_grande.png");

            await _laptopSpecRepository.InsertAsync(new LaptopSpecification
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
                Warranty = "2 years"
            }, autoSave: true);

            var laptop2 = await CreateProductAsync(
                _categories["Laptops"].Id, _manufacturers["Dell"].Id, 60000000, 10,
                "Dell XPS 13 9310", "Ultra-portable 13\" laptop",
                8, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/51529_laptop_dell_xps_9350_xps93_1d46c518185a488a92c40932dd4d5cf6_grande.png");

            await _laptopSpecRepository.InsertAsync(new LaptopSpecification
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
                Warranty = "1 year"
            }, autoSave: true);
        }

        private async Task SeedHandheldsAsync()
        {
            var handheld1 = await CreateProductAsync(
                _categories["Handhelds"].Id, _manufacturers["Valve"].Id, 12000000, 5,
                "Steam Deck 512GB", "Máy chơi game cầm tay, chạy SteamOS",
                20, true, DateTime.Now.AddDays(4),
                "https://cdn.cloudflare.steamstatic.com/steamdeck/images/hero/hero_deck_fullbleed.jpg");

            await _handheldSpecRepository.InsertAsync(new HandheldSpecification
            {
                ProductId = handheld1.Id,
                Processor = "AMD Zen 2 4c/8t",
                Graphics = "AMD RDNA 2 8 CUs",
                RAM = "16GB LPDDR5",
                Storage = "512GB NVMe SSD",
                Display = "7-inch 1280x800 LCD 60Hz",
                BatteryLife = "2-8 hours",
                Weight = "669g",
                OperatingSystem = "SteamOS 3.0",
                Connectivity = ConnectivityType.WirelessAndBluetooth,
                WifiVersion = "WiFi 5 (802.11ac)",
                BluetoothVersion = "5.0"
            }, autoSave: true);
        }

        private async Task SeedConsolesAsync()
        {
            var console1 = await CreateProductAsync(
                _categories["Consoles"].Id, _manufacturers["Sony"].Id, 13000000, 8,
                "PlayStation 5 Digital Edition", "Máy chơi game thế hệ mới, bản kỹ thuật số",
                30, true, DateTime.Now.AddDays(6),
                "https://gmedia.playstation.com/is/image/SIEPDC/ps5-digital-edition-console-image-block-01-en-25jun20");

            await _consoleSpecRepository.InsertAsync(new ConsoleSpecification
            {
                ProductId = console1.Id,
                Processor = "AMD Zen 2 8-core",
                Graphics = "AMD RDNA 2 10.28 TFLOPS",
                RAM = "16GB GDDR6",
                Storage = "825GB SSD",
                OpticalDrive = OpticalDriveType.None,
                MaxResolution = "4K UHD",
                MaxFrameRate = "120fps",
                HDRSupport = true,
                Connectivity = ConnectivityType.WiredWirelessAndBluetooth,
                HasEthernet = true,
                WifiVersion = "WiFi 6 (802.11ax)",
                BluetoothVersion = "5.1"
            }, autoSave: true);
        }
    }
}
