using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
