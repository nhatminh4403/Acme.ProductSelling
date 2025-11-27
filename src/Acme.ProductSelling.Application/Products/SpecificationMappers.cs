using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Junctions;
using Acme.ProductSelling.Specifications.Models;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Products;


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SpecificationEntityToDtoMapper
{
    // 1. Nested/Flattened Mappings using MapProperty (equivalent to ForMember + MapFrom)

    [MapProperty(nameof(CpuSpecification.Socket.Name), nameof(CpuSpecificationDto.SocketName))]
    public partial CpuSpecificationDto Map(CpuSpecification source);

    public partial GpuSpecificationDto Map(GpuSpecification source);

    [MapProperty(nameof(RamSpecification.RamType.Name), nameof(RamSpecificationDto.RamTypeName))]
    public partial RamSpecificationDto Map(RamSpecification source);

    [MapProperty(nameof(MotherboardSpecification.Socket.Name), nameof(MotherboardSpecificationDto.SocketName))]
    [MapProperty(nameof(MotherboardSpecification.Chipset.Name), nameof(MotherboardSpecificationDto.ChipsetName))]
    [MapProperty(nameof(MotherboardSpecification.FormFactor.Name), nameof(MotherboardSpecificationDto.FormFactorName))]
    [MapProperty(nameof(MotherboardSpecification.SupportedRamTypes.Name), nameof(MotherboardSpecificationDto.SupportedRamTypeName))]
    public partial MotherboardSpecificationDto Map(MotherboardSpecification source);

    public partial StorageSpecificationDto Map(StorageSpecification source);

    [MapProperty(nameof(PsuSpecification.FormFactor.Name), nameof(PsuSpecificationDto.FormFactorName))]
    public partial PsuSpecificationDto Map(PsuSpecification source);

    // Custom Collection mapping for Case Materials
    [MapProperty(nameof(CaseSpecification.FormFactor.Name), nameof(CaseSpecificationDto.SupportedMbFormFactorName))]
    public partial CaseSpecificationDto Map(CaseSpecification source);

    private List<string> MapCaseMaterials(ICollection<CaseMaterial> materials)
        => materials?.Select(m => m.Material?.Name).ToList() ?? new List<string>();

    public partial CpuCoolerSpecificationDto Map(CpuCoolerSpecification source);

    private List<string> MapCoolerSockets(ICollection<CpuCoolerSocketSupport> sockets)
        => sockets?.Select(s => s.Socket?.Name).ToList() ?? new List<string>();

    [MapProperty(nameof(KeyboardSpecification.SwitchType.Name), nameof(KeyboardSpecificationDto.SwitchTypeName))]
    public partial KeyboardSpecificationDto Map(KeyboardSpecification source);

    [MapProperty(nameof(MonitorSpecification.PanelType.Name), nameof(MonitorSpecificationDto.PanelTypeName))]
    public partial MonitorSpecificationDto Map(MonitorSpecification source);

    public partial MouseSpecificationDto Map(MouseSpecification source);
    public partial LaptopSpecificationDto Map(LaptopSpecification source);
    public partial HeadsetSpecificationDto Map(HeadsetSpecification source);

    // New specs (Direct map generally)
    public partial CaseFanSpecificationDto Map(CaseFanSpecification source);
    public partial MemoryCardSpecificationDto Map(MemoryCardSpecification source);
    public partial SpeakerSpecificationDto Map(SpeakerSpecification source);
    public partial MicrophoneSpecificationDto Map(MicrophoneSpecification source);
    public partial WebcamSpecificationDto Map(WebcamSpecification source);
    public partial MousePadSpecificationDto Map(MousePadSpecification source);
    public partial ChairSpecificationDto Map(ChairSpecification source);
    public partial DeskSpecificationDto Map(DeskSpecification source);
    public partial SoftwareSpecificationDto Map(SoftwareSpecification source);
    public partial NetworkHardwareSpecificationDto Map(NetworkHardwareSpecification source);
    public partial HandheldSpecificationDto Map(HandheldSpecification source);
    public partial ConsoleSpecificationDto Map(ConsoleSpecification source);
    public partial HubSpecificationDto Map(HubSpecification source);
    public partial CableSpecificationDto Map(CableSpecification source);
    public partial ChargerSpecificationDto Map(ChargerSpecification source);
    public partial PowerBankSpecificationDto Map(PowerBankSpecification source);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SpecificationDtoToCreateUpdateMapper
{
    public partial CreateUpdateCpuSpecificationDto Map(CpuSpecificationDto source);
    public partial CreateUpdateGpuSpecificationDto Map(GpuSpecificationDto source);
    public partial CreateUpdateRamSpecificationDto Map(RamSpecificationDto source);
    public partial CreateUpdateMotherboardSpecificationDto Map(MotherboardSpecificationDto source);
    public partial CreateUpdateStorageSpecificationDto Map(StorageSpecificationDto source);
    public partial CreateUpdatePsuSpecificationDto Map(PsuSpecificationDto source);

    [MapperIgnoreTarget(nameof(CreateUpdateCaseSpecificationDto.MaterialIds))]
    public partial CreateUpdateCaseSpecificationDto Map(CaseSpecificationDto source);

    [MapperIgnoreTarget(nameof(CreateUpdateCpuCoolerSpecificationDto.SupportedSocketIds))]
    public partial CreateUpdateCpuCoolerSpecificationDto Map(CpuCoolerSpecificationDto source);

    public partial CreateUpdateKeyboardSpecificationDto Map(KeyboardSpecificationDto source);
    public partial CreateUpdateMonitorSpecificationDto Map(MonitorSpecificationDto source);
    public partial CreateUpdateMouseSpecificationDto Map(MouseSpecificationDto source);
    public partial CreateUpdateLaptopSpecificationDto Map(LaptopSpecificationDto source);
    public partial CreateUpdateHeadsetSpecificationDto Map(HeadsetSpecificationDto source);

    // New Specs
    public partial CreateUpdateCaseFanSpecificationDto Map(CaseFanSpecificationDto source);
    public partial CreateUpdateMemoryCardSpecificationDto Map(MemoryCardSpecificationDto source);
    public partial CreateUpdateSpeakerSpecificationDto Map(SpeakerSpecificationDto source);
    public partial CreateUpdateMicrophoneSpecificationDto Map(MicrophoneSpecificationDto source);
    public partial CreateUpdateWebcamSpecificationDto Map(WebcamSpecificationDto source);
    public partial CreateUpdateMousePadSpecificationDto Map(MousePadSpecificationDto source);
    public partial CreateUpdateChairSpecificationDto Map(ChairSpecificationDto source);
    public partial CreateUpdateDeskSpecificationDto Map(DeskSpecificationDto source);
    public partial CreateUpdateSoftwareSpecificationDto Map(SoftwareSpecificationDto source);
    public partial CreateUpdateNetworkHardwareSpecificationDto Map(NetworkHardwareSpecificationDto source);
    public partial CreateUpdateHandheldSpecificationDto Map(HandheldSpecificationDto source);
    public partial CreateUpdateConsoleSpecificationDto Map(ConsoleSpecificationDto source);
    public partial CreateUpdateHubSpecificationDto Map(HubSpecificationDto source);
    public partial CreateUpdateCableSpecificationDto Map(CableSpecificationDto source);
    public partial CreateUpdateChargerSpecificationDto Map(ChargerSpecificationDto source);
    public partial CreateUpdatePowerBankSpecificationDto Map(PowerBankSpecificationDto source);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateDtoToSpecificationEntityMapper
{
    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    [MapperIgnoreTarget(nameof(CpuSpecification.Socket))]
    public partial CpuSpecification Map(CreateUpdateCpuSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial GpuSpecification Map(CreateUpdateGpuSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    [MapperIgnoreTarget(nameof(RamSpecification.RamType))]
    public partial RamSpecification Map(CreateUpdateRamSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.Socket))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.Chipset))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.FormFactor))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.SupportedRamTypes))]
    public partial MotherboardSpecification Map(CreateUpdateMotherboardSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial StorageSpecification Map(CreateUpdateStorageSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    [MapperIgnoreTarget(nameof(PsuSpecification.FormFactor))]
    public partial PsuSpecification Map(CreateUpdatePsuSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    [MapperIgnoreTarget(nameof(CaseSpecification.FormFactor))]
    [MapperIgnoreTarget(nameof(CaseSpecification.Materials))]
    public partial CaseSpecification Map(CreateUpdateCaseSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    [MapperIgnoreTarget(nameof(CpuCoolerSpecification.SupportedSockets))]
    public partial CpuCoolerSpecification Map(CreateUpdateCpuCoolerSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    [MapperIgnoreTarget(nameof(KeyboardSpecification.SwitchType))]
    public partial KeyboardSpecification Map(CreateUpdateKeyboardSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    [MapperIgnoreTarget(nameof(MonitorSpecification.PanelType))]
    public partial MonitorSpecification Map(CreateUpdateMonitorSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial MouseSpecification Map(CreateUpdateMouseSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial LaptopSpecification Map(CreateUpdateLaptopSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial HeadsetSpecification Map(CreateUpdateHeadsetSpecificationDto source);

    // New Specs mappings
    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial CaseFanSpecification Map(CreateUpdateCaseFanSpecificationDto source);

    // ... Repeated for other specifications similarly (omitted for brevity, follow same pattern) ...
    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial MemoryCardSpecification Map(CreateUpdateMemoryCardSpecificationDto source);

    // Continue for Speaker, Microphone, etc... using defaults.
    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial SpeakerSpecification Map(CreateUpdateSpeakerSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial MicrophoneSpecification Map(CreateUpdateMicrophoneSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial WebcamSpecification Map(CreateUpdateWebcamSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial MousePadSpecification Map(CreateUpdateMousePadSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial ChairSpecification Map(CreateUpdateChairSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial DeskSpecification Map(CreateUpdateDeskSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial SoftwareSpecification Map(CreateUpdateSoftwareSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial NetworkHardwareSpecification Map(CreateUpdateNetworkHardwareSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial HandheldSpecification Map(CreateUpdateHandheldSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial ConsoleSpecification Map(CreateUpdateConsoleSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial HubSpecification Map(CreateUpdateHubSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial CableSpecification Map(CreateUpdateCableSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial ChargerSpecification Map(CreateUpdateChargerSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpecificationBase.Id))]
    [MapperIgnoreTarget(nameof(SpecificationBase.ProductId))]
    [MapperIgnoreTarget(nameof(SpecificationBase.Product))]
    public partial PowerBankSpecification Map(CreateUpdatePowerBankSpecificationDto source);
}