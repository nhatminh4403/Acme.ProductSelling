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
    public class ChipsetAppService : CrudAppService<Chipset, ChipsetDto, Guid, PagedAndSortedResultRequestDto, ProductLookupDto<Guid>>, IChipsetAppService
    {
        public ChipsetAppService(IRepository<Chipset, Guid> repository) : base(repository)
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

        protected override async Task MapToEntityAsync(ProductLookupDto<Guid> updateInput, Chipset entity)
        {
            entity.Name = updateInput.Name;
        }
        protected override async Task<Chipset> MapToEntityAsync(ProductLookupDto<Guid> createInput)
        {
            return new Chipset { Name = createInput.Name };
        }
    }
}
