using Acme.ProductSelling.Data.BaseSeeder;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.StoreInventories;
using Acme.ProductSelling.Stores;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Acme.ProductSelling.Data.Stores
{
    public class StoreSeeder : IDataSeederContributor
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IStoreInventoryRepository _storeInventoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ILogger<StoreSeeder> _logger;

        public List<Store> SeededStores { get; private set; }

        public StoreSeeder(
            IStoreRepository storeRepository,
            IStoreInventoryRepository storeInventoryRepository,
            IProductRepository productRepository,
            IGuidGenerator guidGenerator,
            ILogger<StoreSeeder> logger)
        {
            _storeRepository = storeRepository;
            _storeInventoryRepository = storeInventoryRepository;
            _productRepository = productRepository;
            _guidGenerator = guidGenerator;
            _logger = logger;
            SeededStores = new List<Store>();
        }

        public async Task SeedAsync()
        {
            if (await _storeRepository.GetCountAsync() > 0)
            {
                return; // Already seeded
            }

            // Create stores
            var store1 = await CreateStoreAsync("ST001", "Main Store", "123 Main St",
                "Ho Chi Minh City", "84-28-1234-5678");
            var store2 = await CreateStoreAsync("ST002", "District 1 Branch", "456 Le Loi Blvd",
                "Ho Chi Minh City", "84-28-2234-5678");
            var store3 = await CreateStoreAsync("ST003", "District 7 Branch", "789 Nguyen Van Linh",
                "Ho Chi Minh City", "84-28-3234-5678");

            SeededStores.AddRange(new[] { store1, store2, store3 });

            // Seed inventory for each store
            await SeedStoreInventoriesAsync(store1.Id, store2.Id, store3.Id);
        }

        private async Task<Store> CreateStoreAsync(
            string code,
            string name,
            string address,
            string city,
            string phoneNumber)
        {
            var existingStore = await _storeRepository.FirstOrDefaultAsync(s => s.Code == code);
            if (existingStore != null)
            {
                return existingStore;
            }

            var store = new Store(
                _guidGenerator.Create(),
                name,
                code,
                address,
                city,
                phoneNumber
            );

            store.Email = $"{code.ToLower()}@acme.com";
            store.ManagerName = $"{name} Manager";

            await _storeRepository.InsertAsync(store, autoSave: true);
            return store;
        }

        private async Task SeedStoreInventoriesAsync(Guid store1Id, Guid store2Id, Guid store3Id)
        {
            var existingCount = await _storeInventoryRepository.CountAsync();
            if (existingCount > 0)
            {
                return; // Already seeded
            }

            var allProducts = await _productRepository.GetListAsync();

            _logger.LogInformation("Seeding store inventories for {ProductCount} products across 3 stores...",
                allProducts.Count);

            var inventories = new List<StoreInventory>();

            foreach (var product in allProducts)
            {
                var totalStock = product.StockCount;

                // Distribute stock across stores: 50%, 30%, 20%
                var store1Stock = (int)(totalStock * 0.5);
                var store2Stock = (int)(totalStock * 0.3);
                var store3Stock = totalStock - store1Stock - store2Stock;

                if (store1Stock > 0)
                {
                    inventories.Add(new StoreInventory(
                        _guidGenerator.Create(),
                        store1Id,
                        product.Id,
                        store1Stock,
                        10, // reorder level
                        50  // reorder quantity
                    ));
                }

                if (store2Stock > 0)
                {
                    inventories.Add(new StoreInventory(
                        _guidGenerator.Create(),
                        store2Id,
                        product.Id,
                        store2Stock,
                        10,
                        50
                    ));
                }

                if (store3Stock > 0)
                {
                    inventories.Add(new StoreInventory(
                        _guidGenerator.Create(),
                        store3Id,
                        product.Id,
                        store3Stock,
                        10,
                        50
                    ));
                }
            }

            await _storeInventoryRepository.InsertManyAsync(inventories, autoSave: true);

            _logger.LogInformation("Store inventories seeded successfully!");
        }
    }
}
