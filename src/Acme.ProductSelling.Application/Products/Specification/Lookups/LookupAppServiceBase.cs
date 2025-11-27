using Acme.ProductSelling.Specifications.Lookups.DTOs;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups
{
    public abstract class LookupAppServiceBase<TEntity, TKey> : CrudAppService<
        TEntity,
        ProductLookupDto<TKey>,
        TKey,
        PagedAndSortedResultRequestDto,
        ProductLookupDto<TKey>>
        where TEntity : class, IEntity<TKey>
        where TKey : struct
    {
        protected LookupAppServiceBase(IRepository<TEntity, TKey> repository) : base(repository)
        {
        }

        public virtual async Task<ListResultDto<ProductLookupDto<TKey>>> GetLookupAsync()
        {
            var items = await Repository.GetListAsync();
            var lookupDtos = items
                .Select(MapToLookupDto)
                .OrderBy(dto => dto.Name)
                .ToList();

            return new ListResultDto<ProductLookupDto<TKey>>(lookupDtos);
        }

        protected abstract ProductLookupDto<TKey> MapToLookupDto(TEntity entity);
        protected abstract void UpdateEntity(TEntity entity, ProductLookupDto<TKey> dto);
        protected abstract TEntity CreateEntity(ProductLookupDto<TKey> dto);

        protected override async Task MapToEntityAsync(ProductLookupDto<TKey> updateInput, TEntity entity)
        {
            UpdateEntity(entity, updateInput);
            await Task.CompletedTask;
        }

        protected override async Task<TEntity> MapToEntityAsync(ProductLookupDto<TKey> createInput)
        {
            return await Task.FromResult(CreateEntity(createInput));
        }
    }
}
