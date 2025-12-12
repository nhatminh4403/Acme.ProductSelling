using Acme.ProductSelling.Products.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Products.Services
{
    public interface IProductLookupAppService : IApplicationService
    {
        Task<PagedResultDto<ProductDto>> GetListByCategoryAsync(GetProductsByCategoryInput input);
        Task<PagedResultDto<ProductDto>> GetListByProductPrice(GetProductsByPriceDto input);
        Task<PagedResultDto<ProductDto>> GetProductsByName(GetProductByNameDto input);
        Task<ProductDto> GetProductBySlug(string slug);
        Task<PagedResultDto<ProductDto>> GetProductByManufacturer(GetProductsByManufacturerDto input);

    }
}
