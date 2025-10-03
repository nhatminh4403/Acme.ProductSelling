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
    public class ChipsetAppService : CrudAppService<Chipset, ChipsetDto, Guid, PagedAndSortedResultRequestDto, ProductLookupDto<Guid>>, IChipsetAppService
    {
        public ChipsetAppService(IRepository<Chipset, Guid> repository) : base(repository)
        {
        }

        public Task<ListResultDto<ProductLookupDto<Guid>>> GetLookupAsync()
        {
            throw new NotImplementedException();
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
