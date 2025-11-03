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

namespace Acme.ProductSelling.Products.Specification.Lookups
{
    public class RamTypeAppService : CrudAppService<RamType, RamTypeDto, Guid, PagedAndSortedResultRequestDto, ProductLookupDto<Guid>>, IRamTypeAppService
    {
        public RamTypeAppService(IRepository<RamType, Guid> repository) : base(repository)
        {
        }

        public async Task<ListResultDto<ProductLookupDto<Guid>>> GetLookupAsync()
        {
            var lookupItems = await Repository.GetListAsync();
            var lookupDtos = lookupItems.Select(item => new ProductLookupDto<Guid>
            {
                Id = item.Id,
                Name = item.Name,
            }).OrderBy(dto => dto.Name).ToList();
            return new ListResultDto<ProductLookupDto<Guid>>(lookupDtos);
        }
        protected override async Task<RamType> MapToEntityAsync(ProductLookupDto<Guid> createInput)
        {
            return new RamType { Name = createInput.Name };
        }
        protected override async Task MapToEntityAsync(ProductLookupDto<Guid> updateInput, RamType entity)
        {
            entity.Name = updateInput.Name;
        }
    }
}
