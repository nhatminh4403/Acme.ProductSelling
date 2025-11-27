using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Lookups.DTOs;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups.Services
{
    public class FormFactorAppService : LookupAppServiceBase<FormFactor, Guid>,
                                        IFormFactorAppSerivce
    {
        private readonly FormFactorToProductLookupDtoMapper _mapper;
        public FormFactorAppService(IRepository<FormFactor, Guid> repository, FormFactorToProductLookupDtoMapper mapper) : base(repository)
        {
            _mapper = mapper;
        }

        protected override FormFactor CreateEntity(ProductLookupDto<Guid> dto)
        {
            return new FormFactor { Name = dto.Name };
        }

        protected override ProductLookupDto<Guid> MapToLookupDto(FormFactor entity)
        {
            return _mapper.Map(entity);
        }

        protected override void UpdateEntity(FormFactor entity, ProductLookupDto<Guid> dto)
        {
            entity.Name = dto.Name;
        }
    }
}
