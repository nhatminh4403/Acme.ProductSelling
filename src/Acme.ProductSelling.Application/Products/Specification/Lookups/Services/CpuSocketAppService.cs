using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Products.Specification.Lookups;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups.Services
{
    public class CpuSocketAppService : LookupAppServiceBase<CpuSocket, Guid>,
                                        ICpuSocketAppService
    {
        public CpuSocketAppService(IRepository<CpuSocket, Guid> repository) : base(repository)
        {
        }


        protected override CpuSocket CreateEntity(ProductLookupDto<Guid> dto)
        {
            return new CpuSocket { Name = dto.Name };
        }

        protected override ProductLookupDto<Guid> MapToLookupDto(CpuSocket entity)
        {
            return new ProductLookupDto<Guid>
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        protected override void UpdateEntity(CpuSocket entity, ProductLookupDto<Guid> dto)
        {
            entity.Name = dto.Name;
        }

    }
}
