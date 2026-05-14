using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Data.BaseSeeder;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Specifications.Junctions;
using Acme.ProductSelling.Specifications.Models;
using Acme.ProductSelling.Specifications.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Data.Products
{
    /// <summary>
    /// Seeder for cooling and power components
    /// </summary>
    public class CoolingSeeder : ProductSeederBase, IDataSeederContributor
    {
        private readonly IRepository<CpuCoolerSocketSupport> _cpuCoolerSocketSupportRepository;
        private readonly IRepository<CaseMaterial> _caseMaterialRepository;

        private readonly ISpecificationRepository _specificationRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;
        private Dictionary<string, FormFactor> _formFactors;
        private Dictionary<string, CpuSocket> _sockets;
        private Dictionary<string, Material> _materials;

        public CoolingSeeder(
            IProductRepository productRepository,
            ProductManager productManager,
            IRepository<CpuCoolerSocketSupport> cpuCoolerSocketSupportRepository,
            IRepository<CaseMaterial> caseMaterialRepository,
            ISpecificationRepository specificationRepository)
            : base(productRepository, productManager)
        {
            _cpuCoolerSocketSupportRepository = cpuCoolerSocketSupportRepository;
            _caseMaterialRepository = caseMaterialRepository;
            _specificationRepository = specificationRepository;
        }

        public void SetDependencies(
            Dictionary<string, Category> categories,
            Dictionary<string, Manufacturer> manufacturers,
            Dictionary<string, FormFactor> formFactors,
            Dictionary<string, CpuSocket> sockets,
            Dictionary<string, Material> materials)
        {
            _categories = categories;
            _manufacturers = manufacturers;
            _formFactors = formFactors;
            _sockets = sockets;
            _materials = materials;
        }

        public async Task SeedAsync()
        {
            await SeedPSUsAsync();
            await SeedCasesAsync();
            await SeedCpuCoolersAsync();
            await SeedCaseFansAsync();
        }

        private async Task SeedPSUsAsync()
        {
            var psu1 = await CreateProductAsync(
                _categories["PSUs"].Id, _manufacturers["Corsair"].Id, 2000000, 5,
                "Corsair RM750x", "<p>Nguồn máy tính <strong>750W</strong> hiệu suất cao với chứng nhận <strong>80 Plus Gold</strong>.</p><ul><li>Công suất 750W ổn định</li><li>Hiệu suất 80 Plus Gold (>90%)</li><li>Fully modular - dễ dàng quản lý dây</li><li>Quạt tản nhiệt 135mm êm ái</li><li>Bảo hành 10 năm</li></ul>",
                50, true, DateTime.Now,
                "https://bizweb.dktcdn.net/thumb/grande/100/329/122/products/nguon-may-tinh-corsair-rm750x-shift-750w-80-plus-gold-cp-9020251-na-04-20838ea6-b253-460f-bb0c-ad9327565373.jpg?v=1743639588677");

            await _specificationRepository.InsertAsync(new PsuSpecification
            {
                ProductId = psu1.Id,
                Wattage = 750,
                EfficiencyRating = "80 Plus Gold",
                Modularity = PsuModularity.FullModularity,
                FormFactorId = _formFactors["ATX"].Id
            }, autoSave: true);

            var psu2 = await CreateProductAsync(
                _categories["PSUs"].Id, _manufacturers["EVGA"].Id, 2200000, 20,
                "EVGA SuperNOVA 750 G5", "<p>Nguồn <strong>750W</strong> fully modular với chứng nhận <strong>80 Plus Gold</strong> từ EVGA.</p><ul><li>Công suất 750W</li><li>Hiệu suất 80 Plus Gold</li><li>Fully modular giúp gọn gàng</li><li>Quạt Fluid Dynamic Bearing êm</li><li>Bảo hành 10 năm</li></ul>",
                35, true, DateTime.Now,
                "https://tandoanh.vn/wp-content/uploads/2021/10/EVGA-SuperNOVA-750-G1-%E2%80%93-80-GOLD-750W-%E2%80%93-Fully-Modular-h1.jpg");

            await _specificationRepository.InsertAsync(new PsuSpecification
            {
                ProductId = psu2.Id,
                Wattage = 750,
                EfficiencyRating = "80 Plus Gold",
                Modularity = PsuModularity.FullModularity,
                FormFactorId = _formFactors["ATX"].Id
            }, autoSave: true);
        }

        private async Task SeedCasesAsync()
        {
            var case1 = await CreateProductAsync(
                _categories["Cases"].Id, _manufacturers["NZXT"].Id, 1500000, 5,
                "NZXT H510", "<p>Vỏ case <strong>Mid-tower ATX</strong> với thiết kế tối giản và kính cường lực.</p><ul><li>Thiết kế tối giản, hiện đại</li><li>Kính cường lực tempered glass</li><li>Hỗ trợ lên đến 6 quạt tản nhiệt</li><li>Radiator lên đến 360mm</li><li>Quản lý dây gọn gàng</li><li>Cổng USB 3.0, USB-C phía trước</li></ul>",
                40, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/4108_be554d73268e3ca69f25d192629df397_b7fd1aebb5f74f50ae18c3b23efb8755_b6d80711bb304b568b03fdcf3e94c1ab_grande.jpg");

            var spec_case1 = await _specificationRepository.InsertAsync(new CaseSpecification
            {
                ProductId = case1.Id,
                FormFactorId = _formFactors["ATX Mid Tower"].Id,
                CoolingSupport = "Up to 6 fans",
                RadiatorSupport = "Up to 360mm",
                DriveBays = "3 x 3.5\" + 2 x 2.5\"",
                FrontPanelPorts = "USB 3.0, USB-C, Audio",
                MaxGpuLength = 400,
                MaxCpuCoolerHeight = 160,
                MaxPsuLength = 200,
                Color = "Black",
                FanSupport = "Up to 6 x 120mm or 4 x 140mm"
            }, autoSave: true);

            await _caseMaterialRepository.InsertAsync(
                new CaseMaterial { CaseSpecificationId = spec_case1.Id, MaterialId = _materials["Steel"].Id },
                autoSave: true);
            await _caseMaterialRepository.InsertAsync(
                new CaseMaterial { CaseSpecificationId = spec_case1.Id, MaterialId = _materials["Tempered Glass"].Id },
                autoSave: true);

            var case2 = await CreateProductAsync(
                _categories["Cases"].Id, _manufacturers["Phanteks"].Id, 1800000, 10,
                "Phanteks Eclipse P400A", "<p>Vỏ case <strong>Mid-tower ATX</strong> với luồng khí tối ưu cho hiệu năng làm mát tốt nhất.</p><ul><li>Thiết kế high-airflow</li><li>Mặt trước lưới thoáng khí</li><li>Kính cường lực tempered glass</li><li>Hỗ trợ radiator 360mm</li><li>Cổng USB 3.0, USB-C</li><li>Màu trắng sang trọng</li></ul>",
                45, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/k-_1_65d8edfddc2b4785af9a13f971fc258a_6043347819ed417bb6dd327b41b39b6e_60a930dd805e4bc891b6ea69e7c2d21a_grande.jpg");

            var spec_case2 = await _specificationRepository.InsertAsync(new CaseSpecification
            {
                ProductId = case2.Id,
                FormFactorId = _formFactors["ATX Mid Tower"].Id,
                CoolingSupport = "Up to 6 fans",
                RadiatorSupport = "Up to 360mm",
                DriveBays = "2 x 3.5\" + 2 x 2.5\"",
                FrontPanelPorts = "USB 3.0, USB-C, Audio",
                MaxGpuLength = 355,
                MaxCpuCoolerHeight = 160,
                MaxPsuLength = 220,
                Color = "White",
                FanSupport = "Up to 6 x 120mm"
            }, autoSave: true);

            await _caseMaterialRepository.InsertAsync(
                new CaseMaterial { CaseSpecificationId = spec_case2.Id, MaterialId = _materials["Steel"].Id },
                autoSave: true);
            await _caseMaterialRepository.InsertAsync(
                new CaseMaterial { CaseSpecificationId = spec_case2.Id, MaterialId = _materials["Tempered Glass"].Id },
                autoSave: true);
        }

        private async Task SeedCpuCoolersAsync()
        {
            var cooler1 = await CreateProductAsync(
                _categories["CPU Coolers"].Id, _manufacturers["Cooler Master"].Id, 1200000, 5,
                "Cooler Master Hyper 212", "<p>Tản nhiệt khí CPU phổ biến với <strong>đèn RGB</strong> và hiệu năng làm mát tốt.</p><ul><li>Quạt 120mm với đèn RGB</li><li>Hỗ trợ TDP lên đến 150W</li><li>Độ ồn thấp chỉ 30dB</li><li>Tương thích socket AM4, LGA1200</li><li>Chiều cao 160mm</li><li>Giá trị tốt nhất trong phân khúc</li></ul>",
                60, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/hyper-212-argb-gallery-4-image_dc19349414e94e0e869c23e85c70cb49_d2713cd5bac947da94ee34d1456220fe_grande.png");

            var spec_cooler1 = await _specificationRepository.InsertAsync(new CpuCoolerSpecification
            {
                ProductId = cooler1.Id,
                CoolerType = "Air",
                FanSize = 120,
                TdpSupport = 150,
                NoiseLevel = 30,
                Color = "Black",
                LedLighting = "RGB",
                Height = 160
            }, autoSave: true);

            await _cpuCoolerSocketSupportRepository.InsertAsync(
                new CpuCoolerSocketSupport { CpuCoolerSpecificationId = spec_cooler1.Id, SocketId = _sockets["AM4"].Id },
                autoSave: true);
            await _cpuCoolerSocketSupportRepository.InsertAsync(
                new CpuCoolerSocketSupport { CpuCoolerSpecificationId = spec_cooler1.Id, SocketId = _sockets["LGA1200"].Id },
                autoSave: true);

            var cooler2 = await CreateProductAsync(
                _categories["CPU Coolers"].Id, _manufacturers["Noctua"].Id, 4000000, 25,
                "Noctua NH-D15", "<p>Tản nhiệt khí cao cấp <strong>dual-tower</strong> với hiệu năng làm mát <strong>vượt trội</strong>.</p><ul><li>Thiết kế dual-tower với 2 quạt 140mm</li><li>Hỗ trợ TDP lên đến 220W</li><li>Cực kỳ êm ái chỉ 24dB</li><li>Chất lượng Noctua hàng đầu</li><li>Tương thích socket AM4, LGA1700</li><li>Bảo hành 6 năm</li></ul>",
                20, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/noctua_nh-d15_2_75940b3d5fbb485190327d6b592429af_9ad735dcdbb94a71ba171d7d4ae0a326_grande.jpg");

            var spec_cooler2 = await _specificationRepository.InsertAsync(new CpuCoolerSpecification
            {
                ProductId = cooler2.Id,
                CoolerType = "Air",
                FanSize = 140,
                TdpSupport = 220,
                NoiseLevel = 24,
                Color = "Beige/Brown",
                LedLighting = "None",
                Height = 165
            }, autoSave: true);

            await _cpuCoolerSocketSupportRepository.InsertAsync(
                new CpuCoolerSocketSupport { CpuCoolerSpecificationId = spec_cooler2.Id, SocketId = _sockets["AM4"].Id },
                autoSave: true);
            await _cpuCoolerSocketSupportRepository.InsertAsync(
                new CpuCoolerSocketSupport { CpuCoolerSpecificationId = spec_cooler2.Id, SocketId = _sockets["LGA1700"].Id },
                autoSave: true);
        }

        private async Task SeedCaseFansAsync()
        {
            var fan1 = await CreateProductAsync(
                _categories["Case Fans"].Id, _manufacturers["Noctua"].Id, 450000, 5,
                "Noctua NF-A12x25 PWM", "<p>Quạt tản nhiệt case <strong>120mm</strong> cao cấp với hiệu năng và độ êm <strong>hàng đầu</strong>.</p><ul><li>Kích thước 120mm chuẩn</li><li>Tốc độ tối đa 2000 RPM</li><li>Độ ồn cực thấp 22.6 dBA</li><li>Lưu lượng gió 102.1 CFM</li><li>Áp suất tĩnh 2.34 mmH₂O</li><li>Ổ trục SSO2 bền bỉ</li><li>Bảo hành 6 năm</li></ul>",
                100, true, DateTime.Now.AddDays(2),
                "https://noctua.at/pub/media/catalog/product/cache/74c1057f7991b4edb2bc7bdaa94de933/n/f/nf_a12x25_pwm_1.jpg");

            await _specificationRepository.InsertAsync(new CaseFanSpecification
            {
                ProductId = fan1.Id,
                FanSize = 120,
                MaxRpm = 2000,
                NoiseLevel = 22.6f,
                Airflow = 102.1f,
                StaticPressure = 2.34f,
                Connector = "4-pin PWM",
                BearingType = BearingType.SSO2,
                HasRgb = false,
                Color = "Brown/Beige"
            }, autoSave: true);
        }
    }
}
