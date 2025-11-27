using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Lookups.DTOs;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups.Services
{
    public class PanelTypeAppService :
        LookupAppServiceBase<PanelType, Guid>,
        IPanelTypeAppService
    {
        private readonly PanelTypeToProductLookupDtoMapper _mapper;
        public PanelTypeAppService(IRepository<PanelType, Guid> repository, PanelTypeToProductLookupDtoMapper mapper) : base(repository)
        {
            _mapper = mapper;
        }

        protected override PanelType CreateEntity(ProductLookupDto<Guid> createInput)
        {
            return new PanelType { Name = createInput.Name };
        }


        protected override ProductLookupDto<Guid> MapToLookupDto(PanelType entity)
        {
           return  _mapper.Map(entity);
        }

        protected override void UpdateEntity(PanelType entity, ProductLookupDto<Guid> updateInput)
        {
            entity.Name = updateInput.Name;
        }
    }
}
