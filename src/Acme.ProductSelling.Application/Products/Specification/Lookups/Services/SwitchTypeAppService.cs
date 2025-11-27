using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Lookups.DTOs;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups.Services
{
    public class SwitchTypeAppService :
        LookupAppServiceBase<SwitchType, Guid>,
        ISwitchTypeAppService
    {
        private readonly SwitchTypeToProductLookupDtoMapper _mapper;
        public SwitchTypeAppService(IRepository<SwitchType, Guid> repository, SwitchTypeToProductLookupDtoMapper mapper) : base(repository)
        {
            _mapper = mapper;
        }



        protected override SwitchType CreateEntity(ProductLookupDto<Guid> createInput)
        {
            return new SwitchType { Name = createInput.Name };
        }



        protected override ProductLookupDto<Guid> MapToLookupDto(SwitchType entity)
        {
            return _mapper.Map(entity);
        }

        protected override void UpdateEntity(SwitchType entity, ProductLookupDto<Guid> updateInput)
        {
            entity.Name = updateInput.Name;
        }
    }
}