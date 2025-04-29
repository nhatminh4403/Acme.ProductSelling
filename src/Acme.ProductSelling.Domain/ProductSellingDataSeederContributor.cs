using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Specifications;
using System;
using System.Collections.Generic;
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

        private readonly IRepository<LaptopSpecification, Guid> _laptopSpecRepository;
        private readonly IRepository<MonitorSpecification, Guid> _monitorSpecRepository;
        private readonly IRepository<MouseSpecification, Guid> _mouseSpecRepository;
        private readonly IRepository<CpuSpecification, Guid> _cpuSpecRepository;
        private readonly IRepository<GpuSpecification, Guid> _gpuSpecRepository;
        private readonly IRepository<RamSpecification, Guid> _ramSpecRepository;
        private readonly IRepository<MotherboardSpecification, Guid> _motherboardSpecRepository;
        private readonly IRepository<StorageSpecification, Guid> _storageSpecRepository;
        private readonly IRepository<PsuSpecification, Guid> _psuSpecRepository;
        private readonly IRepository<CaseSpecification, Guid> _caseSpecRepository;
        private readonly IRepository<CpuCoolerSpecification, Guid> _cpuCoolerSpecRepository;
        private readonly IRepository<KeyboardSpecification, Guid> _keyboardSpecRepository;
        private readonly IRepository<HeadsetSpecification, Guid> _headsetSpecRepository;
        private readonly IGuidGenerator _guidGenerator;
        public ProductSellingDataSeederContributor(
            IRepository<Product, Guid> productRepository,
            IRepository<Category, Guid> categoryRepository, CategoryManager categoryManager,
            IRepository<MonitorSpecification, Guid> monitorSpecRepository,
            IRepository<MouseSpecification, Guid> mouseSpecRepository,
            IRepository<CpuSpecification, Guid> cpuSpecRepository,
            IRepository<GpuSpecification, Guid> gpuSpecRepository,
            IRepository<RamSpecification, Guid> ramSpecRepository,
            IRepository<MotherboardSpecification, Guid> motherboardSpecRepository,
            IRepository<StorageSpecification, Guid> storageSpecRepository,
            IRepository<PsuSpecification, Guid> psuSpecRepository,
            IRepository<CaseSpecification, Guid> caseSpecRepository,
            IRepository<CpuCoolerSpecification, Guid> cpuCoolerSpecRepository,
            IRepository<KeyboardSpecification, Guid> keyboardSpecRepository,
            IRepository<HeadsetSpecification, Guid> headsetSpecRepository,
                     IRepository<LaptopSpecification, Guid> laptopSpecRepository,
             IGuidGenerator guidGenerator)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _categoryManager = categoryManager;
            _monitorSpecRepository = monitorSpecRepository;
            _mouseSpecRepository = mouseSpecRepository;
            _cpuSpecRepository = cpuSpecRepository;
            _gpuSpecRepository = gpuSpecRepository;
            _ramSpecRepository = ramSpecRepository;
            _motherboardSpecRepository = motherboardSpecRepository;
            _storageSpecRepository = storageSpecRepository;
            _psuSpecRepository = psuSpecRepository;
            _caseSpecRepository = caseSpecRepository;
            _cpuCoolerSpecRepository = cpuCoolerSpecRepository;
            _keyboardSpecRepository = keyboardSpecRepository;
            _headsetSpecRepository = headsetSpecRepository;
            _laptopSpecRepository = laptopSpecRepository;
            _guidGenerator = guidGenerator;
        }
        public async Task SeedAsync(DataSeedContext context)
        {
            // --- Seed Categories ---
            // Chỉ seed nếu chưa có Category nào trong DB
            if (await _categoryRepository.GetCountAsync() > 0)
            {
                return; // Không cần seed nếu đã có Category
            }

            var monitors = await _categoryRepository.InsertAsync(
                                                     await _categoryManager.CreateAsync("Monitors", "Displays", SpecificationType.Monitor));

            var mice = await _categoryRepository.InsertAsync(
                                                     await _categoryManager.CreateAsync("Mice", "Pointing devices", SpecificationType.Mouse));

            var keyboards = await _categoryRepository.InsertAsync(
                                                        await _categoryManager.CreateAsync("Keyboards", "Input devices", SpecificationType.Keyboard));
            var laptops = await _categoryRepository.InsertAsync(
                                                        await _categoryManager.CreateAsync("Laptops", "Portable computers", SpecificationType.Laptop));
            var cpus = await _categoryRepository.InsertAsync(
                                                        await _categoryManager.CreateAsync("CPUs", "Central Processing Units", SpecificationType.CPU));
            var gpus = await _categoryRepository.InsertAsync(
                                                        await _categoryManager.CreateAsync("GPUs", "Graphics Cards", SpecificationType.GPU));
            var rams = await _categoryRepository.InsertAsync(
                                                        await _categoryManager.CreateAsync("RAM", "Memory Modules", SpecificationType.RAM));
            var motherboards = await _categoryRepository.InsertAsync(
                                                        await _categoryManager.CreateAsync("Motherboards", "Mainboards", SpecificationType.Motherboard));
            var storage = await _categoryRepository.InsertAsync(
                                                        await _categoryManager.CreateAsync("Storage", "SSDs and HDDs", SpecificationType.Storage));
            var psus = await _categoryRepository.InsertAsync(
                                                        await _categoryManager.CreateAsync("PSUs", "Power Supply Units", SpecificationType.PSU));
            var cases = await _categoryRepository.InsertAsync(
                                                        await _categoryManager.CreateAsync("Cases", "Computer Chassis", SpecificationType.Case));
            var coolers = await _categoryRepository.InsertAsync(
                                                        await _categoryManager.CreateAsync("CPU Coolers", "Cooling Solutions", SpecificationType.CPUCooler));
            var headsets = await _categoryRepository.InsertAsync(
                                                        await _categoryManager.CreateAsync("Headsets", "Audio Devices", SpecificationType.Headset));


            var cpuSpec1 = new CpuSpecification
            {
                Socket = "AM5",
                CoreCount = 8,
                ThreadCount = 16,
                BaseClock = 4.5f,
                BoostClock = 5.4f,
                L3Cache = 32,
                Tdp = 105,
                HasIntegratedGraphics = true
            };
            await _cpuSpecRepository.InsertAsync(cpuSpec1, autoSave: true); // Lưu spec trước
                                                                            //var productCpu1 = new Product(_guidGenerator.Create(), "AMD Ryzen 7 7700X", "Powerful 8-core CPU", 8500000m, 50, cpus.Id)

            var productCpu1 = await _productRepository.InsertAsync(
                 new Product
                 {
                     CategoryId = cpus.Id,
                     Price = 8500000,
                     ProductName = "AMD Ryzen 7 7700X",
                     Description = "Powerful 8-core CPU",
                     StockCount = 50,
                     CpuSpecificationId = cpuSpec1.Id,
                     ImageUrl = "https://product.hstatic.net/200000722513/product/ryzen_7_-_1_00957bbe7b8542308c897a90d439b1fd_e1c9a16c537d47bb9768828dddb332d0_grande.jpg",
                 },
                 autoSave: true
             );

            var gpuSpec1 = new GpuSpecification
            {
                Chipset = "GeForce RTX 4070 Ti",
                MemorySize = 12,
                MemoryType = "GDDR6X",
                BoostClock = 2610f,
                Interface = "PCIe 4.0 x16",
                RecommendedPsu = 700,
                Length = 310f
            };
            await _gpuSpecRepository.InsertAsync(gpuSpec1, autoSave: true); // Lưu spec trước
            var productGpu1 = new Product
            {
                Price = 25000000,
                ProductName = "ASUS ProArt GeForce RTX 4070 Ti SUPER 16GB GDDR6X OC Edition (PROART-RTX4070TIS-O16G)",
                Description = "High-end graphics card",
                StockCount = 20,
                CategoryId = gpus.Id,
                ImageUrl = "https://product.hstatic.net/200000722513/product/fwebp__10__1d22cf39c094494bb772b5bb1c002172_grande.png",
                // Gán FK cho spe
                GpuSpecificationId = gpuSpec1.Id // Gán FK

            };
            await _productRepository.InsertAsync(productGpu1, autoSave: true);
            var ramSpec1 = new RamSpecification
            {
                RamType = "DDR5",
                Capacity = 32,
                Speed = 6000,
                ModuleCount = 2,
                Timing = "36-36-36-76",
                Voltage = 1.35f,
                HasRGB = true
            };
            await _ramSpecRepository.InsertAsync(ramSpec1, autoSave: true); // Lưu spec trước
            var productRam1 = new Product
            {
                Price = 2000000,
                ProductName = "Corsair Vengeance LPX 16GB",
                Description = "High-performance RAM",
                StockCount = 100,
                CategoryId = rams.Id,
                ImageUrl = "https://product.hstatic.net/200000722513/product/gearvn-corsair-vengeance-rgb-ddr-5600-ddr5-5_6e319950a8e14231b28a416076c94951_grande.png",
                // Gán FK cho spec
                RamSpecificationId = ramSpec1.Id // Gán FK
            };
            await _productRepository.InsertAsync(productRam1, autoSave: true);

            var storageSpec1 = new StorageSpecification
            {
                StorageType = "NVMe SSD",
                Interface = "PCIe 4.0 x4",
                Capacity = 1000,
                ReadSpeed = 7000,
                WriteSpeed = 5000,
                FormFactor = "M.2 2280"
            };

            await _storageSpecRepository.InsertAsync(storageSpec1, autoSave: true); // Lưu spec trước
            var productStorage1 = new Product
            {
                Price = 3000000,
                ProductName = "Samsung 970 EVO Plus 1TB",
                Description = "Fast NVMe SSD",
                StockCount = 80,
                CategoryId = storage.Id,
                ImageUrl = "https://bizweb.dktcdn.net/thumb/grande/100/329/122/products/970-evo-plus-1tb-01-1689929004911.jpg?v=1695052940103",
                // Gán FK cho spec
                StorageSpecificationId = storageSpec1.Id // Gán FK
            };

            await _productRepository.InsertAsync(productStorage1, autoSave: true);
            var keyboardSpec1 = new KeyboardSpecification
            {
                KeyboardType = "Mechanical",
                SwitchType = "Cherry MX Brown",
                Layout = "TKL",
                Connectivity = "Wired",
                Backlight = "RGB"
            };
            await _keyboardSpecRepository.InsertAsync(keyboardSpec1, autoSave: true); // Lưu spec trước
            var keyboardProduct1 = new Product
            {
                Price = 3000000,
                ProductName = "Logitech G Pro TKL Keyboard",
                ImageUrl = "https://product.hstatic.net/200000722513/product/1_5b2f7891bf434a7aab9f1abdba56c17e_grande.jpg",
                Description = "Tenkeyless mechanical gaming keyboard",
                StockCount = 80,
                CategoryId = keyboards.Id,
                // Gán FK cho spec
                KeyboardSpecificationId = keyboardSpec1.Id // Gán FK
            };
            await _productRepository.InsertAsync(keyboardProduct1, autoSave: true);


            var mouseSpec1 = new MouseSpecification
            {
                Dpi = 16000,
                PollingRate = 1000,
                SensorType = "Optical",
                Weight = 80,
                Connectivity = "Wired",
                ButtonCount = 6,
                BacklightColor = "RGB",
                Color = "Black"
            };
            await _mouseSpecRepository.InsertAsync(mouseSpec1, autoSave: true);
            var productMouse1 = new Product
            {
                Price = 1000000,
                ProductName = "Logitech G502 HERO",
                Description = "High-performance gaming mouse",
                StockCount = 150,
                CategoryId = mice.Id,
                // Gán FK cho spec
                MouseSpecificationId = mouseSpec1.Id,
                ImageUrl = "https://product.hstatic.net/200000722513/product/10001_01736316d2b443d0838e5a0741434420_grande.png"
            };
            await _productRepository.InsertAsync(productMouse1, autoSave: true);

            var monitorSpec1 = new MonitorSpecification
            {
                ScreenSize = 27,
                Resolution = "2560x1440",
                RefreshRate = 144,
                PanelType = "IPS",
                ResponseTime = 1,
                ColorGamut = "sRGB 99%",
                Brightness = 400,
                VesaMount = true,
                ResponseTimeMs = 1
            };
            await _monitorSpecRepository.InsertAsync(monitorSpec1, autoSave: true); // Lưu spec trước
            var productMonitor1 = new Product
            {
                Price = 7000000,
                ProductName = "LG UltraGear 27GL850",
                Description = "27-inch QHD gaming monitor",
                StockCount = 30,
                CategoryId = monitors.Id,
                // Gán FK cho spec
                MonitorSpecificationId = monitorSpec1.Id,
                ImageUrl = "https://product.hstatic.net/200000722513/product/lg_27gx790a-b_gearvn_18880ec6e5a944c2b29c76d85d44d243_medium.jpg"
            };
            var psuSpec1 = new PsuSpecification
            {
                Wattage = 750,
                EfficiencyRating = "80 Plus Gold",
                Modularity = "Fully Modular",
                FormFactor = "ATX",
            };
            await _psuSpecRepository.InsertAsync(psuSpec1, autoSave: true); // Lưu spec trước
            var productPsu1 = new Product
            {
                Price = 2000000,
                ProductName = "Corsair RM750x",
                Description = "750W fully modular power supply",
                StockCount = 50,
                CategoryId = psus.Id,
                // Gán FK cho spec
                PsuSpecificationId = psuSpec1.Id,
                ImageUrl = "https://bizweb.dktcdn.net/thumb/grande/100/329/122/products/nguon-may-tinh-corsair-rm750x-shift-750w-80-plus-gold-cp-9020251-na-04-20838ea6-b253-460f-bb0c-ad9327565373.jpg?v=1743639588677"

            };
            await _productRepository.InsertAsync(productPsu1, autoSave: true);

            var caseSpec1 = new CaseSpecification
            {
                SupportedMbFormFactor = "ATX Mid Tower",
                CoolingSupport = "Up to 6 fans",
                RadiatorSupport = "Up to 360mm",
                DriveBays = "3 x 3.5\" + 2 x 2.5\"",
                FrontPanelPorts = "USB 3.0, USB-C, Audio",
                MaxGpuLength = 400,
                MaxCpuCoolerHeight = 160,
                MaxPsuLength = 200,
                Material = "Steel, Tempered Glass",
                Color = "Black",
                FanSupport = "Up to 6 x 120mm or 4 x 140mm",

            };
            await _caseSpecRepository.InsertAsync(caseSpec1, autoSave: true); // Lưu spec trước
            var productCase1 = new Product
            {
                Price = 1500000,
                ProductName = "NZXT H510",
                Description = "Mid-tower ATX case with tempered glass",
                StockCount = 40,
                CategoryId = cases.Id,
                // Gán FK cho spec
                CaseSpecificationId = caseSpec1.Id,
                ImageUrl = "https://product.hstatic.net/200000722513/product/4108_be554d73268e3ca69f25d192629df397_b7fd1aebb5f74f50ae18c3b23efb8755_b6d80711bb304b568b03fdcf3e94c1ab_grande.jpg"
            };
            await _productRepository.InsertAsync(productCase1, autoSave: true);
            var cpuCoolerSpec1 = new CpuCoolerSpecification
            {
                CoolerType = "Air",
                FanSize = 120,
                TdpSupport = 150,
                NoiseLevel = 30,
                Color = "Black",
                LedLighting = "RGB",
                Height = 160,
                RadiatorSize = null,
                SupportedSockets = "AM4, LGA1200",
            };
            await _cpuCoolerSpecRepository.InsertAsync(cpuCoolerSpec1, autoSave: true); // Lưu spec trước
            var productCpuCooler1 = new Product
            {
                Price = 1200000,
                ProductName = "Cooler Master Hyper 212",
                Description = "Air cooler with RGB lighting",
                StockCount = 60,
                CategoryId = coolers.Id,
                // Gán FK cho spec
                CpuCoolerSpecificationId = cpuCoolerSpec1.Id,
                ImageUrl = "https://product.hstatic.net/200000722513/product/hyper-212-argb-gallery-4-image_dc19349414e94e0e869c23e85c70cb49_d2713cd5bac947da94ee34d1456220fe_grande.png"
            };

            var headsetSpec1 = new HeadsetSpecification
            {
                DriverSize = 50,
                HasMicrophone = true,
                IsNoiseCancelling = true,
                IsSurroundSound = true,
                Frequency = "20Hz - 20kHz",
                Connectivity = "Wired",
                MicrophoneType = "Omnidirectional",
                Impedance = 32,
                Sensitivity = 100,
                Color = "Black"
            };
            await _headsetSpecRepository.InsertAsync(headsetSpec1, autoSave: true); // Lưu spec trước
            var productHeadset1 = new Product
            {
                Price = 1500000,
                ProductName = "Logitech G Pro X",
                Description = "High-quality gaming headset",
                StockCount = 70,
                CategoryId = headsets.Id,
                // Gán FK cho spec
                HeadsetSpecificationId = headsetSpec1.Id,
                ImageUrl = "https://product.hstatic.net/200000722513/product/gvn_logitech_prox_79c556630c454086baf1bee06c577ab7_3471d9d886fd4dbe8ab5ae6bed9f4d78_grande.png"
            };
            await _productRepository.InsertAsync(productHeadset1, autoSave: true);

            var laptopSpec1 = new LaptopSpecification
            {
                //ScreenSize = 15.6f,
                //Resolution = "1920x1080",
                //Processor = "Intel Core i7-12700H",
                //RamSize = 16,
                //StorageType = "SSD",
                //StorageCapacity = 512,
                //GpuType = "Integrated",
                BatteryLife = "3 hours 30 minutes",
                Weight = "1.95 Kg (4.30 lbs)",
                //Color = "Black",
                Warranty = "1 year",
                Storage = "NVMe SSD",
                /*                CoolingSystem = "Air",
                                Ports = "USB-C, HDMI, Audio Jack",*/
                GraphicsCard = "Intel® Core™ Ultra 9",
                CPU = "Intel® Core™ Ultra 9",
                /*                ScreenType = "IPS",
                                ScreenRefreshRate = 144,
                                ScreenBrightness = 300,
                                ScreenAspectRatio = "16:9",
                                ScreenColorGamut = "sRGB 100%",
                                ScreenResponseTime = 3,*/
                Display = "16-inch 2.5K (2560 x 1600, WQXGA) 16:10 OLED, 240Hz, 0.2ms, 500 nits, 100% DCI-P3, Glossy display, G-Sync / Adaptive-Sync, ROG Nebula Display",
                /*                ScreenContrastRatio = "1000:1",
                                ScreenColorDepth = 8,
                                ScreenViewingAngle = 178,
                                ScreenHDR = "HDR 400",
                                ScreenAdaptiveSync = "G-Sync Compatible",
                                ScreenFlickerFree = true,
                                ScreenBlueLightFilter = true,
                                ScreenTouch = false,
                                ScreenAntiReflective = true,
                                ScreenAntiGlare = true,*/
                RAM = "64GB LPDDR5X 7467 on board",
                OperatingSystem = "Windows 11 Home",

            };
            await _laptopSpecRepository.InsertAsync(laptopSpec1, autoSave: true); // Lưu spec trước
            var productLaptop1 = new Product
            {
                Price = 25000000,
                ProductName = "ASUS ROG Zephyrus G16 GU605CX QR083W",
                Description = "Powerful gaming laptop",
                StockCount = 10,
                CategoryId = laptops.Id,
                // Gán FK cho spec
                LaptopSpecificationId = laptopSpec1.Id,
                ImageUrl = "https://product.hstatic.net/200000722513/product/zephyrus_g16_gu605_grey_03_rgb_1_b58d513a9306445daf2980232fe2544b_grande.png"
            };
            await _productRepository.InsertAsync(productLaptop1, autoSave: true);

            var intelCpuSpec = new CpuSpecification
            {
                Socket = "LGA1700",
                CoreCount = 24,
                ThreadCount = 32,
                BaseClock = 3.0f,
                BoostClock = 5.8f,
                L3Cache = 36,
                Tdp = 125,
                HasIntegratedGraphics = true
            };
            await _cpuSpecRepository.InsertAsync(intelCpuSpec, autoSave: true);
            var productCpu2 = new Product
            {
                CategoryId = cpus.Id,
                CpuSpecificationId = intelCpuSpec.Id,
                ProductName = "Intel Core i9-13900K",
                Description = "Top-tier 24-core CPU",
                Price = 13500000,
                StockCount = 30,
                ImageUrl = "https://product.hstatic.net/200000722513/product/i9k-t2-special-box-07-1080x1080pixels_6c9ec1001cdf4e4998c13af4ac6c7581_114c47698e4a4984863c3b26e0619b65_grande.png"

            };
            await _productRepository.InsertAsync(productCpu2, autoSave: true);

            // 2. GPU: AMD Radeon RX 7900 XT
            var amdGpuSpec = new GpuSpecification
            {
                Chipset = "Radeon RX 7900 XT",
                MemorySize = 20,
                MemoryType = "GDDR6",
                BoostClock = 2600f,
                Interface = "PCIe 4.0 x16",
                RecommendedPsu = 750,
                Length = 310f
            };
            await _gpuSpecRepository.InsertAsync(amdGpuSpec, autoSave: true);
            var productGpu2 = new Product
            {
                CategoryId = gpus.Id,
                GpuSpecificationId = amdGpuSpec.Id,
                ProductName = "Asus Radeon RX 7900 XT TUF Gaming",
                Description = "High-end AMD graphics card",
                Price = 28000000,
                StockCount = 15,
                ImageUrl = " https://product.hstatic.net/200000722513/product/5681_ea11053c19e375dcaa8138b6f531262d_7d029f536978405393da9fb3c8f1e2fa_4d3cedb8fd4a485db1ece7519c1d41a8_grande.jpg"
            };
            await _productRepository.InsertAsync(productGpu2, autoSave: true);
            // 3. RAM: G.Skill Trident Z RGB 16GB (2×8)
            var ramSpec2 = new RamSpecification
            {
                RamType = "DDR4",
                Capacity = 16,
                Speed = 3600,
                ModuleCount = 2,
                Timing = "18-22-22-42",
                Voltage = 1.35f,
                HasRGB = true
            };
            await _ramSpecRepository.InsertAsync(ramSpec2, autoSave: true);
            var productRam2 = new Product
            {
                CategoryId = rams.Id,
                RamSpecificationId = ramSpec2.Id,
                ProductName = "G.Skill Trident Z RGB 16GB (2×8)",
                Description = "DDR4 3600MHz kit with RGB",
                Price = 1800000,
                StockCount = 80,
                ImageUrl = "https://anphat.com.vn/media/product/33685_153665426813.png"
            };
            await _productRepository.InsertAsync(productRam2, autoSave: true);

            // 4. Storage: WD Black SN770 1TB
            var storageSpec2 = new StorageSpecification
            {
                StorageType = "NVMe SSD",
                Interface = "PCIe 4.0 x4",
                Capacity = 1000,
                ReadSpeed = 5150,
                WriteSpeed = 4900,
                FormFactor = "M.2 2280"
            };
            await _storageSpecRepository.InsertAsync(storageSpec2, autoSave: true);
            var productStorage2 = new Product
            {
                CategoryId = storage.Id,
                StorageSpecificationId = storageSpec2.Id,
                ProductName = "WD Black SN770 1TB",
                Description = "High-performance PCIe 4.0 NVMe SSD",
                Price = 2500000,
                StockCount = 60,
                ImageUrl = "https://bizweb.dktcdn.net/thumb/grande/100/329/122/products/ssd-wd-black-sn770-pcie-gen4-x4-nvme-m-2-500gb-wds500g3x0e-b058273a-af63-4053-ac31-83b41eb593a2.jpg?v=1655710957737"
            };
            await _productRepository.InsertAsync(productStorage2, autoSave: true);
            // 5. Keyboard: Razer Huntsman Elite
            var keyboardSpec2 = new KeyboardSpecification
            {
                KeyboardType = "Mechanical",
                SwitchType = " Linear Red Switch",
                Layout = "Full-size",
                Connectivity = "Wired",
                Backlight = "RGB"
            };
            await _keyboardSpecRepository.InsertAsync(keyboardSpec2, autoSave: true);
            var keyboardProduct2 = new Product
            {
                CategoryId = keyboards.Id,
                KeyboardSpecificationId = keyboardSpec2.Id,
                ProductName = "Razer Huntsman Elite",
                Description = "Opto-mechanical gaming keyboard",
                Price = 4500000,
                StockCount = 40,
                ImageUrl = "https://product.hstatic.net/200000722513/product/r3m1_ac3aa0be001640e2873ff732d34617bc_2295901522e24ce399b8f5f07be51467_3ab2e4aca4434a9a84997283b79b5c3c_grande.png"
            };
            await _productRepository.InsertAsync(keyboardProduct2, autoSave: true);

            // 6. Mouse: SteelSeries Rival 3
            var mouseSpec2 = new MouseSpecification
            {
                Dpi = 8500,
                PollingRate = 1000,
                SensorType = "Optical",
                Weight = 77,
                Connectivity = "Wired",
                ButtonCount = 6,
                BacklightColor = "RGB",
                Color = "White"
            };
            await _mouseSpecRepository.InsertAsync(mouseSpec2, autoSave: true);
            var productMouse2 = new Product
            {
                CategoryId = mice.Id,
                MouseSpecificationId = mouseSpec2.Id,
                ProductName = "SteelSeries Rival 3",
                Description = "Lightweight RGB gaming mouse",
                Price = 800000,
                StockCount = 120,
                ImageUrl = "https://product.hstatic.net/200000722513/product/thumbchuot_e01eec6957cc40a88aba550b80cffed2_74ec8f2dec0447c382614fa201a4fa93_grande.png"
            };
            await _productRepository.InsertAsync(productMouse2, autoSave: true);

            // 7. Monitor: ASUS TUF Gaming VG27AQ
            var monitorSpec2 = new MonitorSpecification
            {
                ScreenSize = 27,
                Resolution = "2560x1440",
                RefreshRate = 165,
                PanelType = "IPS",
                ResponseTime = 1,
                ColorGamut = "sRGB 99%",
                Brightness = 350,
                VesaMount = true,
                ResponseTimeMs = 1,
            };
            await _monitorSpecRepository.InsertAsync(monitorSpec2, autoSave: true);
            var productMonitor2 = new Product
            {
                CategoryId = monitors.Id,
                MonitorSpecificationId = monitorSpec2.Id,
                ProductName = "ASUS TUF Gaming VG27AQ",
                Description = "27” QHD IPS 165Hz gaming monitor",
                Price = 9000000,
                StockCount = 25,
                ImageUrl = "https://product.hstatic.net/200000722513/product/ips-2k-170hz-g-sync-hdr-chuyen-game-1_f9de14d5b20041b2b52b0cde6884a3d9_317538ed8cff45e6a25feb1cbb8650d0_grande.png"
            };
            await _productRepository.InsertAsync(productMonitor2, autoSave: true);

            // 8. PSU: EVGA SuperNOVA 750 G5
            var psuSpec2 = new PsuSpecification
            {
                Wattage = 750,
                EfficiencyRating = "80 Plus Gold",
                Modularity = "Fully Modular",
                FormFactor = "ATX",
            };
            await _psuSpecRepository.InsertAsync(psuSpec2, autoSave: true);
            var productPsu2 = new Product
            {
                CategoryId = psus.Id,
                PsuSpecificationId = psuSpec2.Id,
                ProductName = "EVGA SuperNOVA 750 G5",
                Description = "750W gold-rated fully modular PSU",
                Price = 2200000,
                StockCount = 35,
                ImageUrl = "https://tandoanh.vn/wp-content/uploads/2021/10/EVGA-SuperNOVA-750-G1-%E2%80%93-80-GOLD-750W-%E2%80%93-Fully-Modular-h1.jpg"
            };
            await _productRepository.InsertAsync(productPsu2, autoSave: true);
            // 9. Case: Phanteks Eclipse P400A
            var caseSpec2 = new CaseSpecification
            {
                SupportedMbFormFactor = "ATX Mid Tower",
                CoolingSupport = "Up to 6 fans",
                RadiatorSupport = "Up to 360mm",
                DriveBays = "2 x 3.5\" + 2 x 2.5\"",
                FrontPanelPorts = "USB 3.0, USB-C, Audio",
                MaxGpuLength = 355,
                MaxCpuCoolerHeight = 160,
                MaxPsuLength = 220,
                Material = "Steel, Tempered Glass",
                Color = "White",
                FanSupport = "Up to 6 x 120mm"
            };
            await _caseSpecRepository.InsertAsync(caseSpec2, autoSave: true);
            var productCase2 = new Product
            {
                CategoryId = cases.Id,
                CaseSpecificationId = caseSpec2.Id,
                ProductName = "Phanteks Eclipse P400A",
                Description = "High-airflow mid-tower ATX case",
                Price = 1800000,
                StockCount = 45,
                ImageUrl = "https://product.hstatic.net/200000722513/product/k-_1_65d8edfddc2b4785af9a13f971fc258a_6043347819ed417bb6dd327b41b39b6e_60a930dd805e4bc891b6ea69e7c2d21a_grande.jpg"
            };
            await _productRepository.InsertAsync(productCase2, autoSave: true);
            // 10. CPU Cooler: Noctua NH-D15
            var cpuCoolerSpec2 = new CpuCoolerSpecification
            {
                CoolerType = "Air",
                FanSize = 140,
                TdpSupport = 220,
                NoiseLevel = 24,
                Color = "Beige/Brown",
                LedLighting = "None",
                Height = 165,
                RadiatorSize = null,
                SupportedSockets = "AM4, LGA1700"
            };
            await _cpuCoolerSpecRepository.InsertAsync(cpuCoolerSpec2, autoSave: true);
            var productCpuCooler2 = new Product
            {
                CategoryId = coolers.Id,
                CpuCoolerSpecificationId = cpuCoolerSpec2.Id,
                ProductName = "Noctua NH-D15",
                Description = "Premium dual-tower air cooler",
                Price = 1500000,
                StockCount = 20,
                ImageUrl = "https://product.hstatic.net/200000722513/product/noctua_nh-d15_2_75940b3d5fbb485190327d6b592429af_9ad735dcdbb94a71ba171d7d4ae0a326_grande.jpg"
            };
            await _productRepository.InsertAsync(productCpuCooler2, autoSave: true);
            // 11. Headset: HyperX Cloud II
            var headsetSpec2 = new HeadsetSpecification
            {
                DriverSize = 53,
                HasMicrophone = true,
                IsNoiseCancelling = true,
                IsSurroundSound = true,
                Frequency = "15Hz - 25kHz",
                Connectivity = "Wired",
                MicrophoneType = "Detachable",
                Impedance = 60,
                Sensitivity = 98,
                Color = "Black"
            };
            await _headsetSpecRepository.InsertAsync(headsetSpec2, autoSave: true);
            var productHeadset2 = new Product
            {
                CategoryId = headsets.Id,
                HeadsetSpecificationId = headsetSpec2.Id,
                ProductName = "HyperX Cloud Stinger Core II",
                Description = "Comfortable gaming headset with 7.1 surround",
                Price = 1200000,
                StockCount = 60,
                ImageUrl = "https://product.hstatic.net/200000722513/product/thumbtainghe_499f42bf16fe47d28ab00bffb7bd5748_47730811ddaf40a0a969f4e4d49c7b27_grande.png"
            };
            await _productRepository.InsertAsync(productHeadset2, autoSave: true);
            // 12. Laptop: Dell XPS 13 9310
            var laptopSpec2 = new LaptopSpecification
            {
                BatteryLife = "12 hours",
                Weight = "1.2Kg",
                Warranty = "1 year",
                Storage = "PCIe SSD",
                GraphicsCard = "Intel Arc Graphics",
                CPU = "Intel® Core™ Ultra 5",
                RAM = "16GB LPDDR4x",
                OperatingSystem = "Windows 11 Pro",
                Display = "60 Hz, Anti-glare, InfinityEdge display"
            };
            await _laptopSpecRepository.InsertAsync(laptopSpec2, autoSave: true);
            var productLaptop2 = new Product
            {
                CategoryId = laptops.Id,
                LaptopSpecificationId = laptopSpec2.Id,
                ProductName = "Dell XPS 13 9310",
                Description = "Ultra-portable 13” laptop",
                Price = 31000000,
                StockCount = 8,
                ImageUrl = "https://product.hstatic.net/200000722513/product/51529_laptop_dell_xps_9350_xps93_1d46c518185a488a92c40932dd4d5cf6_grande.png"
            };

            await _productRepository.InsertAsync(productLaptop2, autoSave: true);

            // --- Seed Products ---
            // Chỉ seed nếu chưa có Product nào trong DB
            if (await _productRepository.GetCountAsync() > 0)
            {
                return;
            }

            await _productRepository.InsertAsync(productCpu2, autoSave: true);
            await _productRepository.InsertAsync(productGpu2, autoSave: true);
            await _productRepository.InsertAsync(productRam2, autoSave: true);
            await _productRepository.InsertAsync(productStorage2, autoSave: true);
            await _productRepository.InsertAsync(productMouse2, autoSave: true);
            await _productRepository.InsertAsync(keyboardProduct2, autoSave: true);
            await _productRepository.InsertAsync(productMonitor2, autoSave: true);
            await _productRepository.InsertAsync(productPsu2, autoSave: true);
            await _productRepository.InsertAsync(productCase2, autoSave: true);
            await _productRepository.InsertAsync(productCpuCooler2, autoSave: true);
            await _productRepository.InsertAsync(productHeadset2, autoSave: true);
            await _productRepository.InsertAsync(productLaptop2, autoSave: true);
        }
    }
}

