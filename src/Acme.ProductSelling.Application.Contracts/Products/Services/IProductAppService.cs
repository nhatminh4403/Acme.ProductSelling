using Acme.ProductSelling.Products.Dtos;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace Acme.ProductSelling.Products.Services
{
    public interface IProductAppService : ICrudAppService<ProductDto,
                                                            Guid,
                                                            PagedAndSortedResultRequestDto,
                                                            CreateUpdateProductDto>
    {
    }
}
