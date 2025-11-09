using Acme.ProductSelling.Products.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Specifications.Lookups.InterfaceAppServices
{
    public interface ILookupAppService<TKey> : ICrudAppService<
        ProductLookupDto<TKey>,
        TKey,
        PagedAndSortedResultRequestDto,
        ProductLookupDto<TKey>>
        where TKey : struct
    {
        Task<ListResultDto<ProductLookupDto<TKey>>> GetLookupAsync();
    }
}
