using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Acme.ProductSelling.Products
{

    public class SpecificationHandler<TEntity, TDto> : ISpecificationHandler
        where TEntity : class, IEntity<Guid>
        where TDto : class
    {
        private readonly IRepository<TEntity, Guid> _repository;
        private readonly IObjectMapper _objectMapper;
        private readonly Func<Product, Guid?> _getSpecId;
        private readonly Action<Product, Guid?> _setSpecId;
        private readonly Func<CreateUpdateProductDto, TDto> _getDto;

        public SpecificationHandler(
            IRepository<TEntity, Guid> repository,
            IObjectMapper objectMapper,
            Func<Product, Guid?> getSpecId,
            Action<Product, Guid?> setSpecId)
        {
            _repository = repository;
            _objectMapper = objectMapper;
            _getSpecId = getSpecId;
            _setSpecId = setSpecId;
        }

        public async Task CreateAsync(Product product, CreateUpdateProductDto dto)
        {
            var specDto = _getDto(dto);
            if (specDto != null)
            {
                var spec = _objectMapper.Map<TDto, TEntity>(specDto);
                spec = await _repository.InsertAsync(spec, autoSave: true);
                _setSpecId(product, spec.Id);
            }
        }

        public async Task UpdateAsync(Product product, CreateUpdateProductDto dto)
        {
            var specDto = _getDto(dto);
            var currentSpecId = _getSpecId(product);

            if (specDto != null)
            {
                if (currentSpecId.HasValue)
                {
                    var existingSpec = await _repository.GetAsync(currentSpecId.Value);
                    _objectMapper.Map(specDto, existingSpec);
                    await _repository.UpdateAsync(existingSpec, autoSave: true);
                }
                else
                {
                    await CreateAsync(product, dto);
                }
            }
            else if (currentSpecId.HasValue)
            {
                await _repository.DeleteAsync(currentSpecId.Value, autoSave: true);
                _setSpecId(product, null);
            }
        }

        public async Task DeleteIfExistsAsync(Product product)
        {
            var specId = _getSpecId(product);
            if (specId.HasValue)
            {
                await _repository.DeleteAsync(specId.Value, autoSave: true);
                _setSpecId(product, null);
            }
        }
    }
}
