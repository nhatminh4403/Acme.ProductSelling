using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Data.BaseSeeder;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Specifications.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Data.Products
{
    public class ComputerComponentSeeder : ProductSeederBase, IDataSeederContributor
    {
        private readonly IRepository<CpuSpecification, Guid> _cpuSpecRepository;
        private readonly IRepository<GpuSpecification, Guid> _gpuSpecRepository;
        private readonly IRepository<RamSpecification, Guid> _ramSpecRepository;
        private readonly IRepository<StorageSpecification, Guid> _storageSpecRepository;
        private readonly IRepository<MotherboardSpecification, Guid> _motherboardSpecRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;
        private Dictionary<string, CpuSocket> _sockets;
        private Dictionary<string, Chipset> _chipsets;
        private Dictionary<string, FormFactor> _formFactors;
        private Dictionary<string, RamType> _ramTypes;

        public ComputerComponentSeeder(
            IProductRepository productRepository,
            IRepository<CpuSpecification, Guid> cpuSpecRepository,
            IRepository<GpuSpecification, Guid> gpuSpecRepository,
            IRepository<RamSpecification, Guid> ramSpecRepository, ProductManager productManager,
            IRepository<StorageSpecification, Guid> storageSpecRepository,
            IRepository<MotherboardSpecification, Guid> motherboardSpecRepository)
            : base(productRepository, productManager)
        {
            _cpuSpecRepository = cpuSpecRepository;
            _gpuSpecRepository = gpuSpecRepository;
            _ramSpecRepository = ramSpecRepository;
            _storageSpecRepository = storageSpecRepository;
            _motherboardSpecRepository = motherboardSpecRepository;
        }

        public void SetDependencies(
           Dictionary<string, Category> categories,
           Dictionary<string, Manufacturer> manufacturers,
           Dictionary<string, CpuSocket> sockets,
           Dictionary<string, Chipset> chipsets,
           Dictionary<string, FormFactor> formFactors,
           Dictionary<string, RamType> ramTypes)
        {
            _categories = categories;
            _manufacturers = manufacturers;
            _sockets = sockets;
            _chipsets = chipsets;
            _formFactors = formFactors;
            _ramTypes = ramTypes;
        }


        public async Task SeedAsync()
        {
            await SeedCPUsAsync();
            await SeedGPUsAsync();
            await SeedRAMsAsync();
            await SeedStorageAsync();
            await SeedMotherboardsAsync();
        }
        private async Task SeedCPUsAsync()
        {
            var cpu1 = await CreateProductAsync(
                _categories["CPUs"].Id, _manufacturers["AMD"].Id, 8500000, 10,
                "Ryzen 7 7700X", "Powerful 8-core CPU",
                50, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/ryzen_7_-_1_00957bbe7b8542308c897a90d439b1fd_e1c9a16c537d47bb9768828dddb332d0_grande.jpg");

            await _cpuSpecRepository.InsertAsync(new CpuSpecification
            {
                ProductId = cpu1.Id,
                SocketId = _sockets["AM5"].Id,
                CoreCount = 8,
                ThreadCount = 16,
                BaseClock = 4.5f,
                BoostClock = 5.4f,
                L3Cache = 32,
                Tdp = 105,
                HasIntegratedGraphics = true
            }, autoSave: true);

            var cpu2 = await CreateProductAsync(
                _categories["CPUs"].Id, _manufacturers["Intel"].Id, 13500000, 7,
                "Intel Core i9-13900K", "Top-tier 24-core CPU",
                30, true, DateTime.Now.AddDays(7),
                "https://product.hstatic.net/200000722513/product/i9k-t2-special-box-07-1080x1080pixels_6c9ec1001cdf4e4998c13af4ac6c7581_114c47698e4a4984863c3b26e0619b65_grande.png");

            await _cpuSpecRepository.InsertAsync(new CpuSpecification
            {
                ProductId = cpu2.Id,
                SocketId = _sockets["LGA1700"].Id,
                CoreCount = 24,
                ThreadCount = 32,
                BaseClock = 3.0f,
                BoostClock = 5.8f,
                L3Cache = 36,
                Tdp = 125,
                HasIntegratedGraphics = true
            }, autoSave: true);
        }

        private async Task SeedGPUsAsync()
        {
            var gpu1 = await CreateProductAsync(
                _categories["GPUs"].Id, _manufacturers["ASUS"].Id, 25000000, 5,
                "ProArt GeForce RTX 4070 Ti SUPER 16GB", "High-end graphics card",
                20, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/fwebp__10__1d22cf39c094494bb772b5bb1c002172_grande.png");

            await _gpuSpecRepository.InsertAsync(new GpuSpecification
            {
                ProductId = gpu1.Id,
                Chipset = "GeForce RTX 4070 Ti",
                MemorySize = 12,
                MemoryType = "GDDR6X",
                BoostClock = 2610f,
                Interface = "PCIe 4.0 x16",
                RecommendedPsu = 700,
                Length = 310f
            }, autoSave: true);

            var gpu2 = await CreateProductAsync(
                _categories["GPUs"].Id, _manufacturers["ASUS"].Id, 28000000, 10,
                "Asus Radeon RX 7900 XT TUF Gaming", "High-end AMD graphics card",
                15, true, DateTime.Now.AddDays(5),
                "https://product.hstatic.net/200000722513/product/5681_ea11053c19e375dcaa8138b6f531262d_7d029f536978405393da9fb3c8f1e2fa_4d3cedb8fd4a485db1ece7519c1d41a8_grande.jpg");

            await _gpuSpecRepository.InsertAsync(new GpuSpecification
            {
                ProductId = gpu2.Id,
                Chipset = "Radeon RX 7900 XT",
                MemorySize = 20,
                MemoryType = "GDDR6",
                BoostClock = 2600f,
                Interface = "PCIe 4.0 x16",
                RecommendedPsu = 750,
                Length = 310f
            }, autoSave: true);
        }

        private async Task SeedRAMsAsync()
        {
            var ram1 = await CreateProductAsync(
                _categories["RAMs"].Id, _manufacturers["Corsair"].Id, 2000000, 0,
                "Vengeance LPX 16GB", "High-performance RAM",
                100, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/gearvn-corsair-vengeance-rgb-ddr-5600-ddr5-5_6e319950a8e14231b28a416076c94951_grande.png");

            await _ramSpecRepository.InsertAsync(new RamSpecification
            {
                ProductId = ram1.Id,
                RamTypeId = _ramTypes["DDR5"].Id,
                Capacity = 32,
                Speed = 6000,
                ModuleCount = 2,
                Timing = "36-36-36-76",
                Voltage = 1.35f,
                HasRGB = true
            }, autoSave: true);

            var ram2 = await CreateProductAsync(
                _categories["RAMs"].Id, _manufacturers["G.Skill"].Id, 1800000, 5,
                "Trident Z RGB 16GB (2×8)", "DDR4 3600MHz kit with RGB",
                80, true, DateTime.Now.AddDays(-3),
                "https://anphat.com.vn/media/product/33685_153665426813.png");

            await _ramSpecRepository.InsertAsync(new RamSpecification
            {
                ProductId = ram2.Id,
                RamTypeId = _ramTypes["DDR4"].Id,
                Capacity = 16,
                Speed = 3600,
                ModuleCount = 2,
                Timing = "18-22-22-42",
                Voltage = 1.35f,
                HasRGB = true
            }, autoSave: true);
        }

        private async Task SeedStorageAsync()
        {
            var storage1 = await CreateProductAsync(
                _categories["Storages"].Id, _manufacturers["Samsung"].Id, 3000000, 8,
                "970 EVO Plus 1TB", "Fast NVMe SSD",
                80, true, DateTime.Now,
                "https://bizweb.dktcdn.net/thumb/grande/100/329/122/products/970-evo-plus-1tb-01-1689929004911.jpg?v=1695052940103");

            await _storageSpecRepository.InsertAsync(new StorageSpecification
            {
                ProductId = storage1.Id,
                StorageType = StorageType.NvmeSsd,
                Interface = "PCIe 3.0 x4",
                Capacity = 1000,
                ReadSpeed = 3500,
                WriteSpeed = 3300,
                StorageFormFactor = StorageFormFactor.SsdM2_2280
            }, autoSave: true);

            var storage2 = await CreateProductAsync(
                _categories["Storages"].Id, _manufacturers["WD"].Id, 2500000, 5,
                "WD Black SN770 1TB", "High-performance PCIe 4.0 NVMe SSD",
                60, true, DateTime.Now.AddDays(2),
                "https://bizweb.dktcdn.net/thumb/grande/100/329/122/products/ssd-wd-black-sn770-pcie-gen4-x4-nvme-m-2-500gb-wds500g3x0e-b058273a-af63-4053-ac31-83b41eb593a2.jpg?v=1655710957737");

            await _storageSpecRepository.InsertAsync(new StorageSpecification
            {
                ProductId = storage2.Id,
                StorageType = StorageType.NvmeSsd,
                Interface = "PCIe 4.0 x4",
                Capacity = 1000,
                ReadSpeed = 5150,
                WriteSpeed = 4900,
                StorageFormFactor = StorageFormFactor.SsdM2_2280
            }, autoSave: true);
        }

        private async Task SeedMotherboardsAsync()
        {
            var mb1 = await CreateProductAsync(
                _categories["Motherboards"].Id, _manufacturers["Gigabyte"].Id, 8000000, 11,
                "GIGABYTE Z790 AORUS XTREME X ICE", "Premium E-ATX Motherboard",
                20, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/z790_aorus_xtreme_x_ice-01_5a397436688c4f2e9dc0e358ebf25927_grande.png");

            await _motherboardSpecRepository.InsertAsync(new MotherboardSpecification
            {
                ProductId = mb1.Id,
                SocketId = _sockets["LGA1700"].Id,
                ChipsetId = _chipsets["Z790"].Id,
                FormFactorId = _formFactors["E-ATX"].Id,
                RamSlots = 4,
                MaxRam = 192,
                RamTypeId = _ramTypes["DDR5"].Id,
                M2Slots = 5,
                SataPorts = 6,
                HasWifi = true
            }, autoSave: true);

            var mb2 = await CreateProductAsync(
                _categories["Motherboards"].Id, _manufacturers["MSI"].Id, 44000000, 15,
                "MSI MEG Z890 GODLIKE", "Extreme Performance Motherboard",
                15, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/msi-meg_z890_godlike_3d2_rgb_b691f05efbcf45e58c54ab731ea28136_grande.png");

            await _motherboardSpecRepository.InsertAsync(new MotherboardSpecification
            {
                ProductId = mb2.Id,
                SocketId = _sockets["LGA1851"].Id,
                ChipsetId = _chipsets["Z890"].Id,
                FormFactorId = _formFactors["E-ATX"].Id,
                RamSlots = 4,
                MaxRam = 192,
                RamTypeId = _ramTypes["DDR5"].Id,
                M2Slots = 6,
                SataPorts = 6,
                HasWifi = true
            }, autoSave: true);
        }
    }
}
