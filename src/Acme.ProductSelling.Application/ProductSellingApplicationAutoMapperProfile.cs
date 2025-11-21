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

using Acme.ProductSelling.Specifications.Lookups.DTOs;

using Acme.ProductSelling.Specifications.Models;
using Acme.ProductSelling.StoreInventories;
using Acme.ProductSelling.StoreInventories.Dtos;
using Acme.ProductSelling.Stores;
using Acme.ProductSelling.Stores.Dtos;
using AutoMapper;
using System;
using System.Linq;

namespace Acme.ProductSelling;

public class ProductSellingApplicationAutoMapperProfile : Profile
{
    public ProductSellingApplicationAutoMapperProfile()
    {
        #region Manufacturer Mappings
        CreateMap<Manufacturer, ManufacturerDto>()
            .ForMember(dest => dest.UrlSlug, opt => opt.MapFrom(src => src.UrlSlug));
        CreateMap<CreateUpdateManufacturerDto, Manufacturer>();
        CreateMap<Manufacturer, ManufacturerLookupDto>();
        CreateMap<Manufacturer, ManufacturerDto>();
        #endregion

        #region Store Mappings
        CreateMap<Store, StoreDto>();
        CreateMap<CreateUpdateStoreDto, Store>();
        #endregion
        // Add to your existing ProductSellingApplicationAutoMapperProfile.cs

        // StoreInventory mappings
        CreateMap<StoreInventory, StoreInventoryDto>()
            .ForMember(dest => dest.StoreName, opt => opt.Ignore())
            .ForMember(dest => dest.ProductName, opt => opt.Ignore())
            .ForMember(dest => dest.ProductImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.NeedsReorder, opt => opt.Ignore());

        CreateMap<CreateUpdateStoreInventoryDto, StoreInventory>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Store, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());


        #region Category Mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.UrlSlug, opt => opt.MapFrom(src => src.UrlSlug));
        CreateMap<CategoryDto, CreateUpdateCategoryDto>()
            .ForMember(dest => dest.UrlSlug, opt => opt.MapFrom(src => src.UrlSlug));
        CreateMap<CreateUpdateCategoryDto, Category>()
            .ForMember(dest => dest.UrlSlug, opt => opt.MapFrom(src => src.UrlSlug));
        CreateMap<Category, CategoryLookupDto>();
        CreateMap<Category, CategoryInGroupDto>()
            .ForMember(dest => dest.Manufacturers, opt => opt.Ignore());
        #endregion

        #region Lookup Mappings
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
        #endregion

        #region Product Mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Manufacturer.Name))
            .ForMember(dest => dest.IsAvailableForPurchase, opt => opt.MapFrom(src => src.StockCount > 0 && (!src.ReleaseDate.HasValue || src.ReleaseDate.Value <= DateTime.Now)))
            .ForMember(dest => dest.StoreAvailability, opt => opt.Ignore());
        // For Edit page - Map ProductDto back to CreateUpdateProductDto
        CreateMap<ProductDto, CreateUpdateProductDto>()
            .ForMember(dest => dest.ProductImageFile, opt => opt.Ignore())
            .ForMember(dest => dest.ReleaseDate, opt => opt.Ignore());
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
            .ForMember(dest => dest.HeadsetSpecification, opt => opt.Ignore())
            // New specs
            .ForMember(dest => dest.CaseFanSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.MemoryCardSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.SpeakerSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.MicrophoneSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.WebcamSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.MousepadSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.ChairSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.DeskSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.SoftwareSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.NetworkHardwareSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.HandheldSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.ConsoleSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.HubSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.CableSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.ChargerSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.PowerBankSpecification, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());
        #endregion

        #region Existing Specifications - Entity to DTO
        CreateMap<CpuSpecification, CpuSpecificationDto>()
            .ForMember(dest => dest.SocketName, opt => opt.MapFrom(src => src.Socket.Name));

        CreateMap<GpuSpecification, GpuSpecificationDto>();

        CreateMap<RamSpecification, RamSpecificationDto>()
            .ForMember(dest => dest.RamTypeName, opt => opt.MapFrom(src => src.RamType.Name));

        CreateMap<MotherboardSpecification, MotherboardSpecificationDto>()
            .ForMember(dest => dest.SocketName, opt => opt.MapFrom(src => src.Socket.Name))
            .ForMember(dest => dest.ChipsetName, opt => opt.MapFrom(src => src.Chipset.Name))
            .ForMember(dest => dest.FormFactorName, opt => opt.MapFrom(src => src.FormFactor.Name))
            .ForMember(dest => dest.SupportedRamTypeName, opt => opt.MapFrom(src => src.SupportedRamTypes.Name));

