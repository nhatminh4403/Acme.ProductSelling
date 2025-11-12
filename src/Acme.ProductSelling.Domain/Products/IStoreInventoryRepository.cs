using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products
{
    public interface IStoreInventoryRepository : IRepository<StoreInventory, Guid>
    {
        Task<StoreInventory> GetByStoreAndProductAsync(Guid storeId, Guid productId);
        Task<List<StoreInventory>> GetByStoreAsync(Guid storeId);
        Task<List<StoreInventory>> GetByProductAsync(Guid productId);
        Task<List<StoreInventory>> GetLowStockItemsAsync(Guid storeId);
        Task<bool> HasSufficientStockAsync(Guid storeId, Guid productId, int requiredQuantity);
    }
}
