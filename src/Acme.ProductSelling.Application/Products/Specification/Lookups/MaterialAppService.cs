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
    public class MaterialAppService : CrudAppService<Material, MaterialDto, Guid, PagedAndSortedResultRequestDto, ProductLookupDto<Guid>>,
                                        IMaterialAppService
    {
        public MaterialAppService(IRepository<Material, Guid> repository) : base(repository)
        {
        }

        public async Task<ListResultDto<ProductLookupDto<Guid>>> GetLookupAsync()
        {
            var items = await Repository.GetListAsync();
            return new ListResultDto<ProductLookupDto<Guid>>(
                ObjectMapper.Map<List<Material>, List<ProductLookupDto<Guid>>>(items)
            );
        }
        protected override async Task<Material> MapToEntityAsync(ProductLookupDto<Guid> createInput)
        {
            return new Material { Name = createInput.Name };
        }

        protected override async Task MapToEntityAsync(ProductLookupDto<Guid> updateInput, Material entity)
        {
            entity.Name = updateInput.Name;
        }
    }
}
