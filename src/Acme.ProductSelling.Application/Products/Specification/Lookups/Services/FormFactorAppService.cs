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
    public class FormFactorAppService : LookupAppServiceBase<FormFactor, Guid>,
                                        IFormFactorAppSerivce
    {
        public FormFactorAppService(IRepository<FormFactor, Guid> repository) : base(repository)
        {
        }

        protected override FormFactor CreateEntity(ProductLookupDto<Guid> dto)
        {
            return new FormFactor { Name = dto.Name };
        }

        protected override ProductLookupDto<Guid> MapToLookupDto(FormFactor entity)
        {
            return new ProductLookupDto<Guid>
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        protected override void UpdateEntity(FormFactor entity, ProductLookupDto<Guid> dto)
        {
            entity.Name = dto.Name;
        }
    }
}
