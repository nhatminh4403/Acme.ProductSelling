using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products;
using AutoMapper;

namespace Acme.ProductSelling;

public class ProductSellingApplicationAutoMapperProfile : Profile
{
    public ProductSellingApplicationAutoMapperProfile()
    {
        CreateMap<Product, ProductDto>()
               .ForMember(dest => dest.CategoryName, 
                          opt => opt.MapFrom(src => src.Category.Name)); 
        CreateMap<CreateUpdateProductDto, Product>(); 

        // --- Thêm mới Category Mappings ---
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateUpdateCategoryDto, Category>();
        CreateMap<Category, CategoryLookupDto>();
    }
}
