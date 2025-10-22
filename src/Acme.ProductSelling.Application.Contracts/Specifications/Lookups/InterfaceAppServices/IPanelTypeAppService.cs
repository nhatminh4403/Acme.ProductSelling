using Acme.ProductSelling.Products;
using Acme.ProductSelling.Specifications.Lookups.DTOs;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices
{
    public interface IPanelTypeAppService : ICrudAppService<PanelTypeDto, Guid, PagedAndSortedResultRequestDto, ProductLookupDto<Guid>>
    {
        Task<ListResultDto<ProductLookupDto<Guid>>> GetLookupAsync();
    }
}
