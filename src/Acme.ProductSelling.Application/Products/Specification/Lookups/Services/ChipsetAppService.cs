using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Products.Specification.Lookups;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups.Services
{
    public class ChipsetAppService :
        LookupAppServiceBase<Chipset, Guid>,
        IChipsetAppService
    {
        public ChipsetAppService(IRepository<Chipset, Guid> repository) : base(repository)
        {
        }

        protected override ProductLookupDto<Guid> MapToLookupDto(Chipset entity)
        {
            return new ProductLookupDto<Guid>
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        protected override void UpdateEntity(Chipset entity, ProductLookupDto<Guid> dto)
        {
            entity.Name = dto.Name;
        }

        protected override Chipset CreateEntity(ProductLookupDto<Guid> dto)
        {
            return new Chipset { Name = dto.Name };
        }
    }
}
