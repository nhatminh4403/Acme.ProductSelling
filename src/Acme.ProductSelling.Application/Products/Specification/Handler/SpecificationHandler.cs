using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Specifications;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Handler
{
    public class SpecificationHandler<TEntity, TDto> : ISpecificationHandler
        where TEntity : SpecificationBase, new()
        where TDto : class
    {
        private readonly IRepository<TEntity, Guid> _repository;
        private readonly Func<CreateUpdateProductDto, TDto> _getDto;
        private readonly Func<TDto, TEntity> _mapToEntity;
        private readonly Action<TDto, TEntity> _mapToExistingEntity;

        public SpecificationHandler(
            IRepository<TEntity, Guid> repository,
            Func<CreateUpdateProductDto, TDto> getDtoPart,
            Func<TDto, TEntity> mapToEntity,
            Action<TDto, TEntity> mapToExistingEntity)
        {
            _repository = repository;
            _getDto = getDtoPart;
            _mapToEntity = mapToEntity;
            _mapToExistingEntity = mapToExistingEntity;
        }

        public async Task CreateAsync(Guid productId, CreateUpdateProductDto dto)
        {
            var specDto = _getDto(dto);
            if (specDto != null)
            {
                // Execute delegate using specific mapper defined in Service
                var spec = _mapToEntity(specDto);
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
                    // Execute delegate to map DTO onto existing entity
                    _mapToExistingEntity(specDto, currentSpecId);
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