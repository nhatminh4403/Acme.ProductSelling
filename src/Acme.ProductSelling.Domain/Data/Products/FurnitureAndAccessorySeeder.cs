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
    public class FurnitureAndAccessorySeeder : ProductSeederBase, IDataSeederContributor
    {
        private readonly IRepository<ChairSpecification, Guid> _chairSpecRepository;
        private readonly IRepository<DeskSpecification, Guid> _deskSpecRepository;
        private readonly IRepository<CableSpecification, Guid> _cableSpecRepository;
        private readonly IRepository<ChargerSpecification, Guid> _chargerSpecRepository;
        private readonly IRepository<PowerBankSpecification, Guid> _powerBankSpecRepository;
        private readonly IRepository<HubSpecification, Guid> _hubSpecRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;

        public FurnitureAndAccessorySeeder(
            IProductRepository productRepository,
            IRepository<ChairSpecification, Guid> chairSpecRepository,
            IRepository<DeskSpecification, Guid> deskSpecRepository,
            IRepository<CableSpecification, Guid> cableSpecRepository,
            IRepository<ChargerSpecification, Guid> chargerSpecRepository,
            IRepository<PowerBankSpecification, Guid> powerBankSpecRepository,
            ProductManager productManager,
            IRepository<HubSpecification, Guid> hubSpecRepository)
            : base(productRepository, productManager)
        {
            _chairSpecRepository = chairSpecRepository;
            _deskSpecRepository = deskSpecRepository;
            _cableSpecRepository = cableSpecRepository;
            _chargerSpecRepository = chargerSpecRepository;
            _powerBankSpecRepository = powerBankSpecRepository;
            _hubSpecRepository = hubSpecRepository;
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
            await SeedChairsAsync();
            await SeedDesksAsync();
            await SeedCablesAsync();
            await SeedChargersAsync();
            await SeedPowerBanksAsync();
            await SeedHubsAsync();
        }

        private async Task SeedChairsAsync()
        {
            var chair1 = await CreateProductAsync(
                _categories["Chairs"].Id, _manufacturers["Corsair"].Id, 8500000, 10,
                "Corsair TC100 Relaxed", "Ghế gaming vải thoáng khí, êm ái",
                25, true, DateTime.Now.AddDays(7),
                "https://assets.corsair.com/image/upload/f_auto,q_auto/content/TC100-RELAXED-BLACK-Gallery-Hero-01.png");

            await _chairSpecRepository.InsertAsync(new ChairSpecification
            {
                ProductId = chair1.Id,
                ChairType = "Gaming/Office",
                Material = ChairMaterial.Fabric,
                MaxWeight = 120,
                ArmrestType = ArmrestType.Adjustable_3D,
                BackrestAdjustment = "Recline 90°-160°",
                SeatHeight = "42-52cm",
                HasLumbarSupport = true,
                HasHeadrest = true,
                BaseType = "Nylon 5-star",
                WheelType = "60mm PU caster",
                Color = "Black"
            }, autoSave: true);
        }

        private async Task SeedDesksAsync()
        {
            var desk1 = await CreateProductAsync(
                _categories["Desks"].Id, _manufacturers["IKEA"].Id, 3500000, 5,
                "IKEA UPPSPEL Gaming Desk", "Bàn gaming 140x80cm, có hệ thống quản lý dây",
                15, true, DateTime.Now.AddDays(3),
                "https://www.ikea.com/us/en/images/products/uppspel-gaming-desk-black__0981784_pe815503_s5.jpg");

            await _deskSpecRepository.InsertAsync(new DeskSpecification
            {
                ProductId = desk1.Id,
                Width = 140,
                Depth = 80,
                Height = 73,
                Material = DeskMaterial.ParticleBoard,
                MaxWeight = 50,
                IsHeightAdjustable = false,
                HasCableManagement = true,
                HasCupHolder = false,
                HasHeadphoneHook = true,
                SurfaceType = "Smooth laminate",
                Color = "Black"
            }, autoSave: true);
        }

        private async Task SeedCablesAsync()
        {
            var cable1 = await CreateProductAsync(
                _categories["Cables"].Id, _manufacturers["UGREEN"].Id, 250000, 5,
                "UGREEN USB-C to USB-C 100W Cable 2m", "Cáp USB-C hỗ trợ sạc nhanh 100W, truyền dữ liệu",
                200, true, DateTime.Now.AddDays(1),
                "https://i5.walmartimages.com/asr/c6f8f7c8-c0f5-4c8e-9d0e-1f7e0e0e0e0e.jpg");

            await _cableSpecRepository.InsertAsync(new CableSpecification
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

        private async Task SeedChargersAsync()
        {
            var charger1 = await CreateProductAsync(
                _categories["Chargers"].Id, _manufacturers["Anker"].Id, 1200000, 15,
                "Anker 737 Charger GaNPrime 120W", "Sạc nhanh 3 cổng USB-C + USB-A, GaN công nghệ mới",
                80, true, DateTime.Now.AddDays(5),
                "https://m.media-amazon.com/images/I/61Zfh+vkVoL._AC_SL1500_.jpg");

            await _chargerSpecRepository.InsertAsync(new ChargerSpecification
            {
                ProductId = charger1.Id,
                ChargerType = ChargerType.WallCharger,
                TotalWattage = 120,
                PortCount = 3,
                UsbCPorts = 2,
                UsbAPorts = 1,
                MaxOutputPerPort = "100W (USB-C1), 100W (USB-C2), 22.5W (USB-A)",
                FastChargingProtocols = "PD 3.0, QC 3.0, PPS",
                CableIncluded = false,
                HasFoldablePlug = true,
                Technology = "GaN (Gallium Nitride)",
                Color = "Black"
            }, autoSave: true);
        }

        private async Task SeedPowerBanksAsync()
        {
            var powerBank1 = await CreateProductAsync(
                _categories["Power Banks"].Id, _manufacturers["Anker"].Id, 2800000, 10,
                "Anker 737 Power Bank 24000mAh", "Sạc dự phòng 140W, màn hình LED hiển thị",
                50, true, DateTime.Now.AddDays(4),
                "https://m.media-amazon.com/images/I/61SfU5bj9uL._AC_SL1500_.jpg");

            await _powerBankSpecRepository.InsertAsync(new PowerBankSpecification
            {
                ProductId = powerBank1.Id,
                Capacity = 24000,
                TotalWattage = 140,
                PortCount = 3,
                UsbCPorts = 2,
                UsbAPorts = 1,
                InputPorts = "USB-C 140W",
                MaxOutputPerPort = "140W (USB-C1), 100W (USB-C2), 18W (USB-A)",
                FastChargingProtocols = "PD 3.1, QC 3.0, PPS",
                RechargingTime = "About 1 hour (with 140W charger)",
                HasDisplay = true,
                Weight = 640,
                Color = "Black"
            }, autoSave: true);
        }

        private async Task SeedHubsAsync()
        {
            var hub1 = await CreateProductAsync(
                _categories["Hubs And Docks"].Id, _manufacturers["Anker"].Id, 2500000, 12,
                "Anker PowerExpand Elite 13-in-1", "Docking station USB-C 13 cổng, 85W PD",
                35, true, DateTime.Now.AddDays(3),
                "https://d2j6dbq0eux0bg.cloudfront.net/images/66203968/2624850885.jpg");

            await _hubSpecRepository.InsertAsync(new HubSpecification
            {
                ProductId = hub1.Id,
                HubType = HubType.Thunderbolt_3_Dock,
                PortCount = 13,
                UsbAPorts = "3x USB-A 3.0",
                UsbCPorts = "2x USB-C (1x with 85W PD)",
                HdmiPorts = 1,
                DisplayPorts = 0,
                EthernetPort = true,
                SdCardReader = true,
                AudioJack = true,
                MaxDisplays = 2,
                MaxResolution = "4K@60Hz",
                PowerDelivery = "85W",
                Color = "Gray"
            }, autoSave: true);
        }
    }
}
