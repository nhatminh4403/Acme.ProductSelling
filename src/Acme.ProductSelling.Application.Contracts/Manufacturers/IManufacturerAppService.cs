using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace Acme.ProductSelling.Manufacturers
{
    public interface IManufacturerAppService : ICrudAppService<
        ManufacturerDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateManufacturerDto>
    {
        Task<ListResultDto<ManufacturerLookupDto>> GetManufacturerLookupAsync();
        Task<List<ManufacturerLookupDto>> GetManufacturersByKeywordAsync(string keyword);
    }
}
