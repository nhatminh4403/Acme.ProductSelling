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
    public class SwitchTypeAppService : CrudAppService<SwitchType, SwitchTypeDto, Guid, PagedAndSortedResultRequestDto, ProductLookupDto<Guid>>, ISwitchTypeAppService
    {
        public SwitchTypeAppService(IRepository<SwitchType, Guid> repository) : base(repository)
        {
        }

        public async Task<ListResultDto<ProductLookupDto<Guid>>> GetLookupAsync()
        {
            var items = await Repository.GetListAsync();
            var lookupDtos = items.Select(x => new ProductLookupDto<Guid>
            {
                Id = x.Id,
                Name = x.Name,

            }).OrderBy(x => x.Name).ToList();

            return new ListResultDto<ProductLookupDto<Guid>>(lookupDtos);
        }
        protected override async Task<SwitchType> MapToEntityAsync(ProductLookupDto<Guid> createInput)
        {
            return new SwitchType { Name = createInput.Name };
        }
        protected override async Task MapToEntityAsync(ProductLookupDto<Guid> updateInput, SwitchType entity)
        {
            entity.Name = updateInput.Name;
        }

    }
}