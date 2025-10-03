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
    public class PanelTypeAppService : CrudAppService<PanelType, PanelTypeDto, Guid, PagedAndSortedResultRequestDto, ProductLookupDto<Guid>>, IPanelTypeAppService
    {
        public PanelTypeAppService(IRepository<PanelType, Guid> repository) : base(repository)
        {
        }

        public async Task<ListResultDto<ProductLookupDto<Guid>>> GetLookupAsync()
        {
            var lookupItems = await Repository.GetListAsync();

            var lookupDtos = lookupItems.Select(x => new ProductLookupDto<Guid>
            {
                Id = x.Id,
                Name = x.Name,
            }).OrderBy(y => y.Name).ToList();
            return new ListResultDto<ProductLookupDto<Guid>>(lookupDtos);
        }
        protected override async Task<PanelType> MapToEntityAsync(ProductLookupDto<Guid> createInput)
        {
            return new PanelType { Name = createInput.Name };
        }
        protected override async Task MapToEntityAsync(ProductLookupDto<Guid> updateInput, PanelType entity)
        {
            entity.Name = updateInput.Name;
        }
    }
}
