using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Lookups.DTOs;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups.Services
{
    public class ChipsetAppService :
        LookupAppServiceBase<Chipset, Guid>,
        IChipsetAppService
    {
        private readonly ChipsetToProductLookupDtoMapper _mapper;
        public ChipsetAppService(IRepository<Chipset, Guid> repository, ChipsetToProductLookupDtoMapper mapper) : base(repository)
        {
            _mapper = mapper;
        }

        protected override ProductLookupDto<Guid> MapToLookupDto(Chipset entity)
        {
            return _mapper.Map(entity);
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
