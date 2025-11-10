using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups.Services
{
    public class SwitchTypeAppService :
        LookupAppServiceBase<SwitchType, Guid>,
        ISwitchTypeAppService
    {
        public SwitchTypeAppService(IRepository<SwitchType, Guid> repository) : base(repository)
        {
        }



        protected override SwitchType CreateEntity(ProductLookupDto<Guid> createInput)
        {
            return new SwitchType { Name = createInput.Name };
        }



        protected override ProductLookupDto<Guid> MapToLookupDto(SwitchType entity)
        {
            return new ProductLookupDto<Guid>
            {
                Name = entity.Name,
                Id = entity.Id,
            };
        }

        protected override void UpdateEntity(SwitchType entity, ProductLookupDto<Guid> updateInput)
        {
            entity.Name = updateInput.Name;
        }
    }
}