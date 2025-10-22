using Acme.ProductSelling.Categories;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace Acme.ProductSelling.Products
{
    public interface IProductAppService : ICrudAppService<ProductDto,
                                                            Guid,
                                                            PagedAndSortedResultRequestDto,
                                                            CreateUpdateProductDto>
    {
        Task<PagedResultDto<ProductDto>> GetListByCategoryAsync(GetProductsByCategoryInput input);
        Task<PagedResultDto<ProductDto>> GetListByProductPrice(GetProductsByPrice input);
        Task<PagedResultDto<ProductDto>> GetProductsByName(GetProductByName input);
        Task<ProductDto> GetProductBySlug(string slug);
        Task<PagedResultDto<ProductDto>> GetProductByManufacturer(GetProductsByManufacturer input);
    }
}
