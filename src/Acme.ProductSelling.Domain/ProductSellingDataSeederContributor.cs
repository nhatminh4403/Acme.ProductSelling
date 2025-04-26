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
            _guidGenerator = guidGenerator;
        }
        public async Task SeedAsync(DataSeedContext context)
        {
            // --- Seed Categories ---
            // Chỉ seed nếu chưa có Category nào trong DB

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
                ProductName = "NVIDIA GeForce RTX 4070 Ti",
                Description = "High-end graphics card",
                StockCount = 20,
                CategoryId = gpus.Id,
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
                MouseSpecificationId = mouseSpec1.Id // Gán FK
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
                MonitorSpecificationId = monitorSpec1.Id // Gán FK
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
                PsuSpecificationId = psuSpec1.Id // Gán FK
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
                CaseSpecificationId = caseSpec1.Id // Gán FK
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
                CpuCoolerSpecificationId = cpuCoolerSpec1.Id // Gán FK
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
                HeadsetSpecificationId = headsetSpec1.Id // Gán FK
            };
            await _productRepository.InsertAsync(productHeadset1, autoSave: true);




        }
    }
}