        CreateMap<StorageSpecification, StorageSpecificationDto>();

        CreateMap<PsuSpecification, PsuSpecificationDto>()
            .ForMember(dest => dest.FormFactorName, opt => opt.MapFrom(src => src.FormFactor.Name));

        CreateMap<CaseSpecification, CaseSpecificationDto>()
            .ForMember(dest => dest.SupportedMbFormFactorName, opt => opt.MapFrom(src => src.FormFactor.Name))
            .ForMember(dest => dest.MaterialNames, opt => opt.MapFrom(src => src.Materials.Select(m => m.Material.Name).ToList()));

        CreateMap<CpuCoolerSpecification, CpuCoolerSpecificationDto>()
            .ForMember(dest => dest.SupportedSocketNames, opt => opt.MapFrom(src => src.SupportedSockets.Select(s => s.Socket.Name).ToList()));

        CreateMap<KeyboardSpecification, KeyboardSpecificationDto>()
            .ForMember(dest => dest.SwitchTypeName, opt => opt.MapFrom(src => src.SwitchType.Name));

        CreateMap<MonitorSpecification, MonitorSpecificationDto>()
            .ForMember(dest => dest.PanelTypeName, opt => opt.MapFrom(src => src.PanelType.Name));

        CreateMap<MouseSpecification, MouseSpecificationDto>();

        CreateMap<LaptopSpecification, LaptopSpecificationDto>();

        CreateMap<HeadsetSpecification, HeadsetSpecificationDto>();
        #endregion

        #region Existing Specifications - DTO to CreateUpdateDTO (for Edit page)
        CreateMap<CpuSpecificationDto, CreateUpdateCpuSpecificationDto>();
        CreateMap<GpuSpecificationDto, CreateUpdateGpuSpecificationDto>();
        CreateMap<RamSpecificationDto, CreateUpdateRamSpecificationDto>();
        CreateMap<MotherboardSpecificationDto, CreateUpdateMotherboardSpecificationDto>();
        CreateMap<StorageSpecificationDto, CreateUpdateStorageSpecificationDto>();
        CreateMap<PsuSpecificationDto, CreateUpdatePsuSpecificationDto>();
        CreateMap<CaseSpecificationDto, CreateUpdateCaseSpecificationDto>()
            .ForMember(dest => dest.MaterialIds, opt => opt.Ignore());
        CreateMap<CpuCoolerSpecificationDto, CreateUpdateCpuCoolerSpecificationDto>()
            .ForMember(dest => dest.SupportedSocketIds, opt => opt.Ignore());
        CreateMap<KeyboardSpecificationDto, CreateUpdateKeyboardSpecificationDto>();
        CreateMap<MonitorSpecificationDto, CreateUpdateMonitorSpecificationDto>();
        CreateMap<MouseSpecificationDto, CreateUpdateMouseSpecificationDto>();
        CreateMap<LaptopSpecificationDto, CreateUpdateLaptopSpecificationDto>();
        CreateMap<HeadsetSpecificationDto, CreateUpdateHeadsetSpecificationDto>();
        #endregion

