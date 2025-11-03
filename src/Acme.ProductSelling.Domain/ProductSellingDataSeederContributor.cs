using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Products.Specs;
using Acme.ProductSelling.Specifications.Junctions;
using Acme.ProductSelling.Specifications.Models;
using Acme.ProductSelling.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
namespace Acme.ProductSelling
{
    public class ProductSellingDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        #region Fields
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
        private readonly IRepository<Manufacturer, Guid> _manufacturerRepository;

        private readonly IRepository<CpuSocket, Guid> _socketRepository;
        private readonly IRepository<Chipset, Guid> _chipsetRepository;
        private readonly IRepository<FormFactor, Guid> _formFactorRepository;
        private readonly IRepository<Material, Guid> _materialRepository;
        private readonly IRepository<PanelType, Guid> _panelTypeRepository;
        private readonly IRepository<CpuCoolerSocketSupport> _cpuCoolerSocketSupportRepository;
        private readonly IRepository<CaseMaterial> _caseMaterialRepository;
        private readonly IRepository<RamType, Guid> _ramTypeRepository;
        private readonly IRepository<SwitchType, Guid> _switchTypeRepository;
        #endregion

        #region Additional Fields
        private readonly IRepository<CaseFanSpecification, Guid> _caseFanSpecRepository;
        private readonly IRepository<MemoryCardSpecification, Guid> _memoryCardSpecRepository;
        private readonly IRepository<SpeakerSpecification, Guid> _speakerSpecRepository;
        private readonly IRepository<MicrophoneSpecification, Guid> _microphoneSpecRepository;
        private readonly IRepository<WebcamSpecification, Guid> _webcamSpecRepository;
        private readonly IRepository<MousePadSpecification, Guid> _mousePadSpecRepository;
        private readonly IRepository<ChairSpecification, Guid> _chairSpecRepository;
        private readonly IRepository<DeskSpecification, Guid> _deskSpecRepository;
        private readonly IRepository<SoftwareSpecification, Guid> _softwareSpecRepository;
        private readonly IRepository<NetworkHardwareSpecification, Guid> _networkHardwareSpecRepository;
        private readonly IRepository<HandheldSpecification, Guid> _handheldSpecRepository;
        private readonly IRepository<ConsoleSpecification, Guid> _consoleSpecRepository;
        private readonly IRepository<HubSpecification, Guid> _hubSpecRepository;
        private readonly IRepository<CableSpecification, Guid> _cableSpecRepository;
        private readonly IRepository<ChargerSpecification, Guid> _chargerSpecRepository;
        private readonly IRepository<PowerBankSpecification, Guid> _powerBankSpecRepository;
        #endregion
        #region Constructor
        public ProductSellingDataSeederContributor(
            IRepository<Product, Guid> productRepository,
            IRepository<Category, Guid> categoryRepository,
            CategoryManager categoryManager,
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
            IGuidGenerator guidGenerator,
            IdentityUserManager identityUserManager,
            IRepository<Manufacturer, Guid> manufacturerRepository,
            IRepository<CpuSocket, Guid> socketRepository,
            IRepository<Chipset, Guid> chipsetRepository,
            IRepository<FormFactor, Guid> formFactorRepository,
            IRepository<Material, Guid> materialRepository,
            IRepository<PanelType, Guid> panelTypeRepository,
            IRepository<CpuCoolerSocketSupport> cpuCoolerSocketSupportRepository,
            IRepository<CaseMaterial> caseMaterialRepository,
            IRepository<RamType, Guid> ramTypeRepository,
            IRepository<SwitchType, Guid> switchTypeRepository,
            IRepository<CaseFanSpecification, Guid> caseFanSpecRepository,
            IRepository<MemoryCardSpecification, Guid> memoryCardSpecRepository,
            IRepository<SpeakerSpecification, Guid> speakerSpecRepository,
            IRepository<MicrophoneSpecification, Guid> microphoneSpecRepository,
            IRepository<WebcamSpecification, Guid> webcamSpecRepository,
            IRepository<MousePadSpecification, Guid> mousePadSpecRepository,
            IRepository<ChairSpecification, Guid> chairSpecRepository,
            IRepository<DeskSpecification, Guid> deskSpecRepository,
            IRepository<SoftwareSpecification, Guid> softwareSpecRepository,
            IRepository<NetworkHardwareSpecification, Guid> networkHardwareSpecRepository,
            IRepository<HandheldSpecification, Guid> handheldSpecRepository,
            IRepository<ConsoleSpecification, Guid> consoleSpecRepository,
            IRepository<HubSpecification, Guid> hubSpecRepository,
            IRepository<CableSpecification, Guid> cableSpecRepository,
            IRepository<ChargerSpecification, Guid> chargerSpecRepository,
            IRepository<PowerBankSpecification, Guid> powerBankSpecRepository)

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
            _manufacturerRepository = manufacturerRepository;
            _socketRepository = socketRepository;
            _chipsetRepository = chipsetRepository;
            _formFactorRepository = formFactorRepository;
            _materialRepository = materialRepository;
            _panelTypeRepository = panelTypeRepository;
            _cpuCoolerSocketSupportRepository = cpuCoolerSocketSupportRepository;
            _caseMaterialRepository = caseMaterialRepository;
            _ramTypeRepository = ramTypeRepository;
            _switchTypeRepository = switchTypeRepository;
            _caseFanSpecRepository = caseFanSpecRepository;
            _memoryCardSpecRepository = memoryCardSpecRepository;
            _speakerSpecRepository = speakerSpecRepository;
            _microphoneSpecRepository = microphoneSpecRepository;
            _webcamSpecRepository = webcamSpecRepository;
            _mousePadSpecRepository = mousePadSpecRepository;
            _chairSpecRepository = chairSpecRepository;
            _deskSpecRepository = deskSpecRepository;
            _softwareSpecRepository = softwareSpecRepository;
            _networkHardwareSpecRepository = networkHardwareSpecRepository;
            _handheldSpecRepository = handheldSpecRepository;
            _consoleSpecRepository = consoleSpecRepository;
            _hubSpecRepository = hubSpecRepository;
            _cableSpecRepository = cableSpecRepository;
            _chargerSpecRepository = chargerSpecRepository;
            _powerBankSpecRepository = powerBankSpecRepository;
        }
        #endregion
        public async Task SeedAsync(DataSeedContext context)
        {
            #region Categories
            if (await _categoryRepository.GetCountAsync() > 0 || await _productRepository.GetCountAsync() > 0)
            {
                return;
            }

            // 1. Laptop (Individual)
            var laptops = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Laptop",
                    "Laptop cho công việc và giải trí",
                    SpecificationType.Laptop,
                    CategoryGroup.Individual,
                    1,
                    "fas fa-laptop"
                )
            );

            // 2. Main, CPU, VGA Group
            var cpus = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "CPU",
                    "Bộ vi xử lý",
                    SpecificationType.CPU,
                    CategoryGroup.MainCpuVga,
                    2,
                    "fas fa-microchip"
                )
            );

            var gpus = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "VGA (GPU)",
                    "Card màn hình",
                    SpecificationType.GPU,
                    CategoryGroup.MainCpuVga,
                    2,
                    "fas fa-microchip"
                )
            );

            var motherboards = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Mainboard",
                    "Bo mạch chủ",
                    SpecificationType.Motherboard,
                    CategoryGroup.MainCpuVga,
                    2,
                    "fas fa-microchip"
                )
            );

            // 3. Case, Nguồn, Tản Group
            var cases = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Case",
                    "Vỏ máy tính",
                    SpecificationType.Case,
                    CategoryGroup.CaseAndCooling,
                    3,
                    "fas fa-box"
                )
            );

            var psus = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Nguồn (PSU)",
                    "Nguồn máy tính",
                    SpecificationType.PSU,
                    CategoryGroup.CaseAndCooling,
                    3,
                    "fas fa-box"
                )
            );

            var coolers = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Tản CPU",
                    "Tản nhiệt CPU",
                    SpecificationType.CPUCooler,
                    CategoryGroup.CaseAndCooling,
                    3,
                    "fas fa-box"
                )
            );

            var caseFans = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Quạt Case",
                    "Quạt tản nhiệt case",
                    SpecificationType.CaseFan,
                    CategoryGroup.CaseAndCooling,
                    3,
                    "fas fa-box"
                )
            );

            // 4. Storage, RAM, Memory Group
            var storage = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Ổ cứng (SSD/HDD)",
                    "Ổ cứng lưu trữ",
                    SpecificationType.Storage,
                    CategoryGroup.StorageRamMemory,
                    4,
                    "fas fa-memory"
                )
            );

            var rams = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "RAM",
                    "Bộ nhớ RAM",
                    SpecificationType.RAM,
                    CategoryGroup.StorageRamMemory,
                    4,
                    "fas fa-memory"
                )
            );

            var memoryCards = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Thẻ nhớ",
                    "Thẻ nhớ SD, MicroSD",
                    SpecificationType.MemoryCard,
                    CategoryGroup.StorageRamMemory,
                    4,
                    "fas fa-memory"
                )
            );

            // 5. Audio/Video Group
            var speakers = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Loa (Speaker)",
                    "Loa máy tính",
                    SpecificationType.Speaker,
                    CategoryGroup.AudioVideo,
                    5,
                    "fas fa-microphone-alt"
                )
            );

            var microphones = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Micro",
                    "Microphone",
                    SpecificationType.Microphone,
                    CategoryGroup.AudioVideo,
                    5,
                    "fas fa-microphone-alt"
                )
            );

            var webcams = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Webcam",
                    "Camera web",
                    SpecificationType.Webcam,
                    CategoryGroup.AudioVideo,
                    5,
                    "fas fa-microphone-alt"
                )
            );

            // 6. Monitor (Individual)
            var monitors = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Màn hình",
                    "Màn hình máy tính",
                    SpecificationType.Monitor,
                    CategoryGroup.Individual,
                    6,
                    "fas fa-tv"
                )
            );

            // 7. Keyboard (Individual)
            var keyboards = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Bàn phím",
                    "Bàn phím cơ, màng",
                    SpecificationType.Keyboard,
                    CategoryGroup.Individual,
                    7,
                    "fas fa-keyboard"
                )
            );

            // 8. Mouse & Pad Group
            var mice = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Chuột",
                    "Chuột máy tính",
                    SpecificationType.Mouse,
                    CategoryGroup.MouseAndPad,
                    8,
                    "fas fa-mouse"
                )
            );

            var mousePads = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Lót chuột",
                    "Mousepad",
                    SpecificationType.MousePad,
                    CategoryGroup.MouseAndPad,
                    8,
                    "fas fa-mouse"
                )
            );

            // 9. Headset (Individual)
            var headsets = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Tai Nghe",
                    "Tai nghe gaming, văn phòng",
                    SpecificationType.Headset,
                    CategoryGroup.Individual,
                    9,
                    "fas fa-headphones"
                )
            );

            // 10. Furniture Group
            var chairs = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Ghế",
                    "Ghế gaming, văn phòng",
                    SpecificationType.Chair,
                    CategoryGroup.Furniture,
                    10,
                    "fas fa-chair"
                )
            );

            var desks = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Bàn",
                    "Bàn làm việc, gaming",
                    SpecificationType.Desk,
                    CategoryGroup.Furniture,
                    10,
                    "fas fa-chair"
                )
            );

            // 11. Software & Network Group
            var software = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Phần mềm",
                    "Software, licenses",
                    SpecificationType.Software,
                    CategoryGroup.SoftwareNetwork,
                    11,
                    "fas fa-network-wired"
                )
            );

            var networkHardware = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Thiết bị mạng",
                    "Router, switch, adapter",
                    SpecificationType.NetworkHardware,
                    CategoryGroup.SoftwareNetwork,
                    11,
                    "fas fa-network-wired"
                )
            );

            // 12. Handheld & Console Group
            var handhelds = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Handheld",
                    "Máy chơi game cầm tay",
                    SpecificationType.Handheld,
                    CategoryGroup.HandheldConsole,
                    12,
                    "fas fa-gamepad"
                )
            );

            var consoles = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Console",
                    "Máy chơi game console",
                    SpecificationType.Console,
                    CategoryGroup.HandheldConsole,
                    12,
                    "fas fa-gamepad"
                )
            );

            // 13. Accessories Group
            var hubs = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Hub (Docking Station)",
                    "USB Hub, Docking Station",
                    SpecificationType.Hub,
                    CategoryGroup.Accessories,
                    13,
                    "fas fa-plug"
                )
            );

            var cables = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Cáp",
                    "Cáp HDMI, USB, Display",
                    SpecificationType.Cable,
                    CategoryGroup.Accessories,
                    13,
                    "fas fa-plug"
                )
            );

            var chargers = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Sạc",
                    "Adapter, charger",
                    SpecificationType.Charger,
                    CategoryGroup.Accessories,
                    13,
                    "fas fa-plug"
                )
            );

            var powerBanks = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Sạc dự phòng",
                    "Power bank",
                    SpecificationType.PowerBank,
                    CategoryGroup.Accessories,
                    13,
                    "fas fa-plug"
                )
            );

            // 14. Services (Individual)
            var services = await _categoryRepository.InsertAsync(
                await _categoryManager.CreateAsync(
                    "Dịch vụ và thông tin khác",
                    "Services and information",
                    SpecificationType.Services,
                    CategoryGroup.Individual,
                    14,
                    "fas fa-info-circle"
                )
            );

            #endregion

            #region Manufacturers
            List<Manufacturer> Manufacturers = new List<Manufacturer>();
            var asus = new Manufacturer
            {
                ContactInfo = "https://www.asus.com/",
                Name = "ASUS",
                ManufacturerImage = "/images/manufacturers/asus-logo.png",
                Description = "ASUS is a multinational computer hardware and electronics company headquartered in Taipei, Taiwan. It is known for its innovative products, including motherboards, graphics cards, laptops, desktops, and peripherals. ASUS is recognized for its commitment to quality and performance.",
            };
            asus.UrlSlug = UrlHelperMethod.RemoveDiacritics(asus.Name);
            Manufacturers.Add(asus);
            var acer = new Manufacturer
            {
                ContactInfo = "https://www.acer.com/",
                Name = "Acer",
                Description = "Acer is a Taiwanese multinational hardware and electronics corporation specializing in advanced electronics technology. It is known for its laptops, desktops, monitors, and other computer peripherals. Acer focuses on providing innovative solutions for consumers and businesses.",
                ManufacturerImage = "/images/manufacturers/acer-logo.png",
            };
            acer.UrlSlug = UrlHelperMethod.RemoveDiacritics(acer.Name);
            Manufacturers.Add(acer);
            var amd = new Manufacturer
            {
                ContactInfo = "https://www.amd.com/",
                Name = "AMD",
                Description = "AMD (Advanced Micro Devices) is an American multinational semiconductor company that develops computer processors and related technologies. It is known for its Ryzen CPUs and Radeon GPUs, which compete with Intel and NVIDIA products. AMD focuses on high-performance computing and graphics solutions.",
                ManufacturerImage = "/images/manufacturers/amd-logo.png"
            };
            amd.UrlSlug = UrlHelperMethod.RemoveDiacritics(amd.Name);
            Manufacturers.Add(amd);
            var intel = new Manufacturer
            {
                ContactInfo = "https://www.intel.com/",
                Name = "Intel",
                Description = "Intel Corporation is an American multinational corporation and technology company headquartered in Santa Clara, California. It is known for its semiconductor products, including microprocessors, integrated circuits, and memory modules. Intel is a leader in computing innovation and technology.",
                ManufacturerImage = "/images/manufacturers/intel-logo.png"
            };
            intel.UrlSlug = UrlHelperMethod.RemoveDiacritics(intel.Name);
            Manufacturers.Add(intel);
            var logitech = new Manufacturer
            {
                ContactInfo = "https://www.logitech.com/",
                Name = "Logitech",
                Description = "Logitech is a Swiss company that designs and manufactures computer peripherals and software. It is known for its mice, keyboards, webcams, and gaming accessories. Logitech focuses on creating innovative products that enhance the user experience.",
                ManufacturerImage = "/images/manufacturers/logitech-logo.png"
            };
            logitech.UrlSlug = UrlHelperMethod.RemoveDiacritics(logitech.Name);
            Manufacturers.Add(logitech);
            var corsair = new Manufacturer
            {
                ContactInfo = "https://www.corsair.com/",
                Name = "Corsair",
                Description = "Corsair is an American computer peripherals and hardware company known for its high-performance gaming products, including memory modules, power supplies, cooling solutions, and gaming peripherals. Corsair focuses on delivering quality and performance to gamers and PC enthusiasts.",
                ManufacturerImage = "/images/manufacturers/corsair-logo.png"
            };
            corsair.UrlSlug = UrlHelperMethod.RemoveDiacritics(corsair.Name);
            Manufacturers.Add(corsair);
            var coolerMaster = new Manufacturer
            {
                ContactInfo = "https://www.coolermaster.com/",
                Name = "Cooler Master",
                Description = "Cooler Master is a Taiwanese computer hardware manufacturer known for its cooling solutions, power supplies, and computer cases. It focuses on providing innovative products for gamers and PC builders, including CPU coolers, cases, and peripherals.",
                ManufacturerImage = "/images/manufacturers/cooler-master-logo.png",
            };
            coolerMaster.UrlSlug = UrlHelperMethod.RemoveDiacritics(coolerMaster.Name);
            Manufacturers.Add(coolerMaster);
            var msi = new Manufacturer
            {
                ContactInfo = "https://www.msi.com/",
                Name = "MSI",
                Description = "MSI (Micro-Star International) is a Taiwanese multinational information technology corporation known for its computer hardware products, including motherboards, graphics cards, laptops, and gaming peripherals. MSI focuses on high-performance gaming and computing solutions.",
                ManufacturerImage = "/images/manufacturers/MSI-Logo.png"
            };
            msi.UrlSlug = UrlHelperMethod.RemoveDiacritics(msi.Name);
            Manufacturers.Add(msi);
            var gigabyte = new Manufacturer
            {
                ContactInfo = "https://www.gigabyte.com/",
                Name = "Gigabyte",
                Description = "Gigabyte Technology is a Taiwanese manufacturer of computer hardware products, including motherboards, graphics cards, laptops, and gaming peripherals. It is known for its high-performance components and innovative technology solutions.",
                ManufacturerImage = "/images/manufacturers/gigabyte-logo.png"
            };
            gigabyte.UrlSlug = UrlHelperMethod.RemoveDiacritics(gigabyte.Name);
            Manufacturers.Add(gigabyte);
            var razer = new Manufacturer
            {
                ContactInfo = "https://www.razer.com/",
                Name = "Razer",
                Description = "Razer Inc. is a global gaming hardware manufacturing company known for its high-performance gaming peripherals, laptops, and software. It focuses on creating innovative products for gamers, including mice, keyboards, headsets, and gaming laptops.",
                ManufacturerImage = "/images/manufacturers/Razer-logo.png"
            };
            razer.UrlSlug = UrlHelperMethod.RemoveDiacritics(razer.Name);
            Manufacturers.Add(razer);
            var samsung = new Manufacturer
            {
                ContactInfo = "https://www.samsung.com/",
                Name = "Samsung",
                Description = "Samsung Electronics is a South Korean multinational electronics company known for its consumer electronics, semiconductors, and telecommunications products. It is one of the largest manufacturers of smartphones, TVs, and memory chips in the world.",
                ManufacturerImage = "/images/manufacturers/Samsung-Logo.png"
            };
            samsung.UrlSlug = UrlHelperMethod.RemoveDiacritics(samsung.Name);
            Manufacturers.Add(samsung);
            var nvidia = new Manufacturer
            {
                ContactInfo = "https://www.nvidia.com/",
                Name = "NVIDIA",
                Description = "NVIDIA Corporation is an American multinational technology company known for its graphics processing units (GPUs) and AI computing technology." +
                " It is a leader in the gaming, professional visualization, data center, and automotive markets.",
                ManufacturerImage = "/images/manufacturers/nvidia-logo.png"
            };
            nvidia.UrlSlug = UrlHelperMethod.RemoveDiacritics(nvidia.Name);
            Manufacturers.Add(nvidia);
            var lg = new Manufacturer
            {
                ContactInfo = "https://www.lg.com/",
                Name = "LG",
                Description = "LG Electronics is a South Korean multinational electronics company known for its consumer electronics," +
                " home appliances, and mobile communications" +
                ". It is one of the largest manufacturers of TVs, refrigerators, and washing machines in the world.",
                ManufacturerImage = "/images/manufacturers/lg-logo.png"
            };
            lg.UrlSlug = UrlHelperMethod.RemoveDiacritics(lg.Name);
            Manufacturers.Add(lg);
            var nzxt = new Manufacturer
            {
                ContactInfo = "https://www.nzxt.com/",
                Name = "NZXT",
                Description = "NZXT is an American computer hardware company known for its PC cases, cooling solutions," +
                " and gaming peripherals. It focuses on providing innovative products for gamers and PC builders, including cases," +
                " power supplies, and cooling solutions.",
                ManufacturerImage = "/images/manufacturers/nzxt-logo.png"
            };
            nzxt.UrlSlug = UrlHelperMethod.RemoveDiacritics(nzxt.Name);
            Manufacturers.Add(nzxt);
            var gskill = new Manufacturer
            {
                ContactInfo = "https://www.gskill.com/",
                Name = "G.Skill",
                Description = "G.Skill is a Taiwanese manufacturer of computer memory modules and storage solutions. It is known for its high-performance RAM and SSD products, catering to gamers and PC enthusiasts. G.Skill focuses on delivering quality and performance in its products.",
                ManufacturerImage = "/images/manufacturers/gskill-logo.png"
            };
            gskill.UrlSlug = UrlHelperMethod.RemoveDiacritics(gskill.Name);
            Manufacturers.Add(gskill);
            var hyperx = new Manufacturer
            {
                ContactInfo = "https://www.hyperxgaming.com/",
                Name = "HyperX",
                Description = "HyperX is a gaming division of Kingston Technology Company, Inc. It is known for its high-performance gaming peripherals, memory modules, and storage solutions. HyperX focuses on delivering quality and performance for gamers and PC enthusiasts.",
                ManufacturerImage = "/images/manufacturers/hyperx-logo.png"
            };
            hyperx.UrlSlug = UrlHelperMethod.RemoveDiacritics(hyperx.Name);
            Manufacturers.Add(hyperx);
            var steelseries = new Manufacturer
            {
                ContactInfo = "https://steelseries.com/",
                Name = "SteelSeries",
                Description = "SteelSeries is a Danish manufacturer of gaming peripherals and accessories. It is known for its high-performance gaming mice, keyboards, headsets, and mousepads. SteelSeries focuses on creating innovative products for gamers.",
                ManufacturerImage = "/images/manufacturers/SteelSeries-logo.png"
            };
            steelseries.UrlSlug = UrlHelperMethod.RemoveDiacritics(steelseries.Name);
            Manufacturers.Add(steelseries);
            var dell = new Manufacturer
            {
                ContactInfo = "https://www.dell.com/",
                Name = "Dell",
                Description = "Dell Technologies is an American multinational technology company known for its computer hardware, software, and services. It is one of the largest manufacturers of PCs, servers, and storage solutions in the world.",
                ManufacturerImage = "/images/manufacturers/Dell-Logo.png"
            };
            dell.UrlSlug = UrlHelperMethod.RemoveDiacritics(dell.Name);
            Manufacturers.Add(dell);
            var hp = new Manufacturer
            {
                ContactInfo = "https://www.hp.com/",
                Name = "HP",
                Description = "HP Inc. is an American multinational information technology company known for its printers, PCs, and related products. It is one of the largest manufacturers of printers and computers in the world.",
                ManufacturerImage = "/images/manufacturers/hp-logo.png"
            };
            hp.UrlSlug = UrlHelperMethod.RemoveDiacritics(hp.Name);
            Manufacturers.Add(hp);
            var phanteks = new Manufacturer
            {
                ContactInfo = "https://www.phanteks.com/",
                Name = "Phanteks",
                Description = "Phanteks is a manufacturer of computer hardware products, including cases, cooling solutions, and power supplies. It focuses on providing innovative products for gamers and PC builders.",
                ManufacturerImage = "/images/manufacturers/phanteks-logo.png"
            };
            phanteks.UrlSlug = UrlHelperMethod.RemoveDiacritics(phanteks.Name);
            Manufacturers.Add(phanteks);
            var evga = new Manufacturer
            {
                ContactInfo = "https://www.evga.com/",
                Name = "EVGA",
                Description = "EVGA Corporation is an American computer hardware company known for its graphics cards, motherboards, and power supplies. It focuses on providing high-performance products for gamers and PC enthusiasts.",
                ManufacturerImage = "/images/manufacturers/evga-logo.png"
            };
            evga.UrlSlug = UrlHelperMethod.RemoveDiacritics(evga.Name);
            Manufacturers.Add(evga);
            var wd = new Manufacturer
            {
                ContactInfo = "https://www.westerndigital.com/",
                Name = "WD",
                Description = "Western Digital Corporation is an American computer data storage company known for its hard drives and solid-state drives. It is one of the largest manufacturers of storage devices in the world.",
                ManufacturerImage = "/images/manufacturers/wd-logo.png"
            };
            wd.UrlSlug = UrlHelperMethod.RemoveDiacritics(wd.Name);
            Manufacturers.Add(wd);
            var noctua = new Manufacturer
            {
                ContactInfo = "https://noctua.at/en/",
                Name = "Noctua",
                Description = "Noctua is an Austrian manufacturer of computer cooling solutions, including CPU coolers and fans. It is known for its high-performance and quiet cooling products.",
                ManufacturerImage = "/images/manufacturers/noctua-logo.png"
            };
            noctua.UrlSlug = UrlHelperMethod.RemoveDiacritics(noctua.Name);
            Manufacturers.Add(noctua);
            var asrock = new Manufacturer
            {
                ContactInfo = "https://www.asrock.com/",
                Name = "ASRock",
                Description = "ASRock Inc. is a Taiwanese manufacturer of motherboards, graphics cards, and other computer hardware. It is known for its innovative products and focuses on providing quality and performance.",
                ManufacturerImage = "/images/manufacturers/ASRock-logo.png"
            };
            asrock.UrlSlug = UrlHelperMethod.RemoveDiacritics(asrock.Name);
            Manufacturers.Add(asrock);

            // Add to your Manufacturers list
            var ikea = new Manufacturer
            {
                ContactInfo = "https://www.ikea.com/",
                Name = "IKEA",
                Description = "Swedish multinational conglomerate that designs and sells ready-to-assemble furniture, kitchen appliances and home accessories.",
                ManufacturerImage = "/images/manufacturers/ikea-logo.png"
            };
            ikea.UrlSlug = UrlHelperMethod.RemoveDiacritics(ikea.Name);
            Manufacturers.Add(ikea);

            var microsoft = new Manufacturer
            {
                ContactInfo = "https://www.microsoft.com/",
                Name = "Microsoft",
                Description = "American multinational technology corporation known for Windows, Office, Xbox and Azure cloud services.",
                ManufacturerImage = "/images/manufacturers/microsoft-logo.png"
            };
            microsoft.UrlSlug = UrlHelperMethod.RemoveDiacritics(microsoft.Name);
            Manufacturers.Add(microsoft);

            var tplink = new Manufacturer
            {
                ContactInfo = "https://www.tp-link.com/",
                Name = "TP-Link",
                Description = "Chinese manufacturer of computer networking products including routers, switches, and smart home devices.",
                ManufacturerImage = "/images/manufacturers/tplink-logo.png"
            };
            tplink.UrlSlug = UrlHelperMethod.RemoveDiacritics(tplink.Name);
            Manufacturers.Add(tplink);

            var valve = new Manufacturer
            {
                ContactInfo = "https://www.valvesoftware.com/",
                Name = "Valve",
                Description = "American video game developer and digital distribution company, creator of Steam and Steam Deck.",
                ManufacturerImage = "/images/manufacturers/valve-logo.png"
            };
            valve.UrlSlug = UrlHelperMethod.RemoveDiacritics(valve.Name);
            Manufacturers.Add(valve);

            var sony = new Manufacturer
            {
                ContactInfo = "https://www.sony.com/",
                Name = "Sony",
                Description = "Japanese multinational conglomerate known for electronics, gaming (PlayStation), entertainment and imaging products.",
                ManufacturerImage = "/images/manufacturers/sony-logo.png"
            };
            sony.UrlSlug = UrlHelperMethod.RemoveDiacritics(sony.Name);
            Manufacturers.Add(sony);

            var anker = new Manufacturer
            {
                ContactInfo = "https://www.anker.com/",
                Name = "Anker",
                Description = "Chinese electronics company known for charging accessories, power banks, cables and hubs.",
                ManufacturerImage = "/images/manufacturers/anker-logo.png"
            };
            anker.UrlSlug = UrlHelperMethod.RemoveDiacritics(anker.Name);
            Manufacturers.Add(anker);

            var ugreen = new Manufacturer
            {
                ContactInfo = "https://www.ugreen.com/",
                Name = "UGREEN",
                Description = "Chinese manufacturer of computer and mobile accessories including cables, chargers and adapters.",
                ManufacturerImage = "/images/manufacturers/ugreen-logo.png"
            };
            ugreen.UrlSlug = UrlHelperMethod.RemoveDiacritics(ugreen.Name);
            Manufacturers.Add(ugreen);
            await _manufacturerRepository.InsertManyAsync(Manufacturers, autoSave: true);
            #endregion

            #region Seed Lookup Tables

            var am4 = await _socketRepository.InsertAsync(new CpuSocket { Name = "AM4" });
            var am5 = await _socketRepository.InsertAsync(new CpuSocket { Name = "AM5" });
            var lga1700 = await _socketRepository.InsertAsync(new CpuSocket { Name = "LGA1700" });
            var lga1851 = await _socketRepository.InsertAsync(new CpuSocket { Name = "LGA1851" });
            var lga1200 = await _socketRepository.InsertAsync(new CpuSocket { Name = "LGA1200" });

            // Chipsets
            var z790 = await _chipsetRepository.InsertAsync(new Chipset { Name = "Intel Z790 Express" });
            var z890 = await _chipsetRepository.InsertAsync(new Chipset { Name = "Intel Z890" });
            var x670e = await _chipsetRepository.InsertAsync(new Chipset { Name = "AMD X670E" });

            // Form Factors
            var atx = await _formFactorRepository.InsertAsync(new FormFactor { Name = "ATX" });
            var eatx = await _formFactorRepository.InsertAsync(new FormFactor { Name = "E-ATX" });
            var atxMidTower = await _formFactorRepository.InsertAsync(new FormFactor { Name = "ATX Mid Tower" });
            // Materials
            var steel = await _materialRepository.InsertAsync(new Material { Name = "Steel" });
            var temperedGlass = await _materialRepository.InsertAsync(new Material { Name = "Tempered Glass" });

            // Panel Types
            var ips = await _panelTypeRepository.InsertAsync(new PanelType { Name = "IPS" });
            var tnPanel = await _panelTypeRepository.InsertAsync(new PanelType { Name = "TN" });
            var oled = await _panelTypeRepository.InsertAsync(new PanelType { Name = "OLED" });
            var vaPanel = await _panelTypeRepository.InsertAsync(new PanelType { Name = "VA" });
            //ram
            var ddr4 = await _ramTypeRepository.InsertAsync(new RamType { Name = "DDR4" });
            var ddr5 = await _ramTypeRepository.InsertAsync(new RamType { Name = "DDR5" });
            //switchtype

            var linearRed = await _switchTypeRepository.InsertAsync(new SwitchType { Name = "Cherry MX Red (Linear)" });
            var linearBlack = await _switchTypeRepository.InsertAsync(new SwitchType { Name = "Cherry MX Black (Linear)" });
            var tactileBrown = await _switchTypeRepository.InsertAsync(new SwitchType { Name = "Cherry MX Brown (Tactile)" });
            var tactileClear = await _switchTypeRepository.InsertAsync(new SwitchType { Name = "Cherry MX Clear (Tactile)" });
            var clickyBlue = await _switchTypeRepository.InsertAsync(new SwitchType { Name = "Cherry MX Blue (Clicky)" });
            var clickyGreen = await _switchTypeRepository.InsertAsync(new SwitchType { Name = "Cherry MX Green (Clicky)" });
            var silentRed = await _switchTypeRepository.InsertAsync(new SwitchType { Name = "Cherry MX Silent Red (Silent Linear)" });
            var lowProfile = await _switchTypeRepository.InsertAsync(new SwitchType { Name = "Kailh Choc (Low Profile)" });
            var opticalRed = await _switchTypeRepository.InsertAsync(new SwitchType { Name = "Razer Optical Red (Optical Linear)" });
            var hallEffect = await _switchTypeRepository.InsertAsync(new SwitchType { Name = "Wooting Lekker (Hall Effect Analog)" });

            var sockets = await _socketRepository.GetListAsync();
            var chipsets = await _chipsetRepository.GetListAsync();
            var formFactors = await _formFactorRepository.GetListAsync();
            var materials = await _materialRepository.GetListAsync();
            var panelTypes = await _panelTypeRepository.GetListAsync();
            var ramtypes = await _ramTypeRepository.GetListAsync();
            var switchTypes = await _switchTypeRepository.GetListAsync();
            #endregion



            #region Products and Specs

            #region CPU Products
            var productCpu1 = await CreateProductAsync(
                cpus.Id, amd.Id, 8500000, 10,
                "Ryzen 7 7700X",
                "Powerful 8-core CPU",
                50,
                "https://product.hstatic.net/200000722513/product/ryzen_7_-_1_00957bbe7b8542308c897a90d439b1fd_e1c9a16c537d47bb9768828dddb332d0_grande.jpg"
            );
            await _cpuSpecRepository.InsertAsync(new CpuSpecification
            {
                ProductId = productCpu1.Id,
                SocketId = am5.Id,
                CoreCount = 8,
                ThreadCount = 16,
                BaseClock = 4.5f,
                BoostClock = 5.4f,
                L3Cache = 32,
                Tdp = 105,
                HasIntegratedGraphics = true
            }, autoSave: true);

            var productCpu2 = await CreateProductAsync(
                cpus.Id, intel.Id, 13500000, 7,
                "Intel Core i9-13900K",
                "Top-tier 24-core CPU",
                30,
                "https://product.hstatic.net/200000722513/product/i9k-t2-special-box-07-1080x1080pixels_6c9ec1001cdf4e4998c13af4ac6c7581_114c47698e4a4984863c3b26e0619b65_grande.png"
            );
            await _cpuSpecRepository.InsertAsync(new CpuSpecification
            {
                ProductId = productCpu2.Id,
                SocketId = lga1700.Id,
                CoreCount = 24,
                ThreadCount = 32,
                BaseClock = 3.0f,
                BoostClock = 5.8f,
                L3Cache = 36,
                Tdp = 125,
                HasIntegratedGraphics = true
            }, autoSave: true);
            #endregion

            #region GPU Products
            var productGpu1 = await CreateProductAsync(
                gpus.Id, asus.Id, 25000000, 5,
                "ProArt GeForce RTX 4070 Ti SUPER 16GB",
                "High-end graphics card",
                20,
                "https://product.hstatic.net/200000722513/product/fwebp__10__1d22cf39c094494bb772b5bb1c002172_grande.png"
            );
            await _gpuSpecRepository.InsertAsync(new GpuSpecification
            {
                ProductId = productGpu1.Id,
                Chipset = "GeForce RTX 4070 Ti",
                MemorySize = 12,
                MemoryType = "GDDR6X",
                BoostClock = 2610f,
                Interface = "PCIe 4.0 x16",
                RecommendedPsu = 700,
                Length = 310f
            }, autoSave: true);

            var productGpu2 = await CreateProductAsync(
                gpus.Id, asus.Id, 28000000, 10,
                "Asus Radeon RX 7900 XT TUF Gaming",
                "High-end AMD graphics card",
                15,
                "https://product.hstatic.net/200000722513/product/5681_ea11053c19e375dcaa8138b6f531262d_7d029f536978405393da9fb3c8f1e2fa_4d3cedb8fd4a485db1ece7519c1d41a8_grande.jpg"
            );
            await _gpuSpecRepository.InsertAsync(new GpuSpecification
            {
                ProductId = productGpu2.Id,
                Chipset = "Radeon RX 7900 XT",
                MemorySize = 20,
                MemoryType = "GDDR6",
                BoostClock = 2600f,
                Interface = "PCIe 4.0 x16",
                RecommendedPsu = 750,
                Length = 310f
            }, autoSave: true);
            #endregion

            #region RAM Products
            var productRam1 = await CreateProductAsync(
                rams.Id, corsair.Id, 2000000, 0,
                "Vengeance LPX 16GB",
                "High-performance RAM",
                100,
                "https://product.hstatic.net/200000722513/product/gearvn-corsair-vengeance-rgb-ddr-5600-ddr5-5_6e319950a8e14231b28a416076c94951_grande.png"
            );
            await _ramSpecRepository.InsertAsync(new RamSpecification
            {
                ProductId = productRam1.Id,
                RamTypeId = ddr5.Id,
                Capacity = 32,
                Speed = 6000,
                ModuleCount = 2,
                Timing = "36-36-36-76",
                Voltage = 1.35f,
                HasRGB = true
            }, autoSave: true);

            var productRam2 = await CreateProductAsync(
                rams.Id, gskill.Id, 1800000, 5,
                "Trident Z RGB 16GB (2×8)",
                "DDR4 3600MHz kit with RGB",
                80,
                "https://anphat.com.vn/media/product/33685_153665426813.png"
            );
            await _ramSpecRepository.InsertAsync(new RamSpecification
            {
                ProductId = productRam2.Id,
                RamTypeId = ddr4.Id,
                Capacity = 16,
                Speed = 3600,
                ModuleCount = 2,
                Timing = "18-22-22-42",
                Voltage = 1.35f,
                HasRGB = true
            }, autoSave: true);
            #endregion

            #region Storage Products
            var productStorage1 = await CreateProductAsync(
                storage.Id, samsung.Id, 3000000, 8,
                "970 EVO Plus 1TB",
                "Fast NVMe SSD",
                80,
                "https://bizweb.dktcdn.net/thumb/grande/100/329/122/products/970-evo-plus-1tb-01-1689929004911.jpg?v=1695052940103"
            );
            await _storageSpecRepository.InsertAsync(new StorageSpecification
            {
                ProductId = productStorage1.Id,
                StorageType = StorageType.NvmeSsd,
                Interface = "PCIe 3.0 x4",
                Capacity = 1000,
                ReadSpeed = 3500,
                WriteSpeed = 3300,

                StorageFormFactor = StorageFormFactor.SsdM2_2280
            }, autoSave: true);

            var productStorage2 = await CreateProductAsync(
                storage.Id, wd.Id, 2500000, 5,
                "WD Black SN770 1TB",
                "High-performance PCIe 4.0 NVMe SSD",
                60,
                "https://bizweb.dktcdn.net/thumb/grande/100/329/122/products/ssd-wd-black-sn770-pcie-gen4-x4-nvme-m-2-500gb-wds500g3x0e-b058273a-af63-4053-ac31-83b41eb593a2.jpg?v=1655710957737"
            );
            await _storageSpecRepository.InsertAsync(new StorageSpecification
            {
                ProductId = productStorage2.Id,
                StorageType = StorageType.NvmeSsd,
                Interface = "PCIe 4.0 x4",
                Capacity = 1000,
                ReadSpeed = 5150,
                WriteSpeed = 4900,
                StorageFormFactor = StorageFormFactor.SsdM2_2280,

            }, autoSave: true);
            #endregion

            #region Keyboard Products
            var keyboardProduct1 = await CreateProductAsync(
                keyboards.Id, logitech.Id, 3000000, 7,
                "Logitech G Pro TKL Keyboard",
                "Tenkeyless mechanical gaming keyboard",
                80,
                "https://product.hstatic.net/200000722513/product/1_5b2f7891bf434a7aab9f1abdba56c17e_grande.jpg"
            );
            await _keyboardSpecRepository.InsertAsync(new KeyboardSpecification
            {
                ProductId = keyboardProduct1.Id,
                KeyboardType = "Mechanical",
                SwitchTypeId = tactileBrown.Id,
                Layout = KeyboardLayout.TKL,
                Backlight = "RGB"
            }, autoSave: true);

            var keyboardProduct2 = await CreateProductAsync(
                keyboards.Id, razer.Id, 4500000, 5,
                "Razer Huntsman Elite",
                "Opto-mechanical gaming keyboard",
                40,
                "https://product.hstatic.net/200000722513/product/r3m1_ac3aa0be001640e2873ff732d34617bc_2295901522e24ce399b8f5f07be51467_3ab2e4aca4434a9a84997283b79b5c3c_grande.png"
            );
            await _keyboardSpecRepository.InsertAsync(new KeyboardSpecification
            {
                ProductId = keyboardProduct2.Id,
                KeyboardType = "Mechanical",
                SwitchTypeId = linearRed.Id,
                Layout = KeyboardLayout.FullSize,
                Backlight = "RGB"
            }, autoSave: true);
            #endregion

            #region Monitor Products
            var productMonitor1 = await CreateProductAsync(
                monitors.Id, lg.Id, 7000000, 6,
                "UltraGear 27GL850",
                "27-inch QHD gaming monitor",
                30,
                "https://product.hstatic.net/200000722513/product/lg_27gx790a-b_gearvn_18880ec6e5a944c2b29c76d85d44d243_medium.jpg"
            );
            await _monitorSpecRepository.InsertAsync(new MonitorSpecification
            {
                ProductId = productMonitor1.Id,
                ScreenSize = 27,
                Resolution = "2560x1440",
                RefreshRate = 144,
                PanelTypeId = ips.Id,
                ResponseTimeMs = 1,
                ColorGamut = "sRGB 99%",
                Brightness = 400,
                VesaMount = true
            }, autoSave: true);

            var productMonitor2 = await CreateProductAsync(
                monitors.Id, asus.Id, 9000000, 12,
                "ASUS TUF Gaming VG27AQ",
                "27\" QHD IPS 165Hz gaming monitor",
                25,
                "https://product.hstatic.net/200000722513/product/ips-2k-170hz-g-sync-hdr-chuyen-game-1_f9de14d5b20041b2b52b0cde6884a3d9_317538ed8cff45e6a25feb1cbb8650d0_grande.png"
            );
            await _monitorSpecRepository.InsertAsync(new MonitorSpecification
            {
                ProductId = productMonitor2.Id,
                ScreenSize = 27,
                Resolution = "2560x1440",
                RefreshRate = 165,
                PanelTypeId = ips.Id,
                ResponseTimeMs = 1,
                ColorGamut = "sRGB 99%",
                Brightness = 350,
                VesaMount = true
            }, autoSave: true);
            var productKeyboard_MultiMode = await CreateProductAsync(
                keyboards.Id,
                logitech.Id, // Giả sử 'logitech' đã được khởi tạo
                4990000,
                15,
                "Logitech G915 TKL LIGHTSPEED Wireless",
                "Bàn phím cơ TKL mỏng nhẹ, hiệu năng cao, cung cấp 3 chế độ kết nối: LIGHTSPEED không dây chuyên nghiệp cho game, Bluetooth tiện dụng để kết nối nhiều thiết bị, và chế độ có dây.",
                45,
                "https://resource.logitechg.com/w_692,c_lpad,ar_4:3,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/gaming/en/products/g915-tkl/g915-tkl-gallery/g915-tkl-carbon-gallery-1.png"
            );

            await _keyboardSpecRepository.InsertAsync(new KeyboardSpecification
            {
                ProductId = productKeyboard_MultiMode.Id,
                KeyboardType = "Mechanical",
                SwitchTypeId = linearRed.Id, // Giả định dùng switch Linear Red đã seed
                Layout = KeyboardLayout.TKL,
                Connectivity = ConnectivityType.WiredAndWireless, // Enum này đại diện cho khả năng đa kết nối
                Backlight = "RGB LIGHTSYNC"
            }, autoSave: true);
            #endregion

            #region PSU Products
            var productPsu1 = await CreateProductAsync(
                psus.Id, corsair.Id, 2000000, 5,
                "Corsair RM750x",
                "750W fully modular power supply",
                50,
                "https://bizweb.dktcdn.net/thumb/grande/100/329/122/products/nguon-may-tinh-corsair-rm750x-shift-750w-80-plus-gold-cp-9020251-na-04-20838ea6-b253-460f-bb0c-ad9327565373.jpg?v=1743639588677"
            );
            await _psuSpecRepository.InsertAsync(new PsuSpecification
            {
                ProductId = productPsu1.Id,
                Wattage = 750,
                EfficiencyRating = "80 Plus Gold",
                Modularity = PsuModularity.FullModularity,
                FormFactorId = atx.Id
            }, autoSave: true);

            var productPsu2 = await CreateProductAsync(
                psus.Id, evga.Id, 2200000, 20,
                "EVGA SuperNOVA 750 G5",
                "750W gold-rated fully modular PSU",
                35,
                "https://tandoanh.vn/wp-content/uploads/2021/10/EVGA-SuperNOVA-750-G1-%E2%80%93-80-GOLD-750W-%E2%80%93-Fully-Modular-h1.jpg"
            );
            await _psuSpecRepository.InsertAsync(new PsuSpecification
            {
                ProductId = productPsu2.Id,
                Wattage = 750,
                EfficiencyRating = "80 Plus Gold",
                Modularity = PsuModularity.FullModularity,
                FormFactorId = atx.Id
            }, autoSave: true);
            #endregion

            #region Case Products (with Many-to-Many Materials)
            var productCase1 = await CreateProductAsync(
                cases.Id, nzxt.Id, 1500000, 5,
                "NZXT H510",
                "Mid-tower ATX case with tempered glass",
                40,
                "https://product.hstatic.net/200000722513/product/4108_be554d73268e3ca69f25d192629df397_b7fd1aebb5f74f50ae18c3b23efb8755_b6d80711bb304b568b03fdcf3e94c1ab_grande.jpg"
            );
            var spec_case1 = await _caseSpecRepository.InsertAsync(new CaseSpecification
            {
                ProductId = productCase1.Id,
                FormFactorId = atxMidTower.Id,
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
                new CaseMaterial { CaseSpecificationId = spec_case1.Id, MaterialId = steel.Id },
                autoSave: true
            );
            await _caseMaterialRepository.InsertAsync(
                new CaseMaterial { CaseSpecificationId = spec_case1.Id, MaterialId = temperedGlass.Id },
                autoSave: true
            );

            var productCase2 = await CreateProductAsync(
                cases.Id, phanteks.Id, 1800000, 10,
                "Phanteks Eclipse P400A",
                "High-airflow mid-tower ATX case",
                45,
                "https://product.hstatic.net/200000722513/product/k-_1_65d8edfddc2b4785af9a13f971fc258a_6043347819ed417bb6dd327b41b39b6e_60a930dd805e4bc891b6ea69e7c2d21a_grande.jpg"
            );
            var spec_case2 = await _caseSpecRepository.InsertAsync(new CaseSpecification
            {
                ProductId = productCase2.Id,
                FormFactorId = atxMidTower.Id,
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
                new CaseMaterial { CaseSpecificationId = spec_case2.Id, MaterialId = steel.Id },
                autoSave: true
            );
            await _caseMaterialRepository.InsertAsync(
                new CaseMaterial { CaseSpecificationId = spec_case2.Id, MaterialId = temperedGlass.Id },
                autoSave: true
            );
            #endregion

            #region CPU Cooler Products (with Many-to-Many Socket Support)
            var productCpuCooler1 = await CreateProductAsync(
                coolers.Id, coolerMaster.Id, 1200000, 5,
                "Cooler Master Hyper 212",
                "Air cooler with RGB lighting",
                60,
                "https://product.hstatic.net/200000722513/product/hyper-212-argb-gallery-4-image_dc19349414e94e0e869c23e85c70cb49_d2713cd5bac947da94ee34d1456220fe_grande.png"
            );
            var spec_cooler1 = await _cpuCoolerSpecRepository.InsertAsync(new CpuCoolerSpecification
            {
                ProductId = productCpuCooler1.Id,
                CoolerType = "Air",
                FanSize = 120,
                TdpSupport = 150,
                NoiseLevel = 30,
                Color = "Black",
                LedLighting = "RGB",
                Height = 160
            }, autoSave: true);
            await _cpuCoolerSocketSupportRepository.InsertAsync(
                new CpuCoolerSocketSupport { CpuCoolerSpecificationId = spec_cooler1.Id, SocketId = am4.Id },
                autoSave: true
            );
            await _cpuCoolerSocketSupportRepository.InsertAsync(
                new CpuCoolerSocketSupport { CpuCoolerSpecificationId = spec_cooler1.Id, SocketId = lga1200.Id },
                autoSave: true
            );

            var productCpuCooler2 = await CreateProductAsync(
                coolers.Id, noctua.Id, 4000000, 25,
                "Noctua NH-D15",
                "Premium dual-tower air cooler",
                20,
                "https://product.hstatic.net/200000722513/product/noctua_nh-d15_2_75940b3d5fbb485190327d6b592429af_9ad735dcdbb94a71ba171d7d4ae0a326_grande.jpg"
            );
            var spec_cooler2 = await _cpuCoolerSpecRepository.InsertAsync(new CpuCoolerSpecification
            {
                ProductId = productCpuCooler2.Id,
                CoolerType = "Air",
                FanSize = 140,
                TdpSupport = 220,
                NoiseLevel = 24,
                Color = "Beige/Brown",
                LedLighting = "None",
                Height = 165
            }, autoSave: true);
            await _cpuCoolerSocketSupportRepository.InsertAsync(
                new CpuCoolerSocketSupport { CpuCoolerSpecificationId = spec_cooler2.Id, SocketId = am4.Id },
                autoSave: true
            );
            await _cpuCoolerSocketSupportRepository.InsertAsync(
                new CpuCoolerSocketSupport { CpuCoolerSpecificationId = spec_cooler2.Id, SocketId = lga1700.Id },
                autoSave: true
            );
            #endregion

            #region Headset Products
            var productHeadset1 = await CreateProductAsync(
                headsets.Id, logitech.Id, 1500000, 5,
                "Logitech G Pro X",
                "High-quality gaming headset",
                70,
                "https://product.hstatic.net/200000722513/product/gvn_logitech_prox_79c556630c454086baf1bee06c577ab7_3471d9d886fd4dbe8ab5ae6bed9f4d78_grande.png"
            );
            await _headsetSpecRepository.InsertAsync(new HeadsetSpecification
            {
                ProductId = productHeadset1.Id,
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

            var productHeadset2 = await CreateProductAsync(
                headsets.Id, hyperx.Id, 1200000, 5,
                "HyperX Cloud Stinger Core II",
                "Comfortable gaming headset with 7.1 surround",
                60,
                "https://product.hstatic.net/200000722513/product/thumbtainghe_499f42bf16fe47d28ab00bffb7bd5748_47730811ddaf40a0a969f4e4d49c7b27_grande.png"
            );
            await _headsetSpecRepository.InsertAsync(new HeadsetSpecification
            {
                ProductId = productHeadset2.Id,
                Connectivity = ConnectivityType.Wired,
                DriverSize = 53,
                HasMicrophone = true,
                IsNoiseCancelling = true,
                IsSurroundSound = true,
                Frequency = "15Hz - 25kHz",
                MicrophoneType = "Detachable",
                Impedance = 60,
                Sensitivity = 98,
                Color = "Black"
            }, autoSave: true);
            #endregion

            #region Laptop Products
            var productLaptop1 = await CreateProductAsync(
                laptops.Id, asus.Id, 25000000, 12,
                "ASUS ROG Zephyrus G16 GU605CX QR083W",
                "Powerful gaming laptop",
                10,
                "https://product.hstatic.net/200000722513/product/zephyrus_g16_gu605_grey_03_rgb_1_b58d513a9306445daf2980232fe2544b_grande.png"
            );
            await _laptopSpecRepository.InsertAsync(new LaptopSpecification
            {
                ProductId = productLaptop1.Id,
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

            var productLaptop2 = await CreateProductAsync(
                laptops.Id, dell.Id, 60000000, 10,
                "Dell XPS 13 9310",
                "Ultra-portable 13\" laptop",
                8,
                "https://product.hstatic.net/200000722513/product/51529_laptop_dell_xps_9350_xps93_1d46c518185a488a92c40932dd4d5cf6_grande.png"
            );
            await _laptopSpecRepository.InsertAsync(new LaptopSpecification
            {
                ProductId = productLaptop2.Id,
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
            #endregion

            #region Motherboard Products
            var motherboardProduct1 = await CreateProductAsync(
                motherboards.Id, gigabyte.Id, 8000000, 11,
                "GIGABYTE Z790 AORUS XTREME X ICE",
                "Premium E-ATX Motherboard",
                20,
                "https://product.hstatic.net/200000722513/product/z790_aorus_xtreme_x_ice-01_5a397436688c4f2e9dc0e358ebf25927_grande.png"
            );
            await _motherboardSpecRepository.InsertAsync(new MotherboardSpecification
            {
                ProductId = motherboardProduct1.Id,
                SocketId = lga1700.Id,
                ChipsetId = z790.Id,
                FormFactorId = eatx.Id,
                RamSlots = 4,
                MaxRam = 192,
                RamTypeId = ddr5.Id,
                M2Slots = 5,
                SataPorts = 6,
                HasWifi = true
            }, autoSave: true);

            var motherboardProduct2 = await CreateProductAsync(
                motherboards.Id, msi.Id, 44000000, 15,
                "MSI MEG Z890 GODLIKE",
                "Extreme Performance Motherboard",
                15,
                "https://product.hstatic.net/200000722513/product/msi-meg_z890_godlike_3d2_rgb_b691f05efbcf45e58c54ab731ea28136_grande.png"
            );
            await _motherboardSpecRepository.InsertAsync(new MotherboardSpecification
            {
                ProductId = motherboardProduct2.Id,
                SocketId = lga1851.Id,
                ChipsetId = z890.Id,
                FormFactorId = eatx.Id,
                RamSlots = 4,
                MaxRam = 192,
                RamTypeId = ddr5.Id,
                M2Slots = 6,
                SataPorts = 6,
                HasWifi = true
            }, autoSave: true);

            var motherboardProduct3 = await CreateProductAsync(
                motherboards.Id, asus.Id, 2100000, 0,
                "ASUS ROG Crosshair X670E Hero",
                "High-end AMD Motherboard",
                12,
                "https://product.hstatic.net/200000722513/product/rog-crosshair-x870e-hero-01_5ab538b8eb38470a83ff1a122393bd26_grande.jpg"
            );
            await _motherboardSpecRepository.InsertAsync(new MotherboardSpecification
            {
                ProductId = motherboardProduct3.Id,
                SocketId = am5.Id,
                ChipsetId = x670e.Id,
                FormFactorId = atx.Id,
                RamSlots = 4,
                MaxRam = 192,
                RamTypeId = ddr5.Id,
                M2Slots = 5,
                SataPorts = 6,
                HasWifi = true
            }, autoSave: true);

            var motherboardProduct4 = await CreateProductAsync(
                motherboards.Id, asrock.Id, 28000000, 0,
                "ASRock X670E Taichi AQUA",
                "High-end Watercooled Motherboard",
                0,
                "https://www.asrock.com/mb/photo/X670E%20Taichi(M1).png"
            );
            await _motherboardSpecRepository.InsertAsync(new MotherboardSpecification
            {
                ProductId = motherboardProduct4.Id,
                SocketId = am5.Id,
                ChipsetId = x670e.Id,
                FormFactorId = atx.Id,
                RamSlots = 4,
                MaxRam = 192,
                RamTypeId = ddr5.Id,
                M2Slots = 5,
                SataPorts = 8,
                HasWifi = true
            }, autoSave: true);
            #endregion

            #region Mouse Products
            var productMouse1 = await CreateProductAsync(
                mice.Id, logitech.Id, 1000000, 5,
                "Logitech G502 HERO",
                "High-performance gaming mouse",
                150,
                "https://product.hstatic.net/200000722513/product/10001_01736316d2b443d0838e5a0741434420_grande.png"
            );
            await _mouseSpecRepository.InsertAsync(new MouseSpecification
            {
                ProductId = productMouse1.Id,
                Dpi = 16000,
                PollingRate = 1000,
                SensorType = "Optical",
                Weight = 80,
                Connectivity = ConnectivityType.Wired,
                ButtonCount = 6,
                BacklightColor = "RGB",
                Color = "Black"
            }, autoSave: true);

            var productMouse2 = await CreateProductAsync(
                mice.Id, razer.Id, 1500000, 10,
                "Razer DeathAdder V2",
                "Iconic ergonomic gaming mouse",
                120,
                "https://assets2.razerzone.com/images/pnx.assets/14f056ebbd06023cdd8b1351d17cbdaf/razer-deathadder-v2-gallery01.jpg"
            );
            await _mouseSpecRepository.InsertAsync(new MouseSpecification
            {
                ProductId = productMouse2.Id,
                Dpi = 20000,
                PollingRate = 1000,
                SensorType = "Optical",
                Weight = 82,
                Connectivity = ConnectivityType.Wired,
                ButtonCount = 8,
                BacklightColor = "RGB",
                Color = "Black"
            }, autoSave: true);

            var productMouse3 = await CreateProductAsync(
                mice.Id, steelseries.Id, 800000, 20,
                "SteelSeries Rival 3",
                "Lightweight RGB gaming mouse",
                120,
                "https://product.hstatic.net/200000722513/product/thumbchuot_e01eec6957cc40a88aba550b80cffed2_74ec8f2dec0447c382614fa201a4fa93_grande.png"
            );
            await _mouseSpecRepository.InsertAsync(new MouseSpecification
            {
                ProductId = productMouse3.Id,
                Dpi = 8500,
                PollingRate = 1000,
                SensorType = "Optical",
                Weight = 77,
                Connectivity = ConnectivityType.Wired,
                ButtonCount = 6,
                BacklightColor = "RGB",
                Color = "White"
            }, autoSave: true);

            var productMouse4 = await CreateProductAsync(
                mice.Id, logitech.Id, 3500000, 5,
                "Logitech G Pro X Superlight",
                "Chuột không dây siêu nhẹ dành cho esports, pin trâu, cảm biến HERO 25K.",
                60,
                "https://resource.logitechg.com/w_600,c_limit,q_auto,f_auto,dpr_1.0/g_pro_x_superlight/gallery-1.png"
            );
            await _mouseSpecRepository.InsertAsync(new MouseSpecification
            {
                ProductId = productMouse4.Id,
                Dpi = 25000,
                PollingRate = 1000,
                SensorType = "Optical",
                Weight = 63,
                Connectivity = ConnectivityType.Wireless,
                ButtonCount = 5,
                BacklightColor = "RGB",
                Color = "White"
            }, autoSave: true);

            var productMouse5 = await CreateProductAsync(
                mice.Id, coolerMaster.Id, 1200000, 8,
                "Cooler Master MM710",
                "Chuột lỗ siêu nhẹ 53g, cảm biến PMW3389, thiết kế siêu bền.",
                90,
                "https://coolermaster.com/media/catalog/product/cache/1/image/1200x1200/9df78eab33525d08d6e5fb8d27136e95/mm710-gallery01.png"
            );
            await _mouseSpecRepository.InsertAsync(new MouseSpecification
            {
                ProductId = productMouse5.Id,
                Dpi = 16000,
                PollingRate = 1000,
                SensorType = "Optical",
                Weight = 53,
                Connectivity = ConnectivityType.Wired,
                ButtonCount = 6,
                BacklightColor = "No",
                Color = "White"
            }, autoSave: true);

            var productMouse6 = await CreateProductAsync(
                mice.Id, steelseries.Id, 700000, 12,
                "SteelSeries Rival 3 (Black)",
                "Chuột gaming phổ thông với cảm biến TrueMove Core, bền bỉ, RGB tùy chỉnh.",
                200,
                "https://steelseries.com/cdn/shop/products/Rival3_ProductImage_1_grande.png"
            );
            await _mouseSpecRepository.InsertAsync(new MouseSpecification
            {
                ProductId = productMouse6.Id,
                Dpi = 8500,
                PollingRate = 1000,
                SensorType = "Optical",
                Weight = 77,
                Connectivity = ConnectivityType.Wired,
                ButtonCount = 6,
                BacklightColor = "RGB",
                Color = "Black"
            }, autoSave: true);

            var productMouse7 = await CreateProductAsync(
                mice.Id, hyperx.Id, 1100000, 9,
                "HyperX Pulsefire Haste",
                "Chuột siêu nhẹ 59g, honeycomb shell, dây HyperFlex không vướng.",
                140,
                "https://hyperx.com/media/catalog/product/cache/1/image/1200x/9df78eab33525d08d6e5fb8d27136e95/p/u/pulsefire-haste-gallery01.png"
            );
            await _mouseSpecRepository.InsertAsync(new MouseSpecification
            {
                ProductId = productMouse7.Id,
                Dpi = 16000,
                PollingRate = 1000,
                SensorType = "Optical",
                Weight = 59,
                Connectivity = ConnectivityType.Wired,
                ButtonCount = 6,
                BacklightColor = "RGB",
                Color = "Pink"
            }, autoSave: true);

            var productMouse8 = await CreateProductAsync(
                mice.Id, asus.Id, 2200000, 6,
                "ASUS ROG Gladius III",
                "Chuột gaming đa kết nối, switch Omron 80M click, RGB AURA Sync.",
                75,
                "https://dlcdnrog.asus.com/rog/media/1670017436852.png"
            );
            await _mouseSpecRepository.InsertAsync(new MouseSpecification
            {
                ProductId = productMouse8.Id,
                Dpi = 19000,
                PollingRate = 1000,
                SensorType = "Optical",
                Weight = 89,
                Connectivity = ConnectivityType.Wired,
                ButtonCount = 6,
                BacklightColor = "ARGB",
                Color = "Black"
            }, autoSave: true);

            var productMouse9 = await CreateProductAsync(
                mice.Id, msi.Id, 1300000, 10,
                "MSI Clutch GM41 Lightweight",
                "Chuột siêu nhẹ 65g, cảm biến PixArt 3389, vỏ lỗ thoáng khí.",
                100,
                "https://asset.msi.com/resize/image/global/product/product_5_20220224105048_61f9d024d22a7.png"
            );

            await _mouseSpecRepository.InsertAsync(new MouseSpecification
            {
                ProductId = productMouse9.Id,
                Dpi = 19000,
                PollingRate = 8000,
                SensorType = "Optical",
                Weight = 65,
                Connectivity = ConnectivityType.Wired,
                ButtonCount = 6,
                BacklightColor = "RGB",
                Color = "Black"
            }, autoSave: true);

            var productMouse10 = await CreateProductAsync(
                mice.Id, gigabyte.Id, 1400000, 8,
                "GIGABYTE AORUS M4",
                "Chuột gaming tản nhiệt tốt, switch Omron, RGB ARGB 16.7 triệu màu.",
                110,
                "https://www.gigabyte.com/FileUpload/Image/Global/Products/m4-gallery-01.png"
            );
            await _mouseSpecRepository.InsertAsync(new MouseSpecification
            {
                ProductId = productMouse10.Id,
                Dpi = 18000,
                PollingRate = 1000,
                SensorType = "Optical",
                Weight = 85,
                Connectivity = ConnectivityType.Wired,
                ButtonCount = 7,
                BacklightColor = "ARGB",
                Color = "White"
            }, autoSave: true);


            var productMouse_WiredWireless1 = await CreateProductAsync(mice.Id, razer.Id, 3200000, 15, "Razer Basilisk Ultimate", "Chuột gaming không dây hiệu năng cao với dock sạc, hỗ trợ cả hai chế độ kết nối có dây và không dây 2.4GHz.", 55, "https://assets2.razerzone.com/images/pnx.assets/d20a4597d86f7284897d51a667364b18/razer-basilisk-ultimate-gallery-1.jpg");
            await _mouseSpecRepository.InsertAsync(new MouseSpecification
            {
                ProductId = productMouse_WiredWireless1.Id,
                Dpi = 20000,
                PollingRate = 1000,
                SensorType = "Razer Focus+",
                Weight = 107,
                Connectivity = ConnectivityType.WiredAndWireless, // Sử dụng enum
                ButtonCount = 11,
                BacklightColor = "RGB Chroma",
                Color = "Black"
            }, autoSave: true);
            var productMouse_MultiMode = await CreateProductAsync(
                mice.Id,
                razer.Id, // Giả sử 'razer' đã được khởi tạo
                4200000,
                10,
                "Razer Naga V2 Pro",
                "Chuột MMO đỉnh cao với các tấm panel bên hông có thể thay đổi. Hỗ trợ kết nối HyperSpeed Wireless siêu tốc cho gaming, Bluetooth để tiết kiệm pin hoặc kết nối đa thiết bị, và chế độ có dây Speedflex.",
                30,
                "https://assets2.razerzone.com/images/pnx.assets/d7b5c3b0d5c41441b8f0dbde53d5f573/razer-naga-v2-pro-500x500.png"
            );

            await _mouseSpecRepository.InsertAsync(new MouseSpecification
            {
                ProductId = productMouse_MultiMode.Id,
                Dpi = 30000,
                PollingRate = 1000,
                SensorType = "Focus Pro 30K Optical",
                Weight = 134,
                Connectivity = ConnectivityType.WiredAndWireless,
                ButtonCount = 19,
                BacklightColor = "Razer Chroma RGB",
                Color = "Black"
            }, autoSave: true);
            #endregion
            #endregion



            #region New Category Products

            // CaseFan Product
            var productCaseFan1 = await CreateProductAsync(
                caseFans.Id, noctua.Id, 450000, 5,
                "Noctua NF-A12x25 PWM",
                "Quạt tản nhiệt case 120mm cao cấp, êm ái",
                100,
                "https://noctua.at/pub/media/catalog/product/cache/74c1057f7991b4edb2bc7bdaa94de933/n/f/nf_a12x25_pwm_1.jpg"
            );
            await _caseFanSpecRepository.InsertAsync(new CaseFanSpecification
            {
                ProductId = productCaseFan1.Id,
                FanSize = 120,
                MaxRpm = 2000,
                NoiseLevel = 22.6f,
                Airflow = 102.1f,
                StaticPressure = 2.34f,
                Connector = "4-pin PWM",
                BearingType = "SSO2",
                HasRgb = false,
                Color = "Brown/Beige"
            }, autoSave: true);

            // MemoryCard Product
            var productMemoryCard1 = await CreateProductAsync(
                memoryCards.Id, samsung.Id, 350000, 8,
                "Samsung EVO Plus microSD 128GB",
                "Thẻ nhớ tốc độ cao, chống nước, chống sốc",
                200,
                "https://images.samsung.com/is/image/samsung/p6pim/vn/mb-mc128ka-apc/gallery/vn-microsdxc-evo-plus-mb-mc128ka-535234-mb-mc128ka-apc-538392334"
            );
            await _memoryCardSpecRepository.InsertAsync(new MemoryCardSpecification
            {
                ProductId = productMemoryCard1.Id,
                Capacity = 128,
                CardType = "microSDXC",
                SpeedClass = "U3, V30, A2",
                ReadSpeed = 130,
                WriteSpeed = 90,
                Warranty = "10 năm",
                Waterproof = true,
                Shockproof = true
            }, autoSave: true);

            // Speaker Product
            var productSpeaker1 = await CreateProductAsync(
                speakers.Id, logitech.Id, 1500000, 10,
                "Logitech Z623 2.1",
                "Loa 2.1 THX công suất 200W, bass mạnh mẽ",
                45,
                "https://resource.logitech.com/w_800,c_lpad,ar_1:1,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/speakers/z623/gallery/z623-gallery-1.png"
            );
            await _speakerSpecRepository.InsertAsync(new SpeakerSpecification
            {
                ProductId = productSpeaker1.Id,
                SpeakerType = "2.1 (Stereo with Subwoofer)",
                TotalWattage = 200,
                Frequency = "35Hz - 20kHz",
                Connectivity = ConnectivityType.Wired,
                InputPorts = "3.5mm, RCA",
                HasBluetooth = false,
                HasRemote = true,
                Color = "Black"
            }, autoSave: true);

            // Microphone Product
            var productMicrophone1 = await CreateProductAsync(
                microphones.Id, hyperx.Id, 1800000, 5,
                "HyperX QuadCast S",
                "Micro USB chống rung, RGB, 4 polar pattern",
                60,
                "https://row.hyperx.com/cdn/shop/files/hyperx_quadcast_s_mic_1_top_down_zm_lg.jpg"
            );
            await _microphoneSpecRepository.InsertAsync(new MicrophoneSpecification
            {
                ProductId = productMicrophone1.Id,
                MicrophoneType = "Condenser",
                PolarPattern = "Cardioid, Bidirectional, Omnidirectional, Stereo",
                Frequency = "20Hz - 20kHz",
                SampleRate = "48kHz/16-bit",
                Sensitivity = "-36dB",
                Connectivity = ConnectivityType.Wired,
                Connection = "USB Type-C",
                HasShockMount = true,
                HasPopFilter = true,
                HasRgb = true,
                Color = "Black/Red"
            }, autoSave: true);

            // Webcam Product
            var productWebcam1 = await CreateProductAsync(
                webcams.Id, logitech.Id, 2500000, 12,
                "Logitech StreamCam",
                "Webcam 1080p 60fps cho streaming, USB-C",
                50,
                "https://resource.logitech.com/w_800,c_lpad,ar_1:1,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/webcams/streamcam/gallery/streamcam-gallery-1-graphite.png"
            );
            await _webcamSpecRepository.InsertAsync(new WebcamSpecification
            {
                ProductId = productWebcam1.Id,
                Resolution = "1920x1080",
                FrameRate = 60,
                FocusType = "Auto Focus",
                FieldOfView = 78,
                Connectivity = ConnectivityType.Wired,
                Connection = "USB Type-C",
                HasMicrophone = true,
                HasPrivacyShutter = false,
                MountType = "Monitor Clip, Tripod",
                Color = "Graphite"
            }, autoSave: true);

            // MousePad Product
            var productMousePad1 = await CreateProductAsync(
                mousePads.Id, steelseries.Id, 800000, 15,
                "SteelSeries QcK Heavy XXL",
                "Lót chuột vải lớn 900x400mm, dày 6mm",
                120,
                "https://media.steelseriescdn.com/thumbs/catalogue/products/00431-qck-heavy-xxl/c0eb6b6563984f2fab338c58e37b0ee1.png.500x400_q100_crop-fit_optimize.png"
            );
            await _mousePadSpecRepository.InsertAsync(new MousePadSpecification
            {
                ProductId = productMousePad1.Id,
                Width = 900,
                Height = 400,
                Thickness = 6,
                Material = "Cloth",
                SurfaceType = "Smooth fabric",
                BaseType = "Non-slip rubber",
                HasRgb = false,
                IsWashable = true,
                Color = "Black"
            }, autoSave: true);

            // Chair Product
            var productChair1 = await CreateProductAsync(
                chairs.Id, corsair.Id, 8500000, 10,
                "Corsair TC100 Relaxed",
                "Ghế gaming vải thoáng khí, êm ái",
                25,
                "https://assets.corsair.com/image/upload/f_auto,q_auto/content/TC100-RELAXED-BLACK-Gallery-Hero-01.png"
            );
            await _chairSpecRepository.InsertAsync(new ChairSpecification
            {
                ProductId = productChair1.Id,
                ChairType = "Gaming/Office",
                Material = "Fabric",
                MaxWeight = 120,
                ArmrestType = "3D Adjustable",
                BackrestAdjustment = "Recline 90°-160°",
                SeatHeight = "42-52cm",
                HasLumbarSupport = true,
                HasHeadrest = true,
                BaseType = "Nylon 5-star",
                WheelType = "60mm PU caster",
                Color = "Black"
            }, autoSave: true);

            // Desk Product
            var productDesk1 = await CreateProductAsync(
                desks.Id, ikea.Id, 3500000, 5,
                "IKEA UPPSPEL Gaming Desk",
                "Bàn gaming 140x80cm, có hệ thống quản lý dây",
                15,
                "https://www.ikea.com/us/en/images/products/uppspel-gaming-desk-black__0981784_pe815503_s5.jpg"
            );
            await _deskSpecRepository.InsertAsync(new DeskSpecification
            {
                ProductId = productDesk1.Id,
                Width = 140,
                Depth = 80,
                Height = 73,
                Material = "Particle board, Steel",
                MaxWeight = 50,
                IsHeightAdjustable = false,
                HasCableManagement = true,
                HasCupHolder = false,
                HasHeadphoneHook = true,
                SurfaceType = "Smooth laminate",
                Color = "Black"
            }, autoSave: true);

            // Software Product
            var productSoftware1 = await CreateProductAsync(
                software.Id, microsoft.Id, 3200000, 0,
                "Windows 11 Pro",
                "Hệ điều hành Windows 11 Pro bản quyền",
                500,
                "https://cdn-dynmedia-1.microsoft.com/is/image/microsoftcorp/RWZQBF_Hero_960x540_2x_RE4VnJV"
            );
            await _softwareSpecRepository.InsertAsync(new SoftwareSpecification
            {
                ProductId = productSoftware1.Id,
                SoftwareType = "Operating System",
                LicenseType = "Retail",
                Platform = "Windows",
                Version = "22H2",
                Language = "Multi-language",
                DeliveryMethod = "Digital Download/USB",
                SystemRequirements = "64-bit processor, 4GB RAM, 64GB storage",
                IsSubscription = false
            }, autoSave: true);

            // NetworkHardware Product
            var productNetwork1 = await CreateProductAsync(
                networkHardware.Id, tplink.Id, 1200000, 8,
                "TP-Link Archer AX55 AX3000",
                "Router WiFi 6 dual-band, tốc độ 3Gbps",
                40,
                "https://static.tp-link.com/upload/product-overview/2021/202106/20210625155708974.png"
            );
            await _networkHardwareSpecRepository.InsertAsync(new NetworkHardwareSpecification
            {
                ProductId = productNetwork1.Id,
                DeviceType = "WiFi Router",
                WifiStandard = "WiFi 6 (802.11ax)",
                MaxSpeed = "3000 Mbps",
                Frequency = "2.4GHz & 5GHz",
                EthernetPorts = "4x Gigabit LAN, 1x Gigabit WAN",
                AntennaCount = 4,
                HasUsb = false,
                SecurityProtocol = "WPA3",
                Coverage = "Up to 2500 sq ft"
            }, autoSave: true);

            // Handheld Product
            var productHandheld1 = await CreateProductAsync(
                handhelds.Id, valve.Id, 12000000, 5,
                "Steam Deck 512GB",
                "Máy chơi game cầm tay, chạy SteamOS",
                20,
                "https://cdn.cloudflare.steamstatic.com/steamdeck/images/hero/hero_deck_fullbleed.jpg"
            );
            await _handheldSpecRepository.InsertAsync(new HandheldSpecification
            {
                ProductId = productHandheld1.Id,
                Processor = "AMD Zen 2 4c/8t",
                Graphics = "AMD RDNA 2 8 CUs",
                RAM = "16GB LPDDR5",
                Storage = "512GB NVMe SSD",
                Display = "7-inch 1280x800 LCD 60Hz",
                BatteryLife = "2-8 hours",
                Weight = "669g",
                OperatingSystem = "SteamOS 3.0",
                Connectivity = "WiFi 5, Bluetooth 5.0"
            }, autoSave: true);

            // Console Product
            var productConsole1 = await CreateProductAsync(
                consoles.Id, sony.Id, 13000000, 8,
                "PlayStation 5 Digital Edition",
                "Máy chơi game thế hệ mới, bản kỹ thuật số",
                30,
                "https://gmedia.playstation.com/is/image/SIEPDC/ps5-digital-edition-console-image-block-01-en-25jun20"
            );
            await _consoleSpecRepository.InsertAsync(new ConsoleSpecification
            {
                ProductId = productConsole1.Id,
                Processor = "AMD Zen 2 8-core",
                Graphics = "AMD RDNA 2 10.28 TFLOPS",
                RAM = "16GB GDDR6",
                Storage = "825GB SSD",
                OpticalDrive = "None (Digital)",
                MaxResolution = "4K UHD",
                MaxFrameRate = "120fps",
                HDRSupport = true,
                Connectivity = "WiFi 6, Bluetooth 5.1, Gigabit Ethernet"
            }, autoSave: true);

            // Hub (Docking Station) Product
            var productHub1 = await CreateProductAsync(
                hubs.Id, anker.Id, 2500000, 12,
                "Anker PowerExpand Elite 13-in-1",
                "Docking station USB-C 13 cổng, 85W PD",
                35,
                "https://d2j6dbq0eux0bg.cloudfront.net/images/66203968/2624850885.jpg"
            );
            await _hubSpecRepository.InsertAsync(new HubSpecification
            {
                ProductId = productHub1.Id,
                HubType = "Thunderbolt 3 / USB-C Dock",
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

            // Cable Product
            var productCable1 = await CreateProductAsync(
                cables.Id, ugreen.Id, 250000, 5,
                "UGREEN USB-C to USB-C 100W Cable 2m",
                "Cáp USB-C hỗ trợ sạc nhanh 100W, truyền dữ liệu",
                200,
                "https://i5.walmartimages.com/asr/c6f8f7c8-c0f5-4c8e-9d0e-1f7e0e0e0e0e.jpg"
            );
            await _cableSpecRepository.InsertAsync(new CableSpecification
            {
                ProductId = productCable1.Id,
                CableType = "USB-C to USB-C",
                Length = 2.0f,
                MaxPower = "100W",
                DataTransferSpeed = "480 Mbps (USB 2.0)",
                Connector1 = "USB Type-C",
                Connector2 = "USB Type-C",
                IsBraided = true,
                Color = "Black",
                Warranty = "2 năm"
            }, autoSave: true);

            // Charger Product
            var productCharger1 = await CreateProductAsync(
                chargers.Id, anker.Id, 1200000, 15,
                "Anker 737 Charger GaNPrime 120W",
                "Sạc nhanh 3 cổng USB-C + USB-A, GaN công nghệ mới",
                80,
                "https://m.media-amazon.com/images/I/61Zfh+vkVoL._AC_SL1500_.jpg"
            );
            await _chargerSpecRepository.InsertAsync(new ChargerSpecification
            {
                ProductId = productCharger1.Id,
                ChargerType = "Wall Charger",
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

            // PowerBank Product
            var productPowerBank1 = await CreateProductAsync(
                powerBanks.Id, anker.Id, 2800000, 10,
                "Anker 737 Power Bank 24000mAh",
                "Sạc dự phòng 140W, màn hình LED hiển thị",
                50,
                "https://m.media-amazon.com/images/I/61SfU5bj9uL._AC_SL1500_.jpg"
            );
            await _powerBankSpecRepository.InsertAsync(new PowerBankSpecification
            {
                ProductId = productPowerBank1.Id,
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

            #endregion
        }




        private async Task<Product> CreateProductAsync(Guid categoryId, Guid manufacturerId, decimal price,
                                                        double discount, string name, string desc,
                                                        int stock, string imageUrl)
        {
            var product = new Product
            {
                CategoryId = categoryId,
                ManufacturerId = manufacturerId,
                OriginalPrice = price,
                DiscountPercent = discount,
                ProductName = name,
                Description = desc,
                StockCount = stock,
                ImageUrl = imageUrl,
                UrlSlug = UrlHelperMethod.RemoveDiacritics(name)
            };
            return await _productRepository.InsertAsync(product, autoSave: true);
        }

    }
}

