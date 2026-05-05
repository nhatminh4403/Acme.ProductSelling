using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Models;
using Riok.Mapperly.Abstractions;
using System.Linq;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Products;

#region Product Mappings (ROBUST FIX)

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class ProductToProductDtoMapper : MapperBase<Product, ProductDto>
{
    private readonly SpecificationBaseToDtoMapper _specMapper;

    public ProductToProductDtoMapper(SpecificationBaseToDtoMapper specMapper)
    {
        _specMapper = specMapper;
    }
    public override ProductDto Map(Product source)
    {
        var dto = MapInternal(source);

        AfterMap(source, dto);

        return dto;
    }

    public override void Map(Product source, ProductDto destination)
    {
        MapInternal(source, destination);
        AfterMap(source, destination);
    }



    [MapProperty(nameof(Product.Category.Name), nameof(ProductDto.CategoryName))]
    [MapProperty(nameof(Product.Manufacturer.Name), nameof(ProductDto.ManufacturerName))]
    [MapProperty(nameof(Product.Category.SpecificationType), nameof(ProductDto.CategorySpecificationType))]

    [MapperIgnoreTarget(nameof(ProductDto.IsAvailableForPurchase))]
    [MapperIgnoreTarget(nameof(ProductDto.TotalStockAcrossAllStores))]
    [MapperIgnoreTarget(nameof(ProductDto.StoreAvailability))]
    [MapperIgnoreTarget(nameof(ProductDto.SpecificationBase))]
    private partial ProductDto MapInternal(Product source);
    [MapProperty(nameof(Product.Category.Name), nameof(ProductDto.CategoryName))]
    [MapProperty(nameof(Product.Manufacturer.Name), nameof(ProductDto.ManufacturerName))]
    [MapProperty(nameof(Product.Category.SpecificationType), nameof(ProductDto.CategorySpecificationType))]
    [MapperIgnoreTarget(nameof(ProductDto.IsAvailableForPurchase))]
    [MapperIgnoreTarget(nameof(ProductDto.TotalStockAcrossAllStores))]
    [MapperIgnoreTarget(nameof(ProductDto.StoreAvailability))]
    [MapperIgnoreTarget(nameof(ProductDto.SpecificationBase))]
    private partial void MapInternal(Product source, ProductDto destination);

    public override void AfterMap(Product source, ProductDto destination)
    {
        destination.IsAvailableForPurchase = source.IsAvailableForPurchase();

        destination.TotalStockAcrossAllStores = source.StoreInventories?.Sum(x => x.Quantity) ?? 0;
        if (source.SpecificationBase != null)
        {
            destination.SpecificationBase = _specMapper.Map(source.SpecificationBase);
        }
        //switch (source.SpecificationBase)
        //{
        //    case MonitorSpecification monitorSpec:
        //        destination.MonitorSpecification = MapMonitor(monitorSpec);
        //        break;

        //    case CpuSpecification cpuSpec:
        //        destination.CpuSpecification = MapCpu(cpuSpec);
        //        break;

        //    case MotherboardSpecification mbSpec:
        //        destination.MotherboardSpecification = MapMb(mbSpec);
        //        break;

        //    case RamSpecification ramSpec:
        //        destination.RamSpecification = MapRam(ramSpec);
        //        break;

        //    case PsuSpecification psuSpec:
        //        destination.PsuSpecification = MapPsu(psuSpec);
        //        break;

        //    case KeyboardSpecification kbSpec:
        //        destination.KeyboardSpecification = MapKb(kbSpec);
        //        break;

        //    case CaseSpecification caseSpec:
        //        destination.CaseSpecification = MapCase(caseSpec);
        //        break;

        //    case CpuCoolerSpecification coolerSpec:
        //        destination.CpuCoolerSpecification = MapCooler(coolerSpec);
        //        break;

        //    case GpuSpecification gpuSpec:
        //        destination.GpuSpecification = MapGpu(gpuSpec);
        //        break;

        //    case StorageSpecification storageSpec:
        //        destination.StorageSpecification = MapStorage(storageSpec);
        //        break;

        //    case MouseSpecification mouseSpec:
        //        destination.MouseSpecification = MapMouse(mouseSpec);
        //        break;

        //    case LaptopSpecification laptopSpec:
        //        destination.LaptopSpecification = MapLaptop(laptopSpec);
        //        break;

        //    case HeadsetSpecification headsetSpec:
        //        destination.HeadsetSpecification = MapHeadset(headsetSpec);
        //        break;

        //    case SpeakerSpecification speakerSpec:
        //        destination.SpeakerSpecification = MapSpeaker(speakerSpec);
        //        break;

        //    case WebcamSpecification webcamSpec:
        //        destination.WebcamSpecification = MapWebcam(webcamSpec);
        //        break;

        //    case CableSpecification cableSpec:
        //        destination.CableSpecification = MapCable(cableSpec);
        //        break;

        //    case SoftwareSpecification softwareSpec:
        //        destination.SoftwareSpecification = MapSoftware(softwareSpec);
        //        break;

        //    case CaseFanSpecification caseFanSpec:
        //        destination.CaseFanSpecification = MapCaseFan(caseFanSpec);
        //        break;

        //    case ChairSpecification chairSpec:
        //        destination.ChairSpecification = MapChair(chairSpec);
        //        break;

        //    case DeskSpecification deskSpec:
        //        destination.DeskSpecification = MapDesk(deskSpec);
        //        break;

        //    case ChargerSpecification chargerSpec:
        //        destination.ChargerSpecification = MapCharger(chargerSpec);
        //        break;

        //    case ConsoleSpecification consoleSpec:
        //        destination.ConsoleSpecification = MapConsole(consoleSpec);
        //        break;

        //    case HandheldSpecification handheldSpec:
        //        destination.HandheldSpecification = MapHandheld(handheldSpec);
        //        break;

        //    case HubSpecification hubSpec:
        //        destination.HubSpecification = MapHub(hubSpec);
        //        break;

        //    case MemoryCardSpecification memoryCardSpec:
        //        destination.MemoryCardSpecification = MapMemoryCard(memoryCardSpec);
        //        break;

        //    case MicrophoneSpecification micSpec:
        //        destination.MicrophoneSpecification = MapMicrophone(micSpec);
        //        break;

        //    case MousePadSpecification mousepadSpec:
        //        destination.MousepadSpecification = MapMousePad(mousepadSpec);
        //        break;

        //    case NetworkHardwareSpecification networkSpec:
        //        destination.NetworkHardwareSpecification = MapNetwork(networkSpec);
        //        break;

        //        break;
        //}
    }


    private MonitorSpecificationDto MapMonitor(MonitorSpecification s) => new MonitorSpecificationDto
    {
        Id = s.Id,
        RefreshRate = s.RefreshRate,
        Resolution = s.Resolution,
        ScreenSize = s.ScreenSize,
        ResponseTime = s.ResponseTime,
        ResponseTimeMs = s.ResponseTimeMs,
        ColorGamut = s.ColorGamut,
        Brightness = s.Brightness,
        VesaMount = s.VesaMount,
        PanelTypeName = (s.PanelType != null) ? s.PanelType.Name : null
    };

    private CpuSpecificationDto MapCpu(CpuSpecification s) => new CpuSpecificationDto
    {
        Id = s.Id,
        CoreCount = s.CoreCount,
        ThreadCount = s.ThreadCount,
        BaseClock = s.BaseClock,
        BoostClock = s.BoostClock,
        L3Cache = s.L3Cache,
        Tdp = s.Tdp,
        HasIntegratedGraphics = s.HasIntegratedGraphics,
        SocketName = (s.Socket != null) ? s.Socket.Name : null
    };

    private MotherboardSpecificationDto MapMb(MotherboardSpecification s) => new MotherboardSpecificationDto
    {
        Id = s.Id,
        RamSlots = s.RamSlots,
        MaxRam = s.MaxRam,
        M2Slots = s.M2Slots,
        SataPorts = s.SataPorts,
        HasWifi = s.HasWifi,
        SocketName = (s.Socket != null) ? s.Socket.Name : null,
        ChipsetName = (s.Chipset != null) ? s.Chipset.Name : null,
        FormFactorName = (s.FormFactor != null) ? s.FormFactor.Name : null,
        SupportedRamTypeName = (s.SupportedRamTypes != null) ? s.SupportedRamTypes.Name : null
    };

    private RamSpecificationDto MapRam(RamSpecification s) => new RamSpecificationDto
    {
        Id = s.Id,
        Capacity = s.Capacity,
        Speed = s.Speed,
        ModuleCount = s.ModuleCount,
        Timing = s.Timing,
        Voltage = s.Voltage,
        HasRgb = s.HasRgb,
        RamFormFactor = s.RamFormFactor,
        RamTypeName = (s.RamType != null) ? s.RamType.Name : null
    };

    private PsuSpecificationDto MapPsu(PsuSpecification s) => new PsuSpecificationDto
    {
        Id = s.Id,
        Wattage = s.Wattage,
        EfficiencyRating = s.EfficiencyRating,
        Modularity = s.Modularity,
        FormFactorName = (s.FormFactor != null) ? s.FormFactor.Name : null
    };

    private KeyboardSpecificationDto MapKb(KeyboardSpecification s) => new KeyboardSpecificationDto
    {
        Id = s.Id,
        KeyboardType = s.KeyboardType,
        Layout = s.Layout,
        Connectivity = s.Connectivity,
        Backlight = s.Backlight,
        SwitchTypeName = (s.SwitchType != null) ? s.SwitchType.Name : null
    };

    private CaseSpecificationDto MapCase(CaseSpecification s) => new CaseSpecificationDto
    {
        Id = s.Id,
        Color = s.Color,
        MaxGpuLength = s.MaxGpuLength,
        MaxCpuCoolerHeight = s.MaxCpuCoolerHeight,
        SupportedMbFormFactorName = (s.FormFactor != null) ? s.FormFactor.Name : null,
        MaterialNames = s.Materials?.Where(m => m != null && m.Material != null).Select(m => m.Material.Name).ToList() ?? new System.Collections.Generic.List<string>()
    };

    private CpuCoolerSpecificationDto MapCooler(CpuCoolerSpecification s) => new CpuCoolerSpecificationDto
    {
        Id = s.Id,
        CoolerType = s.CoolerType,
        FanSize = s.FanSize,
        RadiatorSize = s.RadiatorSize ?? 0,
        Height = s.Height,
        TdpSupport = s.TdpSupport,
        NoiseLevel = s.NoiseLevel,
        Color = s.Color,
        LedLighting = s.LedLighting,
        SupportedSocketNames = s.SupportedSockets?.Where(ss => ss != null && ss.Socket != null).Select(ss => ss.Socket.Name).ToList() ?? new System.Collections.Generic.List<string>()
    };

    private GpuSpecificationDto MapGpu(GpuSpecification s) => new GpuSpecificationDto
    {
        Id = s.Id,
        Chipset = s.Chipset,
        MemorySize = s.MemorySize,
        MemoryType = s.MemoryType,
        BoostClock = s.BoostClock,
        Interface = s.Interface,
        RecommendedPsu = s.RecommendedPsu,
        Length = s.Length
    };

    private StorageSpecificationDto MapStorage(StorageSpecification s) => new StorageSpecificationDto
    {
        Id = s.Id,
        StorageType = s.StorageType,
        Interface = s.Interface,
        Capacity = s.Capacity,
        ReadSpeed = s.ReadSpeed,
        WriteSpeed = s.WriteSpeed,
        StorageFormFactor = s.StorageFormFactor,
        Rpm = s.Rpm
    };

    private MouseSpecificationDto MapMouse(MouseSpecification s) => new MouseSpecificationDto
    {
        Id = s.Id,
        Dpi = s.Dpi,
        ButtonCount = s.ButtonCount,
        PollingRate = s.PollingRate,
        SensorType = s.SensorType,
        Weight = s.Weight,
        Connectivity = s.Connectivity,
        Color = s.Color,
        BacklightColor = s.BacklightColor
    };

    private LaptopSpecificationDto MapLaptop(LaptopSpecification s) => new LaptopSpecificationDto
    {
        Id = s.Id,
        CPU = s.CPU,
        RAM = s.RAM,
        Storage = s.Storage,
        Display = s.Display,
        GraphicsCard = s.GraphicsCard,
        OperatingSystem = s.OperatingSystem,
        BatteryLife = s.BatteryLife,
        Weight = s.Weight,
        Warranty = s.Warranty
    };

    private HeadsetSpecificationDto MapHeadset(HeadsetSpecification s) => new HeadsetSpecificationDto
    {
        Id = s.Id,
        Connectivity = s.Connectivity,
        HasMicrophone = s.HasMicrophone,
        IsSurroundSound = s.IsSurroundSound,
        IsNoiseCancelling = s.IsNoiseCancelling,
        DriverSize = $"{s.DriverSize}"
    };

    private SpeakerSpecificationDto MapSpeaker(SpeakerSpecification s) => new SpeakerSpecificationDto
    {
        Id = s.Id,
        SpeakerType = s.SpeakerType,
        TotalWattage = s.TotalWattage,
        Frequency = s.Frequency,
        Connectivity = s.Connectivity,
        InputPorts = s.InputPorts,
        Color = s.Color
    };

    private WebcamSpecificationDto MapWebcam(WebcamSpecification s) => new WebcamSpecificationDto
    {
        Id = s.Id,
        Resolution = s.Resolution,
        FrameRate = s.FrameRate,
        FocusType = s.FocusType,
        FieldOfView = s.FieldOfView,
        Connectivity = s.Connectivity,
        Connection = s.Connection,
        HasMicrophone = s.HasMicrophone,
        HasPrivacyShutter = s.HasPrivacyShutter,
        MountType = s.MountType,
        Color = s.Color
    };

    private CableSpecificationDto MapCable(CableSpecification s) => new CableSpecificationDto
    {
        Id = s.Id,
        CableType = s.CableType,
        Length = s.Length,
        MaxPower = s.MaxPower,
        DataTransferSpeed = s.DataTransferSpeed,
        Connector1 = s.Connector1,
        Connector2 = s.Connector2,
        IsBraided = s.IsBraided,
        Color = s.Color,
        Warranty = s.Warranty
    };

    private CaseFanSpecificationDto MapCaseFan(CaseFanSpecification s) => new CaseFanSpecificationDto
    {
        Id = s.Id,
        FanSize = s.FanSize,
        MaxRpm = s.MaxRpm,
        NoiseLevel = s.NoiseLevel,
        Airflow = s.Airflow,
        StaticPressure = s.StaticPressure,
        Connector = s.Connector,
        BearingType = s.BearingType,
        HasRgb = s.HasRgb,
        Color = s.Color
    };

    private MicrophoneSpecificationDto MapMicrophone(MicrophoneSpecification s) => new MicrophoneSpecificationDto
    {
        Id = s.Id,
        MicrophoneType = s.MicrophoneType,
        PolarPattern = s.PolarPattern,
        Frequency = s.Frequency,
        SampleRate = s.SampleRate,
        Sensitivity = s.Sensitivity,
        Connectivity = s.Connectivity,
        Connection = s.Connection,
        HasShockMount = s.HasShockMount,
        HasPopFilter = s.HasPopFilter,
        HasRgb = s.HasRgb,
        Color = s.Color
    };

    private MousePadSpecificationDto MapMousePad(MousePadSpecification s) => new MousePadSpecificationDto
    {
        Id = s.Id,
        Width = s.Width,
        Height = s.Height,
        Thickness = s.Thickness,
        Material = s.Material,
        SurfaceType = s.SurfaceType,
        BaseType = s.BaseType,
        HasRgb = s.HasRgb,
        IsWashable = s.IsWashable,
        Color = s.Color
    };
}


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProductDtoToCreateUpdateProductDtoMapper : MapperBase<ProductDto, CreateUpdateProductDto>
{
    [MapperIgnoreTarget(nameof(CreateUpdateProductDto.ProductImageFile))]
    [MapperIgnoreTarget(nameof(CreateUpdateProductDto.ReleaseDate))]
    public override partial CreateUpdateProductDto Map(ProductDto source);

    [MapperIgnoreTarget(nameof(CreateUpdateProductDto.ProductImageFile))]
    [MapperIgnoreTarget(nameof(CreateUpdateProductDto.ReleaseDate))]
    public override partial void Map(ProductDto source, CreateUpdateProductDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateProductDtoToProductMapper : MapperBase<CreateUpdateProductDto, Product>
{
    [MapperIgnoreTarget(nameof(Product.Category))]
    [MapperIgnoreTarget(nameof(Product.Manufacturer))]
    [MapperIgnoreTarget(nameof(Product.IsActive))]
    [MapperIgnoreTarget(nameof(Product.StoreInventories))]
    [MapperIgnoreTarget(nameof(Product.SpecificationBase))]

    public override partial Product Map(CreateUpdateProductDto source);

    [MapperIgnoreTarget(nameof(Product.Category))]
    [MapperIgnoreTarget(nameof(Product.Manufacturer))]
    [MapperIgnoreTarget(nameof(Product.IsActive))]
    [MapperIgnoreTarget(nameof(Product.StoreInventories))]
    [MapperIgnoreTarget(nameof(Product.SpecificationBase))]
    public override partial void Map(CreateUpdateProductDto source, Product destination);
}

#endregion