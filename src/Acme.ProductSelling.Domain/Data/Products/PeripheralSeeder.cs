using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Data.BaseSeeder;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Specifications.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Data.Products
{
    public class PeripheralSeeder : ProductSeederBase, IDataSeederContributor
    {
        private readonly IRepository<KeyboardSpecification, Guid> _keyboardSpecRepository;
        private readonly IRepository<MouseSpecification, Guid> _mouseSpecRepository;
        private readonly IRepository<MonitorSpecification, Guid> _monitorSpecRepository;
        private readonly IRepository<HeadsetSpecification, Guid> _headsetSpecRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;
        private Dictionary<string, SwitchType> _switchTypes;
        private Dictionary<string, PanelType> _panelTypes;

        public PeripheralSeeder(
            IProductRepository productRepository,
            ProductManager productManager,
            IRepository<KeyboardSpecification, Guid> keyboardSpecRepository,
            IRepository<MouseSpecification, Guid> mouseSpecRepository,
            IRepository<MonitorSpecification, Guid> monitorSpecRepository,
            IRepository<HeadsetSpecification, Guid> headsetSpecRepository)
            : base(productRepository, productManager)
        {
            _keyboardSpecRepository = keyboardSpecRepository;
            _mouseSpecRepository = mouseSpecRepository;
            _monitorSpecRepository = monitorSpecRepository;
            _headsetSpecRepository = headsetSpecRepository;
        }

        public void SetDependencies(
            Dictionary<string, Category> categories,
            Dictionary<string, Manufacturer> manufacturers,
            Dictionary<string, SwitchType> switchTypes,
            Dictionary<string, PanelType> panelTypes)
        {
            _categories = categories;
            _manufacturers = manufacturers;
            _switchTypes = switchTypes;
            _panelTypes = panelTypes;
        }

        public async Task SeedAsync()
        {
            await SeedKeyboardsAsync();
            await SeedMiceAsync();
            await SeedMonitorsAsync();
            await SeedHeadsetsAsync();
        }

        private async Task SeedKeyboardsAsync()
        {
            var kb1 = await CreateProductAsync(
                _categories["Keyboards"].Id, _manufacturers["Logitech"].Id, 3000000, 7,
                "Logitech G Pro TKL Keyboard", "Tenkeyless mechanical gaming keyboard",
                80, true, DateTime.Now.AddDays(-10),
                "https://product.hstatic.net/200000722513/product/1_5b2f7891bf434a7aab9f1abdba56c17e_grande.jpg");

            await _keyboardSpecRepository.InsertAsync(new KeyboardSpecification
            {
                ProductId = kb1.Id,
                KeyboardType = "Mechanical",
                SwitchTypeId = _switchTypes["Tactile Brown"].Id,
                Layout = KeyboardLayout.TKL,
                Backlight = "RGB"
            }, autoSave: true);

            var kb2 = await CreateProductAsync(
                _categories["Keyboards"].Id, _manufacturers["Razer"].Id, 4500000, 5,
                "Razer Huntsman Elite", "Opto-mechanical gaming keyboard",
                40, true, DateTime.Now.AddMonths(1),
                "https://product.hstatic.net/200000722513/product/r3m1_ac3aa0be001640e2873ff732d34617bc_2295901522e24ce399b8f5f07be51467_3ab2e4aca4434a9a84997283b79b5c3c_grande.png");

            await _keyboardSpecRepository.InsertAsync(new KeyboardSpecification
            {
                ProductId = kb2.Id,
                KeyboardType = "Mechanical",
                SwitchTypeId = _switchTypes["Linear Red"].Id,
                Layout = KeyboardLayout.FullSize,
                Backlight = "RGB"
            }, autoSave: true);
        }

        private async Task SeedMiceAsync()
        {
            var mouse1 = await CreateProductAsync(
                _categories["Mice"].Id, _manufacturers["Logitech"].Id, 1000000, 5,
                "Logitech G502 HERO", "High-performance gaming mouse",
                150, true, DateTime.Now.AddDays(3),
                "https://product.hstatic.net/200000722513/product/10001_01736316d2b443d0838e5a0741434420_grande.png");

            await _mouseSpecRepository.InsertAsync(new MouseSpecification
            {
                ProductId = mouse1.Id,
                Dpi = 16000,
                PollingRate = 1000,
                SensorType = "Optical",
                Weight = 80,
                Connectivity = ConnectivityType.Wired,
                ButtonCount = 6,
                BacklightColor = "RGB",
                Color = "Black"
            }, autoSave: true);

            // Add more mice as needed...
        }

        private async Task SeedMonitorsAsync()
        {
            var monitor1 = await CreateProductAsync(
                _categories["Monitors"].Id, _manufacturers["LG"].Id, 7000000, 6,
                "UltraGear 27GL850", "27-inch QHD gaming monitor",
                30, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/lg_27gx790a-b_gearvn_18880ec6e5a944c2b29c76d85d44d243_medium.jpg");

            await _monitorSpecRepository.InsertAsync(new MonitorSpecification
            {
                ProductId = monitor1.Id,
                ScreenSize = 27,
                Resolution = "2560x1440",
                RefreshRate = 144,
                PanelTypeId = _panelTypes["IPS"].Id,
                ResponseTimeMs = 1,
                ColorGamut = "sRGB 99%",
                Brightness = 400,
                VesaMount = true
            }, autoSave: true);
        }

        private async Task SeedHeadsetsAsync()
        {
            var headset1 = await CreateProductAsync(
                _categories["Headsets"].Id, _manufacturers["Logitech"].Id, 1500000, 5,
                "Logitech G Pro X", "High-quality gaming headset",
                70, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/gvn_logitech_prox_79c556630c454086baf1bee06c577ab7_3471d9d886fd4dbe8ab5ae6bed9f4d78_grande.png");

            await _headsetSpecRepository.InsertAsync(new HeadsetSpecification
            {
                ProductId = headset1.Id,
                Connectivity = ConnectivityType.Wired,
                DriverSize = 50,
                HasMicrophone = true,
                IsNoiseCancelling = true,
                IsSurroundSound = true,
                Frequency = "20Hz - 20kHz",
                MicrophoneType = "Omnidirectional",
                Impedance = 32,
                Sensitivity = 100,
                Color = "Black"
            }, autoSave: true);
        }
    }
}
