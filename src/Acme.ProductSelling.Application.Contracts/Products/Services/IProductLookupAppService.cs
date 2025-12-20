using Acme.ProductSelling.Products.Dtos;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Products.Services
{
    public interface IProductLookupAppService : IApplicationService
    {
        Task<PagedResultDto<ProductDto>> GetListByCategoryAsync(GetProductsByCategoryInput input);
        Task<PagedResultDto<ProductDto>> GetListByProductPrice(GetProductsByPriceDto input);
        Task<ProductDto> GetProductBySlug(string slug);
        Task<PagedResultDto<ProductDto>> GetProductsByName(GetProductByNameDto input);
        Task<PagedResultDto<ProductDto>> GetProductsByManufacturer(GetProductsByManufacturerDto input);
        Task<PagedResultDto<ProductDto>> GetProductsByNameWithPrice(GetProductByNameWithPriceDto input);
        Task<PagedResultDto<ProductDto>> GetProductsByManufacturerWithPrice(GetProductsByManufacturerWithPriceDto input);


    }
}
