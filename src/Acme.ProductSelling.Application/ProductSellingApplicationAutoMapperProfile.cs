using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Specifications;
using AutoMapper;
namespace Acme.ProductSelling;
public class ProductSellingApplicationAutoMapperProfile : Profile
{
    public ProductSellingApplicationAutoMapperProfile()
    {
        CreateMap<Manufacturer, ManufacturerDto>();
        CreateMap<CreateUpdateManufacturerDto, Manufacturer>();
        CreateMap<Product, ProductDto>()
               .ForMember(dest => dest.CategoryName,
                          opt => opt.MapFrom(src => src.Category.Name))
               .ForMember(dest => dest.CategorySpecificationType, opt => opt.MapFrom(src => src.Category.SpecificationType))
                .ForMember(dest => dest.ManufacturerName,
                              opt => opt.MapFrom(src => src.Manufacturer.Name))
                // Map các spec con (AutoMapper sẽ tự map nếu tên và kiểu khớp)
                .ForMember(dest => dest.MonitorSpecification, opt => opt.MapFrom(src => src.MonitorSpecification))
                .ForMember(dest => dest.MouseSpecification, opt => opt.MapFrom(src => src.MouseSpecification))
                .ForMember(dest => dest.LaptopSpecification, opt => opt.MapFrom(src => src.LaptopSpecification)) // Giả sử có
                .ForMember(dest => dest.CpuSpecification, opt => opt.MapFrom(src => src.CpuSpecification))
                .ForMember(dest => dest.GpuSpecification, opt => opt.MapFrom(src => src.GpuSpecification))
                .ForMember(dest => dest.RamSpecification, opt => opt.MapFrom(src => src.RamSpecification))
                .ForMember(dest => dest.MotherboardSpecification, opt => opt.MapFrom(src => src.MotherboardSpecification))
                .ForMember(dest => dest.StorageSpecification, opt => opt.MapFrom(src => src.StorageSpecification))
                .ForMember(dest => dest.PsuSpecification, opt => opt.MapFrom(src => src.PsuSpecification))
                .ForMember(dest => dest.CaseSpecification, opt => opt.MapFrom(src => src.CaseSpecification))
                .ForMember(dest => dest.CpuCoolerSpecification, opt => opt.MapFrom(src => src.CpuCoolerSpecification))
                .ForMember(dest => dest.KeyboardSpecification, opt => opt.MapFrom(src => src.KeyboardSpecification))
                .ForMember(dest => dest.HeadsetSpecification, opt => opt.MapFrom(src => src.HeadsetSpecification));
        CreateMap<CreateUpdateProductDto, Product>().ForMember(dest => dest.MonitorSpecificationId, opt => opt.Ignore())
                .ForMember(dest => dest.MonitorSpecification, opt => opt.Ignore())
                .ForMember(dest => dest.MouseSpecificationId, opt => opt.Ignore())
                .ForMember(dest => dest.MouseSpecification, opt => opt.Ignore())
                .ForMember(dest => dest.LaptopSpecificationId, opt => opt.Ignore()) // Giả sử có
                .ForMember(dest => dest.LaptopSpecification, opt => opt.Ignore()) // Giả sử có
                .ForMember(dest => dest.CpuSpecificationId, opt => opt.Ignore())
                .ForMember(dest => dest.CpuSpecification, opt => opt.Ignore())
                .ForMember(dest => dest.GpuSpecificationId, opt => opt.Ignore())
                .ForMember(dest => dest.GpuSpecification, opt => opt.Ignore())
                .ForMember(dest => dest.RamSpecificationId, opt => opt.Ignore())
                .ForMember(dest => dest.RamSpecification, opt => opt.Ignore())
                .ForMember(dest => dest.MotherboardSpecificationId, opt => opt.Ignore())
                .ForMember(dest => dest.MotherboardSpecification, opt => opt.Ignore())
                .ForMember(dest => dest.StorageSpecificationId, opt => opt.Ignore())
                .ForMember(dest => dest.StorageSpecification, opt => opt.Ignore())
                .ForMember(dest => dest.PsuSpecificationId, opt => opt.Ignore())
                .ForMember(dest => dest.PsuSpecification, opt => opt.Ignore())
                .ForMember(dest => dest.CaseSpecificationId, opt => opt.Ignore())
                .ForMember(dest => dest.CaseSpecification, opt => opt.Ignore())
                .ForMember(dest => dest.CpuCoolerSpecificationId, opt => opt.Ignore())
                .ForMember(dest => dest.CpuCoolerSpecification, opt => opt.Ignore())
                .ForMember(dest => dest.KeyboardSpecificationId, opt => opt.Ignore())
                .ForMember(dest => dest.KeyboardSpecification, opt => opt.Ignore())
                .ForMember(dest => dest.HeadsetSpecificationId, opt => opt.Ignore())
                .ForMember(dest => dest.HeadsetSpecification, opt => opt.Ignore());
        // --- Thêm mới Category Mappings ---
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateUpdateCategoryDto, Category>();
        CreateMap<Category, CategoryLookupDto>();
        // --- Thêm mới Spec Mappings ---
        CreateMap<CpuSpecification, CpuSpecificationDto>();
        CreateMap<CreateUpdateCpuSpecificationDto, CpuSpecification>();
        CreateMap<GpuSpecification, GpuSpecificationDto>();
        CreateMap<CreateUpdateGpuSpecificationDto, GpuSpecification>();
        CreateMap<RamSpecification, RamSpecificationDto>();
        CreateMap<CreateUpdateRamSpecificationDto, RamSpecification>();
        CreateMap<MotherboardSpecification, MotherboardSpecificationDto>();
        CreateMap<CreateUpdateMotherboardSpecificationDto, MotherboardSpecification>();
        CreateMap<StorageSpecification, StorageSpecificationDto>();
        CreateMap<CreateUpdateStorageSpecificationDto, StorageSpecification>();
        CreateMap<PsuSpecification, PsuSpecificationDto>();
        CreateMap<CreateUpdatePsuSpecificationDto, PsuSpecification>();
        CreateMap<CaseSpecification, CaseSpecificationDto>();
        CreateMap<CreateUpdateCaseSpecificationDto, CaseSpecification>();
        CreateMap<CpuCoolerSpecification, CpuCoolerSpecificationDto>();
        CreateMap<CreateUpdateCpuCoolerSpecificationDto, CpuCoolerSpecification>();
        CreateMap<KeyboardSpecification, KeyboardSpecificationDto>();
        CreateMap<CreateUpdateKeyboardSpecificationDto, KeyboardSpecification>();
        CreateMap<HeadsetSpecification, HeadsetSpecificationDto>();
        CreateMap<CreateUpdateHeadsetSpecificationDto, HeadsetSpecification>();
        CreateMap<MonitorSpecification, MonitorSpecificationDto>();
        CreateMap<CreateUpdateMonitorSpecificationDto, MonitorSpecification>();
        CreateMap<MouseSpecification, MouseSpecificationDto>();
        CreateMap<CreateUpdateMouseSpecificationDto, MouseSpecification>();
        CreateMap<LaptopSpecification, LaptopSpecificationDto>();
        CreateMap<CreateUpdateLaptopSpecificationDto, LaptopSpecification>();

        CreateMap<Order, OrderDto>();
        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore()) 
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore()); 
        CreateMap<CreateOrderItemDto, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.LineTotalAmount, opt => opt.Ignore());
        CreateMap<OrderItem, OrderItemDto>()
               .ForMember(dest => dest.LineTotalAmount, opt => opt.MapFrom(src => src.LineTotalAmount));
        CreateMap<Cart, CartDto>()
            .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.Items)); // Map collection Items
        CreateMap<CartItem, CartItemDto>();
    }
}