        #region Existing Specifications - CreateUpdateDTO to Entity
        CreateMap<CreateUpdateCpuSpecificationDto, CpuSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.Socket, opt => opt.Ignore());

        CreateMap<CreateUpdateGpuSpecificationDto, GpuSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateRamSpecificationDto, RamSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.RamType, opt => opt.Ignore());

        CreateMap<CreateUpdateMotherboardSpecificationDto, MotherboardSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.Socket, opt => opt.Ignore())
            .ForMember(dest => dest.Chipset, opt => opt.Ignore())
            .ForMember(dest => dest.FormFactor, opt => opt.Ignore())
            .ForMember(dest => dest.SupportedRamTypes, opt => opt.Ignore());

        CreateMap<CreateUpdateStorageSpecificationDto, StorageSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdatePsuSpecificationDto, PsuSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.FormFactor, opt => opt.Ignore());

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
            .ForMember(dest => dest.SupportedSockets, opt => opt.Ignore());

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

        CreateMap<CreateUpdateMouseSpecificationDto, MouseSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateLaptopSpecificationDto, LaptopSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateHeadsetSpecificationDto, HeadsetSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());
        #endregion

        #region New Specifications - Entity to DTO
        CreateMap<CaseFanSpecification, CaseFanSpecificationDto>();
        CreateMap<MemoryCardSpecification, MemoryCardSpecificationDto>();
        CreateMap<SpeakerSpecification, SpeakerSpecificationDto>();
        CreateMap<MicrophoneSpecification, MicrophoneSpecificationDto>();
        CreateMap<WebcamSpecification, WebcamSpecificationDto>();
        CreateMap<MousePadSpecification, MousePadSpecificationDto>();
        CreateMap<ChairSpecification, ChairSpecificationDto>();
        CreateMap<DeskSpecification, DeskSpecificationDto>();
        CreateMap<SoftwareSpecification, SoftwareSpecificationDto>();
        CreateMap<NetworkHardwareSpecification, NetworkHardwareSpecificationDto>();
        CreateMap<HandheldSpecification, HandheldSpecificationDto>();
        CreateMap<ConsoleSpecification, ConsoleSpecificationDto>();
        CreateMap<HubSpecification, HubSpecificationDto>();
        CreateMap<CableSpecification, CableSpecificationDto>();
        CreateMap<ChargerSpecification, ChargerSpecificationDto>();
        CreateMap<PowerBankSpecification, PowerBankSpecificationDto>();
        #endregion

        #region New Specifications - DTO to CreateUpdateDTO (for Edit page)
        CreateMap<CaseFanSpecificationDto, CreateUpdateCaseFanSpecificationDto>();
        CreateMap<MemoryCardSpecificationDto, CreateUpdateMemoryCardSpecificationDto>();
        CreateMap<SpeakerSpecificationDto, CreateUpdateSpeakerSpecificationDto>();
        CreateMap<MicrophoneSpecificationDto, CreateUpdateMicrophoneSpecificationDto>();
        CreateMap<WebcamSpecificationDto, CreateUpdateWebcamSpecificationDto>();
        CreateMap<MousePadSpecificationDto, CreateUpdateMousePadSpecificationDto>();
        CreateMap<ChairSpecificationDto, CreateUpdateChairSpecificationDto>();
        CreateMap<DeskSpecificationDto, CreateUpdateDeskSpecificationDto>();
        CreateMap<SoftwareSpecificationDto, CreateUpdateSoftwareSpecificationDto>();
        CreateMap<NetworkHardwareSpecificationDto, CreateUpdateNetworkHardwareSpecificationDto>();
        CreateMap<HandheldSpecificationDto, CreateUpdateHandheldSpecificationDto>();
        CreateMap<ConsoleSpecificationDto, CreateUpdateConsoleSpecificationDto>();
        CreateMap<HubSpecificationDto, CreateUpdateHubSpecificationDto>();
        CreateMap<CableSpecificationDto, CreateUpdateCableSpecificationDto>();
        CreateMap<ChargerSpecificationDto, CreateUpdateChargerSpecificationDto>();
        CreateMap<PowerBankSpecificationDto, CreateUpdatePowerBankSpecificationDto>();
        #endregion

        #region New Specifications - CreateUpdateDTO to Entity
        CreateMap<CreateUpdateCaseFanSpecificationDto, CaseFanSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateMemoryCardSpecificationDto, MemoryCardSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateSpeakerSpecificationDto, SpeakerSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateMicrophoneSpecificationDto, MicrophoneSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateWebcamSpecificationDto, WebcamSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateMousePadSpecificationDto, MousePadSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateChairSpecificationDto, ChairSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateDeskSpecificationDto, DeskSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateSoftwareSpecificationDto, SoftwareSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateNetworkHardwareSpecificationDto, NetworkHardwareSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateHandheldSpecificationDto, HandheldSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateConsoleSpecificationDto, ConsoleSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateHubSpecificationDto, HubSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateCableSpecificationDto, CableSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdateChargerSpecificationDto, ChargerSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<CreateUpdatePowerBankSpecificationDto, PowerBankSpecification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());
        #endregion

        #region Order and Cart Mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus))
            .ForMember(dest => dest.OrderStatusText, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus))
            .ForMember(dest => dest.PaymentStatusText, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
            .ForMember(dest => dest.StoreName, opt => opt.Ignore())
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


        CreateMap<OrderHistory, OrderHistoryDto>();
        #endregion

        #region Blog and Comment Mappings
        CreateMap<Blog, BlogDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author));

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
        #endregion
    }
}