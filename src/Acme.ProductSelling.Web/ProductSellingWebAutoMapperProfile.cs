using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products;
using AutoMapper;

namespace Acme.ProductSelling.Web;

public class ProductSellingWebAutoMapperProfile : Profile
{
    public ProductSellingWebAutoMapperProfile()
    {
        CreateMap<ProductDto, CreateUpdateProductDto>();
        CreateMap<CategoryDto, CreateUpdateCategoryDto>();

    }
}
