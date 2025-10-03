using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Lookups.DTOs;
using Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Specification.Lookups
{
    public class CpuSocketAppService : CrudAppService<CpuSocket, CpuSocketDto, Guid, PagedAndSortedResultRequestDto, ProductLookupDto<Guid>>,
                                        ICpuSocketAppService
    {
        public CpuSocketAppService(IRepository<CpuSocket, Guid> repository) : base(repository)
        {
        }

        public async Task<ListResultDto<ProductLookupDto<Guid>>> GetLookupAsync()
        {
            var items = await Repository.GetListAsync();
            var lookupDtos = items.Select(item => new ProductLookupDto<Guid>
            {
                Id = item.Id,
                Name = item.Name
            })
                .OrderBy(dto => dto.Name) 
                .ToList();

            return new ListResultDto<ProductLookupDto<Guid>>(lookupDtos);
        }
        protected override async Task<CpuSocket> MapToEntityAsync(ProductLookupDto<Guid> createInput)
        {
            return new CpuSocket
            {
                Name = createInput.Name
            };
        }

         
        protected override async Task MapToEntityAsync(ProductLookupDto<Guid> updateInput, CpuSocket entity)
        {
            entity.Name = updateInput.Name;
        }
    }
}
