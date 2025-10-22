using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Lookups.DTOs;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups
{
    public class FormFactorAppService : CrudAppService<FormFactor, FormFactorDto, Guid, PagedAndSortedResultRequestDto, ProductLookupDto<Guid>>,
                                        IFormFactorAppSerivce
    {
        public FormFactorAppService(IRepository<FormFactor, Guid> repository) : base(repository)
        {
        }

        public Task<ListResultDto<ProductLookupDto<Guid>>> GetLookupAsync()
        {
            throw new NotImplementedException();
        }
        protected override async Task<FormFactor> MapToEntityAsync(ProductLookupDto<Guid> createInput)
        {
            return new FormFactor { Name = createInput.Name };
        }
        protected override async Task MapToEntityAsync(ProductLookupDto<Guid> updateInput, FormFactor entity)
        {
            entity.Name = updateInput.Name;
        }
    }
}
