using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Lookups.DTOs;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
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
