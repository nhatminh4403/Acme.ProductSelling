using Acme.ProductSelling.Blogs;
using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Comments;
using Acme.ProductSelling.Manufacturers;
using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Cable;
using Acme.ProductSelling.Specifications.CaseFan;
using Acme.ProductSelling.Specifications.Chair;
using Acme.ProductSelling.Specifications.Charger;
using Acme.ProductSelling.Specifications.Console;
using Acme.ProductSelling.Specifications.Desk;
using Acme.ProductSelling.Specifications.Handheld;
using Acme.ProductSelling.Specifications.Hub;
using Acme.ProductSelling.Specifications.Lookups.DTOs;
using Acme.ProductSelling.Specifications.MemoryCard;
using Acme.ProductSelling.Specifications.Microphone;
using Acme.ProductSelling.Specifications.Models;
using Acme.ProductSelling.Specifications.MousePad;
using Acme.ProductSelling.Specifications.NetworkHardware;
using Acme.ProductSelling.Specifications.PowerBank;
using Acme.ProductSelling.Specifications.Software;
using Acme.ProductSelling.Specifications.Speaker;
using Acme.ProductSelling.Specifications.Webcam;
using AutoMapper;
using System;
using System.Linq;

namespace Acme.ProductSelling;
public class ProductSellingApplicationAutoMapperProfile : Profile
{
    public ProductSellingApplicationAutoMapperProfile()
    {
        //manufacture
        CreateMap<Manufacturer, ManufacturerDto>().ForMember(dest => dest.UrlSlug, opt => opt.MapFrom(src => src.UrlSlug));
        CreateMap<CreateUpdateManufacturerDto, Manufacturer>();
        CreateMap<Manufacturer, ManufacturerLookupDto>();

        // Category Mappings ---
        CreateMap<Category, CategoryDto>().ForMember(dest => dest.UrlSlug, opt => opt.MapFrom(src => src.UrlSlug));
        CreateMap<CategoryDto, CreateUpdateCategoryDto>().ForMember(dest => dest.UrlSlug, opt => opt.MapFrom(src => src.UrlSlug));
        CreateMap<CreateUpdateCategoryDto, Category>().ForMember(dest => dest.UrlSlug, opt => opt.MapFrom(src => src.UrlSlug));
        CreateMap<Category, CategoryLookupDto>();
        CreateMap<Category, CategoryInGroupDto>().ForMember(dest => dest.Manufacturers, opt => opt.Ignore());

        //lookup

        CreateMap<CpuSocket, CpuSocketDto>();
        CreateMap<Material, MaterialDto>();
        CreateMap<Chipset, ChipsetDto>();
        CreateMap<FormFactor, FormFactorDto>();
        CreateMap<PanelType, PanelTypeDto>();
        CreateMap<RamType, RamTypeDto>();
        CreateMap<SwitchType, SwitchTypeDto>();

        CreateMap<CpuSocket, ProductLookupDto<Guid>>();
        CreateMap<Chipset, ProductLookupDto<Guid>>();
        CreateMap<FormFactor, ProductLookupDto<Guid>>();
        CreateMap<Material, ProductLookupDto<Guid>>();
        CreateMap<PanelType, ProductLookupDto<Guid>>();
        CreateMap<RamType, ProductLookupDto<Guid>>();
        CreateMap<SwitchType, ProductLookupDto<Guid>>();
        //product
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Manufacturer.Name));
        CreateMap<ProductDto, CreateUpdateProductDto>();

        CreateMap<CpuSpecificationDto, CreateUpdateCpuSpecificationDto>();
        CreateMap<MotherboardSpecificationDto, CreateUpdateMotherboardSpecificationDto>();
        CreateMap<GpuSpecificationDto, CreateUpdateGpuSpecificationDto>();
        CreateMap<HeadsetSpecificationDto, CreateUpdateHeadsetSpecificationDto>();
        CreateMap<KeyboardSpecificationDto, CreateUpdateKeyboardSpecificationDto>();
        CreateMap<LaptopSpecificationDto, CreateUpdateLaptopSpecificationDto>();
        CreateMap<MonitorSpecificationDto, CreateUpdateMonitorSpecificationDto>();
        CreateMap<MouseSpecificationDto, CreateUpdateMouseSpecificationDto>();
        CreateMap<PsuSpecificationDto, CreateUpdatePsuSpecificationDto>();
        CreateMap<RamSpecificationDto, CreateUpdateRamSpecificationDto>();
        CreateMap<StorageSpecificationDto, CreateUpdateStorageSpecificationDto>();
        CreateMap<CaseSpecificationDto, CreateUpdateCaseSpecificationDto>()
            .ForMember(dest => dest.MaterialIds, opt => opt.Ignore());

        CreateMap<CpuCoolerSpecificationDto, CreateUpdateCpuCoolerSpecificationDto>()
            .ForMember(dest => dest.SupportedSocketIds, opt => opt.Ignore());

        CreateMap<CreateUpdateProductDto, Product>()
            .ForMember(dest => dest.Category, opt => opt.Ignore())
            .ForMember(dest => dest.Manufacturer, opt => opt.Ignore())
            .ForMember(dest => dest.MonitorSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.MouseSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.LaptopSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.CpuSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.GpuSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.RamSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.MotherboardSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.StorageSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.PsuSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.CaseSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.CpuCoolerSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.KeyboardSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.HeadsetSpecification, opt => opt.Ignore());
        CreateMap<CaseFanSpecification, CaseFanSpecificationDto>();
        CreateMap<CreateUpdateCaseFanSpecificationDto, CaseFanSpecification>();

        // MemoryCard
        CreateMap<MemoryCardSpecification, MemoryCardSpecificationDto>();
        CreateMap<CreateUpdateMemoryCardSpecificationDto, MemoryCardSpecification>();

        // Speaker
        CreateMap<SpeakerSpecification, SpeakerSpecificationDto>();
        CreateMap<CreateUpdateSpeakerSpecificationDto, SpeakerSpecification>();

        // Microphone
        CreateMap<MicrophoneSpecification, MicrophoneSpecificationDto>();
        CreateMap<CreateUpdateMicrophoneSpecificationDto, MicrophoneSpecification>();

        // Webcam
        CreateMap<WebcamSpecification, WebcamSpecificationDto>();
        CreateMap<CreateUpdateWebcamSpecificationDto, WebcamSpecification>();

        // MousePad
        CreateMap<MousePadSpecification, MousePadSpecificationDto>();
        CreateMap<CreateUpdateMousePadSpecificationDto, MousePadSpecification>();

        // Chair
        CreateMap<ChairSpecification, ChairSpecificationDto>();
        CreateMap<CreateUpdateChairSpecificationDto, ChairSpecification>();

        // Desk
        CreateMap<DeskSpecification, DeskSpecificationDto>();
        CreateMap<CreateUpdateDeskSpecificationDto, DeskSpecification>();

        // Software
        CreateMap<SoftwareSpecification, SoftwareSpecificationDto>();
        CreateMap<CreateUpdateSoftwareSpecificationDto, SoftwareSpecification>();

        // NetworkHardware
        CreateMap<NetworkHardwareSpecification, NetworkHardwareSpecificationDto>();
        CreateMap<CreateUpdateNetworkHardwareSpecificationDto, NetworkHardwareSpecification>();

        // Handheld
        CreateMap<HandheldSpecification, HandheldSpecificationDto>();
        CreateMap<CreateUpdateHandheldSpecificationDto, HandheldSpecification>();

        // Console
        CreateMap<ConsoleSpecification, ConsoleSpecificationDto>();
        CreateMap<CreateUpdateConsoleSpecificationDto, ConsoleSpecification>();

        // Hub
        CreateMap<HubSpecification, HubSpecificationDto>();
        CreateMap<CreateUpdateHubSpecificationDto, HubSpecification>();

        // Cable
        CreateMap<CableSpecification, CableSpecificationDto>();
        CreateMap<CreateUpdateCableSpecificationDto, CableSpecification>();

        // Charger
        CreateMap<ChargerSpecification, ChargerSpecificationDto>();
        CreateMap<CreateUpdateChargerSpecificationDto, ChargerSpecification>();

        // PowerBank
        CreateMap<PowerBankSpecification, PowerBankSpecificationDto>();
        CreateMap<CreateUpdatePowerBankSpecificationDto, PowerBankSpecification>();
    

    // Spec Mappings ---

    CreateMap<CpuSpecification, CpuSpecificationDto>()
            .ForMember(dest => dest.SocketName, opt => opt.MapFrom(src => src.Socket.Name));

        CreateMap<MotherboardSpecification, MotherboardSpecificationDto>()
            .ForMember(dest => dest.SocketName, opt => opt.MapFrom(src => src.Socket.Name))
            .ForMember(dest => dest.ChipsetName, opt => opt.MapFrom(src => src.Chipset.Name))
            .ForMember(dest => dest.FormFactorName, opt => opt.MapFrom(src => src.FormFactor.Name))
            .ForMember(dest => dest.SupportedRamTypeName, opt => opt.MapFrom(src => src.SupportedRamTypes.Name));

        CreateMap<CaseSpecification, CaseSpecificationDto>()
            .ForMember(dest => dest.SupportedMbFormFactorName, opt => opt.MapFrom(src => src.FormFactor.Name))
            .ForMember(dest => dest.MaterialNames, opt => opt.MapFrom(src => src.Materials.Select(m => m.Material.Name).ToList()));

        CreateMap<CpuCoolerSpecification, CpuCoolerSpecificationDto>()
            .ForMember(dest => dest.SupportedSocketNames, opt => opt.MapFrom(src => src.SupportedSockets.Select(s => s.Socket.Name).ToList()));

        CreateMap<KeyboardSpecification, KeyboardSpecificationDto>()
            .ForMember(dest => dest.SwitchTypeName, opt => opt.MapFrom(src => src.SwitchType.Name));

        CreateMap<MonitorSpecification, MonitorSpecificationDto>()
            .ForMember(dest => dest.PanelTypeName, opt => opt.MapFrom(src => src.PanelType.Name));

        CreateMap<PsuSpecification, PsuSpecificationDto>()
            .ForMember(dest => dest.FormFactorName, opt => opt.MapFrom(src => src.FormFactor.Name));

        CreateMap<RamSpecification, RamSpecificationDto>()
            .ForMember(dest => dest.RamTypeName, opt => opt.MapFrom(src => src.RamType.Name));

        CreateMap<GpuSpecification, GpuSpecificationDto>();
        CreateMap<HeadsetSpecification, HeadsetSpecificationDto>();
        CreateMap<LaptopSpecification, LaptopSpecificationDto>();
        CreateMap<MouseSpecification, MouseSpecificationDto>();
        CreateMap<StorageSpecification, StorageSpecificationDto>();

        // SPECIFICATIONS (CREATEUPDATEDTO -> ENTITY) ---

        CreateMap<CreateUpdateCpuSpecificationDto, CpuSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.Socket, opt => opt.Ignore());

        CreateMap<CreateUpdateMotherboardSpecificationDto, MotherboardSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.Socket, opt => opt.Ignore())
            .ForMember(dest => dest.Chipset, opt => opt.Ignore())
            .ForMember(dest => dest.FormFactor, opt => opt.Ignore())
            .ForMember(dest => dest.SupportedRamTypes, opt => opt.Ignore());

        CreateMap<CreateUpdateCaseSpecificationDto, CaseSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.FormFactor, opt => opt.Ignore())
            .ForMember(dest => dest.Materials, opt => opt.Ignore());

        CreateMap<CreateUpdateCpuCoolerSpecificationDto, CpuCoolerSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.SupportedSockets, opt => opt.Ignore()); // MANY-TO-MANY

        CreateMap<CreateUpdateKeyboardSpecificationDto, KeyboardSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.SwitchType, opt => opt.Ignore());

        CreateMap<CreateUpdateMonitorSpecificationDto, MonitorSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.PanelType, opt => opt.Ignore());

        CreateMap<CreateUpdatePsuSpecificationDto, PsuSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.FormFactor, opt => opt.Ignore());

        CreateMap<CreateUpdateRamSpecificationDto, RamSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.RamType, opt => opt.Ignore());


        //order - cart
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus))
            .ForMember(dest => dest.OrderStatusText, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus))
            .ForMember(dest => dest.PaymentStatusText, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
             .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod));
        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod));

        CreateMap<CreateOrderItemDto, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.LineTotalAmount, opt => opt.Ignore());
        CreateMap<OrderItem, OrderItemDto>()
               .ForMember(dest => dest.LineTotalAmount, opt => opt.MapFrom(src => src.LineTotalAmount));
        CreateMap<Cart, CartDto>()
            .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.Items));
        CreateMap<CartItem, CartItemDto>();


        //blog - comment
        CreateMap<Blog, BlogDto>().ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));
        CreateMap<CreateAndUpdateBlogDto, Blog>()
            .ConvertUsing((src, dest, context) =>
            {
                var blog = new Blog(
                    context.Mapper.Map<Guid>(Guid.NewGuid()),
                    src.Title,
                    src.Content,
                    src.PublishedDate,
                    src.Author,
                    src.AuthorId,
                    src.UrlSlug,
                    src.MainImageUrl,
                    src.MainImageId
                );
                return blog;
            });

        CreateMap<Comment, CommentDto>();


    }
}
