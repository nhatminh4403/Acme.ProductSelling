using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Data.Categories;
using Acme.ProductSelling.Data.Identity;
using Acme.ProductSelling.Data.Lookups;
using Acme.ProductSelling.Data.Manufacturers;
using Acme.ProductSelling.Data.Products;
using Acme.ProductSelling.Data.Stores;
using Acme.ProductSelling.Products.Services;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
namespace Acme.ProductSelling
{
    public class ProductSellingDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        #region Fields
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        // Specialized seeders
        private readonly CategorySeeder _categorySeeder;
        private readonly ManufacturerSeeder _manufacturerSeeder;
        private readonly LookupDataSeeder _lookupDataSeeder;

        // Product seeders
        private readonly ComputerComponentSeeder _computerComponentSeeder;
        private readonly PeripheralSeeder _peripheralSeeder;
        private readonly CoolingSeeder _coolingSeeder;
        private readonly GamingSeeder _gamingSeeder;
        private readonly AudioVideoSeeder _audioVideoSeeder;
        private readonly FurnitureAndAccessorySeeder _furnitureAndAccessorySeeder;
        private readonly StorageAndNetworkSeeder _storageAndNetworkSeeder;

        // Support seeders

        private readonly StoreSeeder _storeSeeder;
        private readonly IdentityDataSeederContributor _identityDataSeedContributor;
        #endregion

        #region Constructor
        public ProductSellingDataSeederContributor(
             IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            CategorySeeder categorySeeder,
            ManufacturerSeeder manufacturerSeeder,
            LookupDataSeeder lookupDataSeeder,
            ComputerComponentSeeder computerComponentSeeder,
            PeripheralSeeder peripheralSeeder,
            CoolingSeeder coolingSeeder,
            GamingSeeder gamingSeeder,
            AudioVideoSeeder audioVideoSeeder,
            FurnitureAndAccessorySeeder furnitureAndAccessorySeeder,
            StorageAndNetworkSeeder storageAndNetworkSeeder,
            StoreSeeder storeSeeder,
            IdentityDataSeederContributor identityDataSeedContributor)

        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _categorySeeder = categorySeeder;
            _manufacturerSeeder = manufacturerSeeder;
            _lookupDataSeeder = lookupDataSeeder;
            _computerComponentSeeder = computerComponentSeeder;
            _peripheralSeeder = peripheralSeeder;
            _coolingSeeder = coolingSeeder;
            _gamingSeeder = gamingSeeder;
            _audioVideoSeeder = audioVideoSeeder;
            _furnitureAndAccessorySeeder = furnitureAndAccessorySeeder;
            _storageAndNetworkSeeder = storageAndNetworkSeeder;
            _storeSeeder = storeSeeder;
            _identityDataSeedContributor = identityDataSeedContributor;
        }
        #endregion
        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _categoryRepository.GetCountAsync() > 0 || await _productRepository.GetCountAsync() > 0)
            {
                return;
            }

            await _categorySeeder.SeedAsync();

            // 1.2: Manufacturers (required by products)
            await _manufacturerSeeder.SeedAsync();

            // 1.3: Lookup tables (sockets, chipsets, etc. - required by specifications)
            await _lookupDataSeeder.SeedAsync();


            // 2.1: Computer Components (CPUs, GPUs, RAM, Storage, Motherboards)
            _computerComponentSeeder.SetDependencies(
                _categorySeeder.SeededCategories,
                _manufacturerSeeder.SeededManufacturers,
                _lookupDataSeeder.Sockets,
                _lookupDataSeeder.Chipsets,
                _lookupDataSeeder.FormFactors,
                _lookupDataSeeder.RamTypes
            );
            await _computerComponentSeeder.SeedAsync();

            // 2.2: Peripherals (Keyboards, Mice, Monitors, Headsets)
            _peripheralSeeder.SetDependencies(
                _categorySeeder.SeededCategories,
                _manufacturerSeeder.SeededManufacturers,
                _lookupDataSeeder.SwitchTypes,
                _lookupDataSeeder.PanelTypes
            );
            await _peripheralSeeder.SeedAsync();

            // 2.3: Cooling & Power (PSUs, Cases, CPU Coolers, Case Fans)
            _coolingSeeder.SetDependencies(
                _categorySeeder.SeededCategories,
                _manufacturerSeeder.SeededManufacturers,
                _lookupDataSeeder.FormFactors,
                _lookupDataSeeder.Sockets,
                _lookupDataSeeder.Materials
            );
            await _coolingSeeder.SeedAsync();

            // 2.4: Gaming Devices (Laptops, Handhelds, Consoles)
            _gamingSeeder.SetDependencies(
                _categorySeeder.SeededCategories,
                _manufacturerSeeder.SeededManufacturers
            );
            await _gamingSeeder.SeedAsync();

            // 2.5: Audio/Video Equipment (Speakers, Microphones, Webcams)
            _audioVideoSeeder.SetDependencies(
                _categorySeeder.SeededCategories,
                _manufacturerSeeder.SeededManufacturers
            );
            await _audioVideoSeeder.SeedAsync();

            // 2.6: Furniture & Accessories (Chairs, Desks, Cables, Chargers, Power Banks, Hubs)
            _furnitureAndAccessorySeeder.SetDependencies(
                _categorySeeder.SeededCategories,
                _manufacturerSeeder.SeededManufacturers
            );
            await _furnitureAndAccessorySeeder.SeedAsync();

            // 2.7: Storage & Network (Memory Cards, Software, Network Hardware, Mouse Pads)
            _storageAndNetworkSeeder.SetDependencies(
                _categorySeeder.SeededCategories,
                _manufacturerSeeder.SeededManufacturers
            );
            await _storageAndNetworkSeeder.SeedAsync();

            await _storeSeeder.SeedAsync();

            await _identityDataSeedContributor.SeedAsync();
        }


    }
}

