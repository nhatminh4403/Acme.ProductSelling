using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups.Services
{
    public class RamTypeAppService :
        LookupAppServiceBase<RamType, Guid>,
        IRamTypeAppService
    {
        public RamTypeAppService(IRepository<RamType, Guid> repository) : base(repository)
        {
        }

        protected override RamType CreateEntity(ProductLookupDto<Guid> createInput)
        {
            return new RamType { Name = createInput.Name };
        }


        protected override ProductLookupDto<Guid> MapToLookupDto(RamType entity)
        {
            return new ProductLookupDto<Guid>
            {
                Id = entity.Id,
                Name = entity.Name,
            };
        }

        protected override void UpdateEntity(RamType entity, ProductLookupDto<Guid> updateInput)
        {
            entity.Name = updateInput.Name;
        }
    }
}
