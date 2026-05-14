using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Data.BaseSeeder;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Specifications.Models;
using Acme.ProductSelling.Specifications.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Data.Products
{
    public class PeripheralSeeder : ProductSeederBase, IDataSeederContributor
    {
        private readonly ISpecificationRepository _specificationRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;
        private Dictionary<string, SwitchType> _switchTypes;
        private Dictionary<string, PanelType> _panelTypes;

        public PeripheralSeeder(
            IProductRepository productRepository,
            ProductManager productManager,
            ISpecificationRepository specificationRepository)
            : base(productRepository, productManager)
        {
            _specificationRepository = specificationRepository;
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
                "Logitech G Pro TKL Keyboard", "<p>Bàn phím cơ gaming <strong>Tenkeyless</strong> chuyên nghiệp cho esports.</p><ul><li>Thiết kế TKL gọn gàng, di động</li><li>Switch cơ học Tactile Brown</li><li>Đèn nền RGB tùy chỉnh</li><li>Khung nhôm chắc chắn</li><li>Dây cáp tháo rời</li><li>Lý tưởng cho gaming chuyên nghiệp</li></ul>",
                80, true, DateTime.Now.AddDays(-10),
                "https://product.hstatic.net/200000722513/product/1_5b2f7891bf434a7aab9f1abdba56c17e_grande.jpg");

            await _specificationRepository.InsertAsync(new KeyboardSpecification
            {
                ProductId = kb1.Id,
                KeyboardType = "Mechanical",
                SwitchTypeId = _switchTypes["Tactile Brown"].Id,
                Layout = KeyboardLayout.TKL,
                Backlight = "RGB"
            }, autoSave: true);

            var kb2 = await CreateProductAsync(
                _categories["Keyboards"].Id, _manufacturers["Razer"].Id, 4500000, 5,
                "Razer Huntsman Elite", "<p>Bàn phím cơ <strong>opto-mechanical</strong> cao cấp với phản hồi cực nhanh.</p><ul><li>Switch opto-mechanical Linear Red</li><li>Phản hồi cực nhanh với ánh sáng</li><li>Đèn RGB Chroma đẹp mắt</li><li>Tựa tay từ tính thoải mái</li><li>Bánh xe điều khiển đa phương tiện</li><li>Full-size với numpad</li></ul>",
                40, true, DateTime.Now.AddMonths(1),
                "https://product.hstatic.net/200000722513/product/r3m1_ac3aa0be001640e2873ff732d34617bc_2295901522e24ce399b8f5f07be51467_3ab2e4aca4434a9a84997283b79b5c3c_grande.png");

            await _specificationRepository.InsertAsync(new KeyboardSpecification
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
                "Logitech G502 HERO", "<p>Chuột gaming hiệu năng cao với cảm biến <strong>HERO 16K</strong> và thiết kế ergonomic.</p><ul><li>Cảm biến HERO 16K DPI</li><li>11 nút lập trình được</li><li>Hệ thống trọng lượng điều chỉnh</li><li>Đèn RGB tùy chỉnh</li><li>Polling rate 1000Hz</li><li>Thiết kế ergonomic thoải mái</li></ul>",
                150, true, DateTime.Now.AddDays(3),
                "https://product.hstatic.net/200000722513/product/10001_01736316d2b443d0838e5a0741434420_grande.png");

            await _specificationRepository.InsertAsync(new MouseSpecification
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
        }

        private async Task SeedMonitorsAsync()
        {
            var monitor1 = await CreateProductAsync(
                _categories["Monitors"].Id, _manufacturers["LG"].Id, 7000000, 6,
                "UltraGear 27GL850", "<p>Màn hình gaming <strong>27 inch QHD</strong> với tần số quét <strong>144Hz</strong> và tấm nền IPS.</p><ul><li>Kích thước 27 inch lý tưởng</li><li>Độ phân giải 2560x1440 (QHD)</li><li>Tần số quét 144Hz mượt mà</li><li>Tấm nền IPS màu sắc chính xác</li><li>Thời gian phản hồi 1ms</li><li>Hỗ trợ G-Sync/FreeSync</li><li>Gắn VESA tiện lợi</li></ul>",
                30, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/lg_27gx790a-b_gearvn_18880ec6e5a944c2b29c76d85d44d243_medium.jpg");

            await _specificationRepository.InsertAsync(new MonitorSpecification
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
                "Logitech G Pro X", "<p>Tai nghe gaming chất lượng cao với <strong>micro Blue VO!CE</strong> và driver <strong>PRO-G 50mm</strong>.</p><ul><li>Driver PRO-G 50mm chất lượng cao</li><li>Micro Blue VO!CE chống ồn</li><li>Âm thanh vòm DTS Headphone:X 2.0</li><li>Đệm tai memory foam thoải mái</li><li>Khung nhôm và thép bền bỉ</li><li>Dây cáp tháo rời tiện lợi</li></ul>",
                70, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/gvn_logitech_prox_79c556630c454086baf1bee06c577ab7_3471d9d886fd4dbe8ab5ae6bed9f4d78_grande.png");

            await _specificationRepository.InsertAsync(new HeadsetSpecification
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
