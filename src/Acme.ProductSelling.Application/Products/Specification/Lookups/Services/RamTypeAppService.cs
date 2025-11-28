using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Lookups.DTOs;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups.Services
{
    public class RamTypeAppService :
        LookupAppServiceBase<RamType, Guid>,
        IRamTypeAppService
    {
        private readonly RamTypeToProductLookupDtoMapper _mapper;
        public RamTypeAppService(IRepository<RamType, Guid> repository, RamTypeToProductLookupDtoMapper mapper) : base(repository)
        {
            _mapper = mapper;
        }

        protected override RamType CreateEntity(ProductLookupDto<Guid> createInput)
        {
            return new RamType { Name = createInput.Name };
        }


        protected override ProductLookupDto<Guid> MapToLookupDto(RamType entity)
        {
            return _mapper.Map(entity);
        }

        protected override void UpdateEntity(RamType entity, ProductLookupDto<Guid> updateInput)
        {
            entity.Name = updateInput.Name;
        }
    }
}
