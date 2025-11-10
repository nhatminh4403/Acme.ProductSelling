using Acme.ProductSelling.Stores.Dtos;
using Acme.ProductSelling.Stores.Services;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Stores
{
    public class StoreAppService :
        CrudAppService<Store, StoreDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateStoreDto>,
        IStoreAppService
    {
        private readonly IStoreRepository _storeRepository;

        public StoreAppService(IRepository<Store, Guid> repository, IStoreRepository storeRepository) : base(repository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<StoreDto> ActivateAsync(Guid id)
        {
            var store = await _storeRepository.GetAsync(id);
            store.Activate();
            await _storeRepository.UpdateAsync(store);
            return ObjectMapper.Map<Store, StoreDto>(store);
        }

        public async Task<StoreDto> DeactivateAsync(Guid id)
        {
            var store = await _storeRepository.GetAsync(id);
            store.Deactivate();
            await _storeRepository.UpdateAsync(store);
            return ObjectMapper.Map<Store, StoreDto>(store);
        }
    }
}
