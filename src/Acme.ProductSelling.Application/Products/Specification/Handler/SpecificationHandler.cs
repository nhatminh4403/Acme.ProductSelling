using Acme.ProductSelling.Specifications;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Acme.ProductSelling.Products
{

    public class SpecificationHandler<TEntity, TDto> : ISpecificationHandler
        where TEntity : SpecificationBase, new()
        where TDto : class
    {
        private readonly IRepository<TEntity, Guid> _repository;
        private readonly IObjectMapper _objectMapper;
        private readonly Func<CreateUpdateProductDto, TDto> _getDto;

        public SpecificationHandler(
            IRepository<TEntity, Guid> repository,
            IObjectMapper objectMapper,
            Func<CreateUpdateProductDto, TDto> getSpecId)
        {
            _repository = repository;
            _objectMapper = objectMapper;
            _getDto = getSpecId;
        }

        public async Task CreateAsync(Guid productId, CreateUpdateProductDto dto)
        {
            var specDto = _getDto(dto);
            if (specDto != null)
            {
                var spec = _objectMapper.Map<TDto, TEntity>(specDto);
                spec.ProductId = productId;
                spec = await _repository.InsertAsync(spec, autoSave: true);
            }
        }

        public async Task UpdateAsync(Guid productId, CreateUpdateProductDto dto)
        {
            var specDto = _getDto(dto);
            var currentSpecId = await _repository.FirstOrDefaultAsync(s => s.ProductId == productId);

            if (specDto != null)
            {
                if (currentSpecId != null)
                {
                    //var existingSpec = await _repository.GetAsync(currentSpecId.Value);
                    _objectMapper.Map(specDto, currentSpecId);
                    await _repository.UpdateAsync(currentSpecId, autoSave: true);
                }
                else
                {
                    await CreateAsync(productId, dto);
                }
            }
            else if (currentSpecId != null)
            {
                await _repository.DeleteAsync(currentSpecId, autoSave: true);

            }
        }

        public async Task DeleteIfExistsAsync(Guid productId)
        {
            var spec = await _repository.FirstOrDefaultAsync(s => s.ProductId == productId);
            if (spec != null)
            {
                await _repository.DeleteAsync(spec, autoSave: true);
            }
        }
    }
}
