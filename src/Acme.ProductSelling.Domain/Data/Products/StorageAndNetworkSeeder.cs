using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Data.BaseSeeder;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Specifications.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Data.Products
{
    public class StorageAndNetworkSeeder : ProductSeederBase, IDataSeederContributor
    {
        private readonly IRepository<MemoryCardSpecification, Guid> _memoryCardSpecRepository;
        private readonly IRepository<SoftwareSpecification, Guid> _softwareSpecRepository;
        private readonly IRepository<NetworkHardwareSpecification, Guid> _networkHardwareSpecRepository;
        private readonly IRepository<MousePadSpecification, Guid> _mousePadSpecRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;

        public StorageAndNetworkSeeder(
            IProductRepository productRepository,
            ProductManager productManager,
            IRepository<MemoryCardSpecification, Guid> memoryCardSpecRepository,
            IRepository<SoftwareSpecification, Guid> softwareSpecRepository,
            IRepository<NetworkHardwareSpecification, Guid> networkHardwareSpecRepository,
            IRepository<MousePadSpecification, Guid> mousePadSpecRepository)
            : base(productRepository, productManager)
        {
            _memoryCardSpecRepository = memoryCardSpecRepository;
            _softwareSpecRepository = softwareSpecRepository;
            _networkHardwareSpecRepository = networkHardwareSpecRepository;
            _mousePadSpecRepository = mousePadSpecRepository;
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
            await SeedMemoryCardsAsync();
            await SeedSoftwareAsync();
            await SeedNetworkHardwareAsync();
            await SeedMousePadsAsync();
        }

        private async Task SeedMemoryCardsAsync()
        {
            var memoryCard1 = await CreateProductAsync(
                _categories["Memory Cards"].Id, _manufacturers["Samsung"].Id, 350000, 8,
                "Samsung EVO Plus microSD 128GB", "Thẻ nhớ tốc độ cao, chống nước, chống sốc",
                200, true, DateTime.Now.AddDays(1),
                "https://images.samsung.com/is/image/samsung/p6pim/vn/mb-mc128ka-apc/gallery/vn-microsdxc-evo-plus-mb-mc128ka-535234-mb-mc128ka-apc-538392334");

            await _memoryCardSpecRepository.InsertAsync(new MemoryCardSpecification
            {
                ProductId = memoryCard1.Id,
                Capacity = 128,
                CardType = CardType.MicroSDHC,
                SpeedClass = "U3, V30, A2",
                ReadSpeed = 130,
                WriteSpeed = 90,
                Warranty = "10 năm",
                Waterproof = true,
                Shockproof = true
            }, autoSave: true);
        }

        private async Task SeedSoftwareAsync()
        {
            var software1 = await CreateProductAsync(
                _categories["Software"].Id, _manufacturers["Microsoft"].Id, 3200000, 0,
                "Windows 11 Pro", "Hệ điều hành Windows 11 Pro bản quyền",
                500, true, DateTime.Now.AddDays(1),
                "https://cdn-dynmedia-1.microsoft.com/is/image/microsoftcorp/RWZQBF_Hero_960x540_2x_RE4VnJV");

            await _softwareSpecRepository.InsertAsync(new SoftwareSpecification
            {
                ProductId = software1.Id,
                SoftwareType = SoftwareType.OperatingSystem,
                LicenseType = LicenseType.Retail,
                Platform = Platform.Windows,
                Version = "22H2",
                Language = "Multi-language",
                DeliveryMethod = "Digital Download/USB",
                SystemRequirements = "64-bit processor, 4GB RAM, 64GB storage",
                IsSubscription = false
            }, autoSave: true);
        }

        private async Task SeedNetworkHardwareAsync()
        {
            var network1 = await CreateProductAsync(
                _categories["Networking"].Id, _manufacturers["TP-Link"].Id, 1200000, 8,
                "TP-Link Archer AX55 AX3000", "Router WiFi 6 dual-band, tốc độ 3Gbps",
                40, true, DateTime.Now.AddDays(2),
                "https://static.tp-link.com/upload/product-overview/2021/202106/20210625155708974.png");

            await _networkHardwareSpecRepository.InsertAsync(new NetworkHardwareSpecification
            {
                ProductId = network1.Id,
                DeviceType = NetworkDeviceType.Router,
                WifiStandard = WifiStandard.WiFi6_802_11ax,
                MaxSpeed = "3000 Mbps",
                Frequency = "2.4GHz & 5GHz",
                EthernetPorts = "4x Gigabit LAN, 1x Gigabit WAN",
                AntennaCount = 4,
                HasUsb = false,
                SecurityProtocol = "WPA3",
                Coverage = "Up to 2500 sq ft"
            }, autoSave: true);
        }

        private async Task SeedMousePadsAsync()
        {
            var mousePad1 = await CreateProductAsync(
                _categories["Mouse Pads"].Id, _manufacturers["SteelSeries"].Id, 800000, 15,
                "SteelSeries QcK Heavy XXL", "Lót chuột vải lớn 900x400mm, dày 6mm",
                120, true, DateTime.Now.AddDays(2),
                "https://media.steelseriescdn.com/thumbs/catalogue/products/00431-qck-heavy-xxl/c0eb6b6563984f2fab338c58e37b0ee1.png.500x400_q100_crop-fit_optimize.png");

            await _mousePadSpecRepository.InsertAsync(new MousePadSpecification
            {
                ProductId = mousePad1.Id,
                Width = 900,
                Height = 400,
                Thickness = 6,
                Material = MousePadMaterial.Cloth,
                SurfaceType = SurfaceType.Smooth,
                BaseType = "Non-slip rubber",
                HasRgb = false,
                IsWashable = true,
                Color = "Black"
            }, autoSave: true);
        }
    }
}
