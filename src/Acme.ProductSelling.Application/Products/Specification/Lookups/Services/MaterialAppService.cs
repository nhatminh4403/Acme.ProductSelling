using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Lookups.DTOs;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups.Services
{
    public class MaterialAppService :
        LookupAppServiceBase<Material, Guid>,
        IMaterialAppService
    {
        private readonly MaterialToProductLookupDtoMapper _mapper;
        public MaterialAppService(IRepository<Material, Guid> repository, MaterialToProductLookupDtoMapper mapper) : base(repository)
        {
            _mapper = mapper;
        }

        protected override Material CreateEntity(ProductLookupDto<Guid> createInput)
        {
            return new Material { Name = createInput.Name };
        }

        protected override ProductLookupDto<Guid> MapToLookupDto(Material entity)
        {

            return _mapper.Map(entity);
        }

        protected override void UpdateEntity(Material entity, ProductLookupDto<Guid> updateInput)
        {
            entity.Name = updateInput.Name;
        }
    }
}
