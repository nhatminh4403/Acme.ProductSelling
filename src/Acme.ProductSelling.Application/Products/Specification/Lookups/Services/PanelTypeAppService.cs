using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Products.Specification.Lookups;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups.Services
{
    public class PanelTypeAppService :
        LookupAppServiceBase<PanelType, Guid>,
        IPanelTypeAppService
    {
        public PanelTypeAppService(IRepository<PanelType, Guid> repository) : base(repository)
        {
        }

        protected override PanelType CreateEntity(ProductLookupDto<Guid> createInput)
        {
            return new PanelType { Name = createInput.Name };
        }


        protected override ProductLookupDto<Guid> MapToLookupDto(PanelType entity)
        {
            return new ProductLookupDto<Guid>
            {
                Id = entity.Id,
                Name = entity.Name,
            };
        }

        protected override void UpdateEntity(PanelType entity, ProductLookupDto<Guid> updateInput)
        {
            entity.Name = updateInput.Name;
        }
    }
}
