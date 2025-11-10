using Acme.ProductSelling.Stores.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Stores.Services
{
    public interface IStoreAppService : ICrudAppService<StoreDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateStoreDto>
    {
        Task<StoreDto> ActivateAsync(Guid id);
        Task<StoreDto> DeactivateAsync(Guid id);
    }
}
