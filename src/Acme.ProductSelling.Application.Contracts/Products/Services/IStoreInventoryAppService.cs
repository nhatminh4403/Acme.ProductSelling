using Acme.ProductSelling.Products.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Products.Services
{
    public interface IStoreInventoryAppService : IApplicationService
    {
        Task<StoreInventoryDto> GetAsync(Guid id);
        Task<PagedResultDto<StoreInventoryDto>> GetListAsync(GetStoreInventoryListDto input);
        Task<StoreInventoryDto> CreateAsync(CreateUpdateStoreInventoryDto input);
        Task<StoreInventoryDto> UpdateAsync(Guid id, CreateUpdateStoreInventoryDto input);
        Task DeleteAsync(Guid id);
        Task<StoreInventoryDto> AdjustQuantityAsync(Guid id, AdjustStoreInventoryDto input);
        Task TransferInventoryAsync(TransferInventoryDto input);
        Task<StoreInventoryDto> GetByStoreAndProductAsync(Guid storeId, Guid productId);
        Task<int> GetAvailableStockAsync(Guid storeId, Guid productId);
    }
}
