using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Lookups.DTOs;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups.Services
{
    public class CpuSocketAppService : LookupAppServiceBase<CpuSocket, Guid>,
                                        ICpuSocketAppService
    {
        private readonly CpuSocketToProductLookupDtoMapper _mapper;
        public CpuSocketAppService(IRepository<CpuSocket, Guid> repository, CpuSocketToProductLookupDtoMapper mapper) : base(repository)
        {
            _mapper = mapper;
        }


        protected override CpuSocket CreateEntity(ProductLookupDto<Guid> dto)
        {
            return new CpuSocket { Name = dto.Name };
        }

        protected override ProductLookupDto<Guid> MapToLookupDto(CpuSocket entity)
        {
            return _mapper.Map(entity);
        }

        protected override void UpdateEntity(CpuSocket entity, ProductLookupDto<Guid> dto)
        {
            entity.Name = dto.Name;
        }

    }
}
