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
    public class ComputerComponentSeeder : ProductSeederBase, IDataSeederContributor
    {
        private readonly ISpecificationRepository _specificationRepository;

        private Dictionary<string, Category> _categories;
        private Dictionary<string, Manufacturer> _manufacturers;
        private Dictionary<string, CpuSocket> _sockets;
        private Dictionary<string, Chipset> _chipsets;
        private Dictionary<string, FormFactor> _formFactors;
        private Dictionary<string, RamType> _ramTypes;

        public ComputerComponentSeeder(
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
            var cpu1 = await CreateProductAsync(categoryId: _categories["CPUs"].Id,
                                                manufacturerId: _manufacturers["AMD"].Id,
                                                price: 8500000,
                                                discount: 10,
                                                name: "Ryzen 7 7700X",
                                                description: "<p>CPU <strong>8 nhân 16 luồng</strong> mạnh mẽ từ AMD với kiến trúc Zen 4.</p><ul><li>8 nhân, 16 luồng xử lý đa nhiệm tốt</li><li>Xung nhịp base 4.5GHz, boost 5.4GHz</li><li>32MB L3 Cache</li><li>TDP 105W</li><li>Socket AM5 hiện đại</li><li>Tích hợp GPU Radeon</li><li>Lý tưởng cho gaming và sáng tạo nội dung</li></ul>",
                                                stock: 50,
                                                isActive: true,
                                                releaseDate: DateTime.Now,
                                                imageUrl: "https://product.hstatic.net/200000722513/product/ryzen_7_-_1_00957bbe7b8542308c897a90d439b1fd_e1c9a16c537d47bb9768828dddb332d0_grande.jpg");

            await _specificationRepository.InsertAsync(new CpuSpecification
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
                "Intel Core i9-13900K", "<p>CPU cao cấp <strong>24 nhân</strong> với hiệu năng <strong>đỉnh cao</strong> từ Intel.</p><ul><li>24 nhân (8P+16E), 32 luồng</li><li>Xung nhịp base 3.0GHz, boost 5.8GHz</li><li>36MB Intel Smart Cache</li><li>TDP 125W (PL1)</li><li>Socket LGA1700</li><li>Tích hợp Intel UHD Graphics 770</li><li>Hiệu năng gaming và workstation hàng đầu</li></ul>",
                30, true, DateTime.Now.AddDays(7),
                "https://product.hstatic.net/200000722513/product/i9k-t2-special-box-07-1080x1080pixels_6c9ec1001cdf4e4998c13af4ac6c7581_114c47698e4a4984863c3b26e0619b65_grande.png");

            await _specificationRepository.InsertAsync(new CpuSpecification
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
            var gpu1 = await CreateProductAsync(_categories["GPUs"].Id,
                                                _manufacturers["ASUS"].Id,
                                                25000000,
                                                5,
                                                "ProArt GeForce RTX 4070 Ti SUPER 16GB",
                                                "<p>Card đồ họa cao cấp <strong>NVIDIA GeForce RTX 4070 Ti</strong> với kiến trúc Ada Lovelace.</p><ul><li>GPU GeForce RTX 4070 Ti mạnh mẽ</li><li>12GB GDDR6X VRAM</li><li>Xung nhịp boost 2610 MHz</li><li>Ray Tracing và DLSS 3.0</li><li>PCIe 4.0 x16</li><li>Nguồn khuyến nghị 700W</li><li>Chiều dài 310mm</li></ul>",
                                                20,
                                                true,
                                                DateTime.Now,
                                                "https://product.hstatic.net/200000722513/product/fwebp__10__1d22cf39c094494bb772b5bb1c002172_grande.png");

            await _specificationRepository.InsertAsync(new GpuSpecification
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
                "Asus Radeon RX 7900 XT TUF Gaming", "<p>Card đồ họa AMD cao cấp với <strong>20GB VRAM</strong> và hiệu năng gaming xuất sắc.</p><ul><li>GPU AMD Radeon RX 7900 XT</li><li>20GB GDDR6 VRAM</li><li>Xung nhịp boost 2600 MHz</li><li>Kiến trúc RDNA 3</li><li>PCIe 4.0 x16</li><li>Nguồn khuyến nghị 750W</li><li>Tản nhiệt TUF Gaming mạnh mẽ</li></ul>",
                15, true, DateTime.Now.AddDays(5),
                "https://product.hstatic.net/200000722513/product/5681_ea11053c19e375dcaa8138b6f531262d_7d029f536978405393da9fb3c8f1e2fa_4d3cedb8fd4a485db1ece7519c1d41a8_grande.jpg");

            await _specificationRepository.InsertAsync(new GpuSpecification
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
                "Vengeance LPX 16GB", "<p>RAM <strong>DDR5 6000MHz</strong> hiệu năng cao với đèn RGB đẹp mắt.</p><ul><li>Dung lượng 32GB (2x16GB)</li><li>Tốc độ DDR5 6000MHz</li><li>Timing 36-36-36-76</li><li>Đèn RGB tùy chỉnh</li><li>Tản nhiệt nhôm cao cấp</li><li>Hỗ trợ Intel XMP 3.0</li><li>Lý tưởng cho gaming và workstation</li></ul>",
                100, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/gearvn-corsair-vengeance-rgb-ddr-5600-ddr5-5_6e319950a8e14231b28a416076c94951_grande.png");

            await _specificationRepository.InsertAsync(new RamSpecification
            {
                ProductId = ram1.Id,
                RamTypeId = _ramTypes["DDR5"].Id,
                Capacity = 32,
                Speed = 6000,
                ModuleCount = 2,
                Timing = "36-36-36-76",
                Voltage = 1.35f,
                HasRgb = true
            }, autoSave: true);

            var ram2 = await CreateProductAsync(
                _categories["RAMs"].Id, _manufacturers["G.Skill"].Id, 1800000, 5,
                "Trident Z RGB 16GB (2×8)", "<p>RAM <strong>DDR4 3600MHz</strong> với đèn RGB Trident Z nổi tiếng.</p><ul><li>Dung lượng 16GB (2x8GB)</li><li>Tốc độ DDR4 3600MHz</li><li>Timing 18-22-22-42</li><li>Đèn RGB đa màu sắc</li><li>Tản nhiệt nhôm chất lượng</li><li>Hỗ trợ Intel XMP 2.0</li><li>Thiết kế đẹp mắt, hiệu năng tốt</li></ul>",
                80, true, DateTime.Now.AddDays(-3),
                "https://anphat.com.vn/media/product/33685_153665426813.png");

            await _specificationRepository.InsertAsync(new RamSpecification
            {
                ProductId = ram2.Id,
                RamTypeId = _ramTypes["DDR4"].Id,
                Capacity = 16,
                Speed = 3600,
                ModuleCount = 2,
                Timing = "18-22-22-42",
                Voltage = 1.35f,
                HasRgb = true
            }, autoSave: true);
        }

        private async Task SeedStorageAsync()
        {
            var storage1 = await CreateProductAsync(
                _categories["Storages"].Id, _manufacturers["Samsung"].Id, 3000000, 8,
                "970 EVO Plus 1TB", "<p>Ổ cứng SSD <strong>NVMe PCIe 3.0</strong> tốc độ cao từ Samsung.</p><ul><li>Dung lượng 1TB (1000GB)</li><li>Tốc độ đọc 3500 MB/s</li><li>Tốc độ ghi 3300 MB/s</li><li>Giao tiếp PCIe 3.0 x4 NVMe</li><li>Form factor M.2 2280</li><li>Công nghệ V-NAND 3-bit MLC</li><li>Bảo hành 5 năm</li></ul>",
                80, true, DateTime.Now,
                "https://bizweb.dktcdn.net/thumb/grande/100/329/122/products/970-evo-plus-1tb-01-1689929004911.jpg?v=1695052940103");

            await _specificationRepository.InsertAsync(new StorageSpecification
            {
                ProductId = storage1.Id,
                StorageType = StorageType.NvmeSsd,
                Interface = "PCIe 3.0 x4",
                Capacity = 1000,
                ReadSpeed = 3500,
                Rpm = null,
                WriteSpeed = 3300,
                StorageFormFactor = StorageFormFactor.SsdM2_2280
            }, autoSave: true);

            var storage2 = await CreateProductAsync(
                _categories["Storages"].Id, _manufacturers["WD"].Id, 2500000, 5,
                "WD Black SN770 1TB", "<p>SSD NVMe <strong>PCIe 4.0</strong> hiệu năng cao với tốc độ vượt trội.</p><ul><li>Dung lượng 1TB</li><li>Tốc độ đọc 5150 MB/s</li><li>Tốc độ ghi 4900 MB/s</li><li>Giao tiếp PCIe 4.0 x4 NVMe</li><li>Form factor M.2 2280</li><li>Tiết kiệm điện năng</li><li>Bảo hành 5 năm</li></ul>",
                60, true, DateTime.Now.AddDays(2),
                "https://bizweb.dktcdn.net/thumb/grande/100/329/122/products/ssd-wd-black-sn770-pcie-gen4-x4-nvme-m-2-500gb-wds500g3x0e-b058273a-af63-4053-ac31-83b41eb593a2.jpg?v=1655710957737");

            await _specificationRepository.InsertAsync(new StorageSpecification
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
                "GIGABYTE Z790 AORUS XTREME X ICE", "<p>Bo mạch chủ <strong>E-ATX</strong> cao cấp với thiết kế màu trắng sang trọng.</p><ul><li>Socket LGA1700 cho Intel Gen 12-14</li><li>Chipset Z790 cao cấp</li><li>4 khe RAM DDR5, tối đa 192GB</li><li>5 khe M.2 NVMe</li><li>6 cổng SATA</li><li>WiFi 6E tích hợp</li><li>Thiết kế tản nhiệt mạnh mẽ</li><li>RGB Fusion 2.0</li></ul>",
                20, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/z790_aorus_xtreme_x_ice-01_5a397436688c4f2e9dc0e358ebf25927_grande.png");

            await _specificationRepository.InsertAsync(new MotherboardSpecification
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
                "MSI MEG Z890 GODLIKE", "<p>Bo mạch chủ <strong>flagship</strong> với hiệu năng và tính năng <strong>đỉnh cao</strong>.</p><ul><li>Socket LGA1851 cho Intel Gen 15</li><li>Chipset Z890 mới nhất</li><li>4 khe RAM DDR5, tối đa 192GB</li><li>6 khe M.2 NVMe Gen 5</li><li>6 cổng SATA</li><li>WiFi 7 tích hợp</li><li>VRM 24+2+1 phase mạnh mẽ</li><li>Màn hình OLED tích hợp</li></ul>",
                15, true, DateTime.Now,
                "https://product.hstatic.net/200000722513/product/msi-meg_z890_godlike_3d2_rgb_b691f05efbcf45e58c54ab731ea28136_grande.png");

            await _specificationRepository.InsertAsync(new MotherboardSpecification
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
