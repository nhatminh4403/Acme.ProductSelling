using Acme.ProductSelling.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.ProductSelling.StoreInventories
{
    public class EfCoreStoreInventoryRepository : EfCoreRepository<ProductSellingDbContext, StoreInventory, Guid>, IStoreInventoryRepository
    {
        public EfCoreStoreInventoryRepository(IDbContextProvider<ProductSellingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<StoreInventory> GetByStoreAndProductAsync(Guid storeId, Guid productId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Include(si => si.Product)
                .Include(si => si.Store)
                .FirstOrDefaultAsync(si => si.StoreId == storeId && si.ProductId == productId);
        }

        public async Task<List<StoreInventory>> GetByStoreAsync(Guid storeId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Include(si => si.Product)
                .Where(si => si.StoreId == storeId)
                .OrderBy(si => si.Product.ProductName)
                .ToListAsync();
        }

        public async Task<List<StoreInventory>> GetByProductAsync(Guid productId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Include(si => si.Store)
                .Where(si => si.ProductId == productId)
                .OrderBy(si => si.Store.Name)
                .ToListAsync();
        }

        public async Task<List<StoreInventory>> GetLowStockItemsAsync(Guid storeId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Include(si => si.Product)
                .Include(si => si.Store)
                .Where(si => si.StoreId == storeId && si.Quantity <= si.ReorderLevel)
                .OrderBy(si => si.Quantity)
                .ThenBy(si => si.Product.ProductName)
                .ToListAsync();
        }

        public async Task<bool> HasSufficientStockAsync(Guid storeId, Guid productId, int requiredQuantity)
        {
            var dbSet = await GetDbSetAsync();
            var inventory = await dbSet.FirstOrDefaultAsync(si => si.StoreId == storeId && si.ProductId == productId);

            if (inventory == null)
                return false;

            return inventory.Quantity >= requiredQuantity && inventory.IsAvailableForSale;
        }
        public async Task<List<StoreInventory>> GetLowStockItemsAsync()
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Include(si => si.Product)
                .Include(si => si.Store)
                .Where(si => si.Quantity <= si.ReorderLevel)
                .OrderBy(si => si.Quantity)
                .ThenBy(si => si.Store.Name)
                .ThenBy(si => si.Product.ProductName)
                .ToListAsync();
        }
    }
}
