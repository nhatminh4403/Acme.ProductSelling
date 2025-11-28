using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Models;
using Riok.Mapperly.Abstractions;
using System;
using System.Linq;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Products;


#region Existing Specifications - Entity to DTO

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CpuSpecificationToDtoMapper : MapperBase<CpuSpecification, CpuSpecificationDto>
{
    [MapProperty(nameof(CpuSpecification.Socket.Name), nameof(CpuSpecificationDto.SocketName))]
    public override partial CpuSpecificationDto Map(CpuSpecification source);
    public override partial void Map(CpuSpecification source, CpuSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class GpuSpecificationToDtoMapper : MapperBase<GpuSpecification, GpuSpecificationDto>
{
    public override partial GpuSpecificationDto Map(GpuSpecification source);
    public override partial void Map(GpuSpecification source, GpuSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RamSpecificationToDtoMapper : MapperBase<RamSpecification, RamSpecificationDto>
{
    [MapProperty(nameof(RamSpecification.RamType.Name), nameof(RamSpecificationDto.RamTypeName))]
    public override partial RamSpecificationDto Map(RamSpecification source);
    public override partial void Map(RamSpecification source, RamSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MotherboardSpecificationToDtoMapper : MapperBase<MotherboardSpecification, MotherboardSpecificationDto>
{
    [MapProperty(nameof(MotherboardSpecification.Socket.Name), nameof(MotherboardSpecificationDto.SocketName))]
    [MapProperty(nameof(MotherboardSpecification.Chipset.Name), nameof(MotherboardSpecificationDto.ChipsetName))]
    [MapProperty(nameof(MotherboardSpecification.FormFactor.Name), nameof(MotherboardSpecificationDto.FormFactorName))]
    [MapProperty(nameof(MotherboardSpecification.SupportedRamTypes.Name), nameof(MotherboardSpecificationDto.SupportedRamTypeName))]
    public override partial MotherboardSpecificationDto Map(MotherboardSpecification source);
    public override partial void Map(MotherboardSpecification source, MotherboardSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class StorageSpecificationToDtoMapper : MapperBase<StorageSpecification, StorageSpecificationDto>
{
    public override partial StorageSpecificationDto Map(StorageSpecification source);
    public override partial void Map(StorageSpecification source, StorageSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PsuSpecificationToDtoMapper : MapperBase<PsuSpecification, PsuSpecificationDto>
{
    [MapProperty(nameof(PsuSpecification.FormFactor.Name), nameof(PsuSpecificationDto.FormFactorName))]
    public override partial PsuSpecificationDto Map(PsuSpecification source);
    public override partial void Map(PsuSpecification source, PsuSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CaseSpecificationToDtoMapper : MapperBase<CaseSpecification, CaseSpecificationDto>
{
    [MapProperty(nameof(CaseSpecification.FormFactor.Name), nameof(CaseSpecificationDto.SupportedMbFormFactorName))]
    [MapperIgnoreTarget(nameof(CaseSpecificationDto.MaterialNames))] // Calculated in AfterMap
    public override partial CaseSpecificationDto Map(CaseSpecification source);

    [MapperIgnoreTarget(nameof(CaseSpecificationDto.MaterialNames))]
    public override partial void Map(CaseSpecification source, CaseSpecificationDto destination);

    public override void AfterMap(CaseSpecification source, CaseSpecificationDto destination)
    {
        destination.MaterialNames = source.Materials?.Select(m => m.Material?.Name).Where(x => x != null).ToList();
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CpuCoolerSpecificationToDtoMapper : MapperBase<CpuCoolerSpecification, CpuCoolerSpecificationDto>
{
    [MapperIgnoreTarget(nameof(CpuCoolerSpecificationDto.SupportedSocketNames))] // Calculated in AfterMap
    public override partial CpuCoolerSpecificationDto Map(CpuCoolerSpecification source);

    [MapperIgnoreTarget(nameof(CpuCoolerSpecificationDto.SupportedSocketNames))]
    public override partial void Map(CpuCoolerSpecification source, CpuCoolerSpecificationDto destination);

    public override void AfterMap(CpuCoolerSpecification source, CpuCoolerSpecificationDto destination)
    {
        destination.SupportedSocketNames = source.SupportedSockets?.Select(s => s.Socket?.Name).Where(x => x != null).ToList();
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class KeyboardSpecificationToDtoMapper : MapperBase<KeyboardSpecification, KeyboardSpecificationDto>
{
    [MapProperty(nameof(KeyboardSpecification.SwitchType.Name), nameof(KeyboardSpecificationDto.SwitchTypeName))]
    public override partial KeyboardSpecificationDto Map(KeyboardSpecification source);
    public override partial void Map(KeyboardSpecification source, KeyboardSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MonitorSpecificationToDtoMapper : MapperBase<MonitorSpecification, MonitorSpecificationDto>
{
    [MapProperty(nameof(MonitorSpecification.PanelType.Name), nameof(MonitorSpecificationDto.PanelTypeName))]
    public override partial MonitorSpecificationDto Map(MonitorSpecification source);
    public override partial void Map(MonitorSpecification source, MonitorSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MouseSpecificationToDtoMapper : MapperBase<MouseSpecification, MouseSpecificationDto>
{
    public override partial MouseSpecificationDto Map(MouseSpecification source);
    public override partial void Map(MouseSpecification source, MouseSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class LaptopSpecificationToDtoMapper : MapperBase<LaptopSpecification, LaptopSpecificationDto>
{
    public override partial LaptopSpecificationDto Map(LaptopSpecification source);
    public override partial void Map(LaptopSpecification source, LaptopSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class HeadsetSpecificationToDtoMapper : MapperBase<HeadsetSpecification, HeadsetSpecificationDto>
{
    public override partial HeadsetSpecificationDto Map(HeadsetSpecification source);
    public override partial void Map(HeadsetSpecification source, HeadsetSpecificationDto destination);
}

#endregion

#region New Specifications - Entity to DTO

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CaseFanSpecificationToDtoMapper : MapperBase<CaseFanSpecification, CaseFanSpecificationDto>
{
    public override partial CaseFanSpecificationDto Map(CaseFanSpecification source);
    public override partial void Map(CaseFanSpecification source, CaseFanSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MemoryCardSpecificationToDtoMapper : MapperBase<MemoryCardSpecification, MemoryCardSpecificationDto>
{
    public override partial MemoryCardSpecificationDto Map(MemoryCardSpecification source);
    public override partial void Map(MemoryCardSpecification source, MemoryCardSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SpeakerSpecificationToDtoMapper : MapperBase<SpeakerSpecification, SpeakerSpecificationDto>
{
    public override partial SpeakerSpecificationDto Map(SpeakerSpecification source);
    public override partial void Map(SpeakerSpecification source, SpeakerSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MicrophoneSpecificationToDtoMapper : MapperBase<MicrophoneSpecification, MicrophoneSpecificationDto>
{
    public override partial MicrophoneSpecificationDto Map(MicrophoneSpecification source);
    public override partial void Map(MicrophoneSpecification source, MicrophoneSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class WebcamSpecificationToDtoMapper : MapperBase<WebcamSpecification, WebcamSpecificationDto>
{
    public override partial WebcamSpecificationDto Map(WebcamSpecification source);
    public override partial void Map(WebcamSpecification source, WebcamSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MousePadSpecificationToDtoMapper : MapperBase<MousePadSpecification, MousePadSpecificationDto>
{
    public override partial MousePadSpecificationDto Map(MousePadSpecification source);
    public override partial void Map(MousePadSpecification source, MousePadSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ChairSpecificationToDtoMapper : MapperBase<ChairSpecification, ChairSpecificationDto>
{
    public override partial ChairSpecificationDto Map(ChairSpecification source);
    public override partial void Map(ChairSpecification source, ChairSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class DeskSpecificationToDtoMapper : MapperBase<DeskSpecification, DeskSpecificationDto>
{
    public override partial DeskSpecificationDto Map(DeskSpecification source);
    public override partial void Map(DeskSpecification source, DeskSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SoftwareSpecificationToDtoMapper : MapperBase<SoftwareSpecification, SoftwareSpecificationDto>
{
    public override partial SoftwareSpecificationDto Map(SoftwareSpecification source);
    public override partial void Map(SoftwareSpecification source, SoftwareSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class NetworkHardwareSpecificationToDtoMapper : MapperBase<NetworkHardwareSpecification, NetworkHardwareSpecificationDto>
{
    public override partial NetworkHardwareSpecificationDto Map(NetworkHardwareSpecification source);
    public override partial void Map(NetworkHardwareSpecification source, NetworkHardwareSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class HandheldSpecificationToDtoMapper : MapperBase<HandheldSpecification, HandheldSpecificationDto>
{
    public override partial HandheldSpecificationDto Map(HandheldSpecification source);
    public override partial void Map(HandheldSpecification source, HandheldSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ConsoleSpecificationToDtoMapper : MapperBase<ConsoleSpecification, ConsoleSpecificationDto>
{
    public override partial ConsoleSpecificationDto Map(ConsoleSpecification source);
    public override partial void Map(ConsoleSpecification source, ConsoleSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class HubSpecificationToDtoMapper : MapperBase<HubSpecification, HubSpecificationDto>
{
    public override partial HubSpecificationDto Map(HubSpecification source);
    public override partial void Map(HubSpecification source, HubSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CableSpecificationToDtoMapper : MapperBase<CableSpecification, CableSpecificationDto>
{
    public override partial CableSpecificationDto Map(CableSpecification source);
    public override partial void Map(CableSpecification source, CableSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ChargerSpecificationToDtoMapper : MapperBase<ChargerSpecification, ChargerSpecificationDto>
{
    public override partial ChargerSpecificationDto Map(ChargerSpecification source);
    public override partial void Map(ChargerSpecification source, ChargerSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PowerBankSpecificationToDtoMapper : MapperBase<PowerBankSpecification, PowerBankSpecificationDto>
{
    public override partial PowerBankSpecificationDto Map(PowerBankSpecification source);
    public override partial void Map(PowerBankSpecification source, PowerBankSpecificationDto destination);
}

#endregion

#region Specifications - DTO to CreateUpdateDTO

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CpuSpecDtoToCreateUpdateMapper : MapperBase<CpuSpecificationDto, CreateUpdateCpuSpecificationDto>
{
    public override partial CreateUpdateCpuSpecificationDto Map(CpuSpecificationDto source);
    public override partial void Map(CpuSpecificationDto source, CreateUpdateCpuSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class GpuSpecDtoToCreateUpdateMapper : MapperBase<GpuSpecificationDto, CreateUpdateGpuSpecificationDto>
{
    public override partial CreateUpdateGpuSpecificationDto Map(GpuSpecificationDto source);
    public override partial void Map(GpuSpecificationDto source, CreateUpdateGpuSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RamSpecDtoToCreateUpdateMapper : MapperBase<RamSpecificationDto, CreateUpdateRamSpecificationDto>
{
    public override partial CreateUpdateRamSpecificationDto Map(RamSpecificationDto source);
    public override partial void Map(RamSpecificationDto source, CreateUpdateRamSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MotherboardSpecDtoToCreateUpdateMapper : MapperBase<MotherboardSpecificationDto, CreateUpdateMotherboardSpecificationDto>
{
    public override partial CreateUpdateMotherboardSpecificationDto Map(MotherboardSpecificationDto source);
    public override partial void Map(MotherboardSpecificationDto source, CreateUpdateMotherboardSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class StorageSpecDtoToCreateUpdateMapper : MapperBase<StorageSpecificationDto, CreateUpdateStorageSpecificationDto>
{
    public override partial CreateUpdateStorageSpecificationDto Map(StorageSpecificationDto source);
    public override partial void Map(StorageSpecificationDto source, CreateUpdateStorageSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PsuSpecDtoToCreateUpdateMapper : MapperBase<PsuSpecificationDto, CreateUpdatePsuSpecificationDto>
{
    public override partial CreateUpdatePsuSpecificationDto Map(PsuSpecificationDto source);
    public override partial void Map(PsuSpecificationDto source, CreateUpdatePsuSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CaseSpecDtoToCreateUpdateMapper : MapperBase<CaseSpecificationDto, CreateUpdateCaseSpecificationDto>
{
    [MapperIgnoreTarget(nameof(CreateUpdateCaseSpecificationDto.MaterialIds))]
    public override partial CreateUpdateCaseSpecificationDto Map(CaseSpecificationDto source);

    [MapperIgnoreTarget(nameof(CreateUpdateCaseSpecificationDto.MaterialIds))]
    public override partial void Map(CaseSpecificationDto source, CreateUpdateCaseSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CpuCoolerSpecDtoToCreateUpdateMapper : MapperBase<CpuCoolerSpecificationDto, CreateUpdateCpuCoolerSpecificationDto>
{
    [MapperIgnoreTarget(nameof(CreateUpdateCpuCoolerSpecificationDto.SupportedSocketIds))]
    public override partial CreateUpdateCpuCoolerSpecificationDto Map(CpuCoolerSpecificationDto source);

    [MapperIgnoreTarget(nameof(CreateUpdateCpuCoolerSpecificationDto.SupportedSocketIds))]
    public override partial void Map(CpuCoolerSpecificationDto source, CreateUpdateCpuCoolerSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class KeyboardSpecDtoToCreateUpdateMapper : MapperBase<KeyboardSpecificationDto, CreateUpdateKeyboardSpecificationDto>
{
    public override partial CreateUpdateKeyboardSpecificationDto Map(KeyboardSpecificationDto source);
    public override partial void Map(KeyboardSpecificationDto source, CreateUpdateKeyboardSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MonitorSpecDtoToCreateUpdateMapper : MapperBase<MonitorSpecificationDto, CreateUpdateMonitorSpecificationDto>
{
    public override partial CreateUpdateMonitorSpecificationDto Map(MonitorSpecificationDto source);
    public override partial void Map(MonitorSpecificationDto source, CreateUpdateMonitorSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MouseSpecDtoToCreateUpdateMapper : MapperBase<MouseSpecificationDto, CreateUpdateMouseSpecificationDto>
{
    public override partial CreateUpdateMouseSpecificationDto Map(MouseSpecificationDto source);
    public override partial void Map(MouseSpecificationDto source, CreateUpdateMouseSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class LaptopSpecDtoToCreateUpdateMapper : MapperBase<LaptopSpecificationDto, CreateUpdateLaptopSpecificationDto>
{
    public override partial CreateUpdateLaptopSpecificationDto Map(LaptopSpecificationDto source);
    public override partial void Map(LaptopSpecificationDto source, CreateUpdateLaptopSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class HeadsetSpecDtoToCreateUpdateMapper : MapperBase<HeadsetSpecificationDto, CreateUpdateHeadsetSpecificationDto>
{
    public override partial CreateUpdateHeadsetSpecificationDto Map(HeadsetSpecificationDto source);
    public override partial void Map(HeadsetSpecificationDto source, CreateUpdateHeadsetSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CaseFanSpecDtoToCreateUpdateMapper : MapperBase<CaseFanSpecificationDto, CreateUpdateCaseFanSpecificationDto>
{
    public override partial CreateUpdateCaseFanSpecificationDto Map(CaseFanSpecificationDto source);
    public override partial void Map(CaseFanSpecificationDto source, CreateUpdateCaseFanSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MemoryCardSpecDtoToCreateUpdateMapper : MapperBase<MemoryCardSpecificationDto, CreateUpdateMemoryCardSpecificationDto>
{
    public override partial CreateUpdateMemoryCardSpecificationDto Map(MemoryCardSpecificationDto source);
    public override partial void Map(MemoryCardSpecificationDto source, CreateUpdateMemoryCardSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SpeakerSpecDtoToCreateUpdateMapper : MapperBase<SpeakerSpecificationDto, CreateUpdateSpeakerSpecificationDto>
{
    public override partial CreateUpdateSpeakerSpecificationDto Map(SpeakerSpecificationDto source);
    public override partial void Map(SpeakerSpecificationDto source, CreateUpdateSpeakerSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MicrophoneSpecDtoToCreateUpdateMapper : MapperBase<MicrophoneSpecificationDto, CreateUpdateMicrophoneSpecificationDto>
{
    public override partial CreateUpdateMicrophoneSpecificationDto Map(MicrophoneSpecificationDto source);
    public override partial void Map(MicrophoneSpecificationDto source, CreateUpdateMicrophoneSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class WebcamSpecDtoToCreateUpdateMapper : MapperBase<WebcamSpecificationDto, CreateUpdateWebcamSpecificationDto>
{
    public override partial CreateUpdateWebcamSpecificationDto Map(WebcamSpecificationDto source);
    public override partial void Map(WebcamSpecificationDto source, CreateUpdateWebcamSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MousePadSpecDtoToCreateUpdateMapper : MapperBase<MousePadSpecificationDto, CreateUpdateMousePadSpecificationDto>
{
    public override partial CreateUpdateMousePadSpecificationDto Map(MousePadSpecificationDto source);
    public override partial void Map(MousePadSpecificationDto source, CreateUpdateMousePadSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ChairSpecDtoToCreateUpdateMapper : MapperBase<ChairSpecificationDto, CreateUpdateChairSpecificationDto>
{
    public override partial CreateUpdateChairSpecificationDto Map(ChairSpecificationDto source);
    public override partial void Map(ChairSpecificationDto source, CreateUpdateChairSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class DeskSpecDtoToCreateUpdateMapper : MapperBase<DeskSpecificationDto, CreateUpdateDeskSpecificationDto>
{
    public override partial CreateUpdateDeskSpecificationDto Map(DeskSpecificationDto source);
    public override partial void Map(DeskSpecificationDto source, CreateUpdateDeskSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SoftwareSpecDtoToCreateUpdateMapper : MapperBase<SoftwareSpecificationDto, CreateUpdateSoftwareSpecificationDto>
{
    public override partial CreateUpdateSoftwareSpecificationDto Map(SoftwareSpecificationDto source);
    public override partial void Map(SoftwareSpecificationDto source, CreateUpdateSoftwareSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class NetworkHardwareSpecDtoToCreateUpdateMapper : MapperBase<NetworkHardwareSpecificationDto, CreateUpdateNetworkHardwareSpecificationDto>
{
    public override partial CreateUpdateNetworkHardwareSpecificationDto Map(NetworkHardwareSpecificationDto source);
    public override partial void Map(NetworkHardwareSpecificationDto source, CreateUpdateNetworkHardwareSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class HandheldSpecDtoToCreateUpdateMapper : MapperBase<HandheldSpecificationDto, CreateUpdateHandheldSpecificationDto>
{
    public override partial CreateUpdateHandheldSpecificationDto Map(HandheldSpecificationDto source);
    public override partial void Map(HandheldSpecificationDto source, CreateUpdateHandheldSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ConsoleSpecDtoToCreateUpdateMapper : MapperBase<ConsoleSpecificationDto, CreateUpdateConsoleSpecificationDto>
{
    public override partial CreateUpdateConsoleSpecificationDto Map(ConsoleSpecificationDto source);
    public override partial void Map(ConsoleSpecificationDto source, CreateUpdateConsoleSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class HubSpecDtoToCreateUpdateMapper : MapperBase<HubSpecificationDto, CreateUpdateHubSpecificationDto>
{
    public override partial CreateUpdateHubSpecificationDto Map(HubSpecificationDto source);
    public override partial void Map(HubSpecificationDto source, CreateUpdateHubSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CableSpecDtoToCreateUpdateMapper : MapperBase<CableSpecificationDto, CreateUpdateCableSpecificationDto>
{
    public override partial CreateUpdateCableSpecificationDto Map(CableSpecificationDto source);
    public override partial void Map(CableSpecificationDto source, CreateUpdateCableSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ChargerSpecDtoToCreateUpdateMapper : MapperBase<ChargerSpecificationDto, CreateUpdateChargerSpecificationDto>
{
    public override partial CreateUpdateChargerSpecificationDto Map(ChargerSpecificationDto source);
    public override partial void Map(ChargerSpecificationDto source, CreateUpdateChargerSpecificationDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PowerBankSpecDtoToCreateUpdateMapper : MapperBase<PowerBankSpecificationDto, CreateUpdatePowerBankSpecificationDto>
{
    public override partial CreateUpdatePowerBankSpecificationDto Map(PowerBankSpecificationDto source);
    public override partial void Map(PowerBankSpecificationDto source, CreateUpdatePowerBankSpecificationDto destination);
}

#endregion

#region Specifications - CreateUpdateDTO to Entity

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateCpuSpecToEntityMapper : MapperBase<CreateUpdateCpuSpecificationDto, CpuSpecification>
{
    [MapperIgnoreTarget(nameof(CpuSpecification.Id))]
    [MapperIgnoreTarget(nameof(CpuSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(CpuSpecification.Product))]
    [MapperIgnoreTarget(nameof(CpuSpecification.Socket))]
    public override partial CpuSpecification Map(CreateUpdateCpuSpecificationDto source);

    [MapperIgnoreTarget(nameof(CpuSpecification.Id))]
    [MapperIgnoreTarget(nameof(CpuSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(CpuSpecification.Product))]
    [MapperIgnoreTarget(nameof(CpuSpecification.Socket))]
    public override partial void Map(CreateUpdateCpuSpecificationDto source, CpuSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateGpuSpecToEntityMapper : MapperBase<CreateUpdateGpuSpecificationDto, GpuSpecification>
{
    [MapperIgnoreTarget(nameof(GpuSpecification.Id))]
    [MapperIgnoreTarget(nameof(GpuSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(GpuSpecification.Product))]
    public override partial GpuSpecification Map(CreateUpdateGpuSpecificationDto source);

    [MapperIgnoreTarget(nameof(GpuSpecification.Id))]
    [MapperIgnoreTarget(nameof(GpuSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(GpuSpecification.Product))]
    public override partial void Map(CreateUpdateGpuSpecificationDto source, GpuSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateRamSpecToEntityMapper : MapperBase<CreateUpdateRamSpecificationDto, RamSpecification>
{
    [MapperIgnoreTarget(nameof(RamSpecification.Id))]
    [MapperIgnoreTarget(nameof(RamSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(RamSpecification.Product))]
    [MapperIgnoreTarget(nameof(RamSpecification.RamType))]
    public override partial RamSpecification Map(CreateUpdateRamSpecificationDto source);

    [MapperIgnoreTarget(nameof(RamSpecification.Id))]
    [MapperIgnoreTarget(nameof(RamSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(RamSpecification.Product))]
    [MapperIgnoreTarget(nameof(RamSpecification.RamType))]
    public override partial void Map(CreateUpdateRamSpecificationDto source, RamSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateMotherboardSpecToEntityMapper : MapperBase<CreateUpdateMotherboardSpecificationDto, MotherboardSpecification>
{
    [MapperIgnoreTarget(nameof(MotherboardSpecification.Id))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.Product))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.Socket))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.Chipset))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.FormFactor))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.SupportedRamTypes))]
    public override partial MotherboardSpecification Map(CreateUpdateMotherboardSpecificationDto source);

    [MapperIgnoreTarget(nameof(MotherboardSpecification.Id))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.Product))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.Socket))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.Chipset))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.FormFactor))]
    [MapperIgnoreTarget(nameof(MotherboardSpecification.SupportedRamTypes))]
    public override partial void Map(CreateUpdateMotherboardSpecificationDto source, MotherboardSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateStorageSpecToEntityMapper : MapperBase<CreateUpdateStorageSpecificationDto, StorageSpecification>
{
    [MapperIgnoreTarget(nameof(StorageSpecification.Id))]
    [MapperIgnoreTarget(nameof(StorageSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(StorageSpecification.Product))]
    public override partial StorageSpecification Map(CreateUpdateStorageSpecificationDto source);

    [MapperIgnoreTarget(nameof(StorageSpecification.Id))]
    [MapperIgnoreTarget(nameof(StorageSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(StorageSpecification.Product))]
    public override partial void Map(CreateUpdateStorageSpecificationDto source, StorageSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdatePsuSpecToEntityMapper : MapperBase<CreateUpdatePsuSpecificationDto, PsuSpecification>
{
    [MapperIgnoreTarget(nameof(PsuSpecification.Id))]
    [MapperIgnoreTarget(nameof(PsuSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(PsuSpecification.Product))]
    [MapperIgnoreTarget(nameof(PsuSpecification.FormFactor))]
    public override partial PsuSpecification Map(CreateUpdatePsuSpecificationDto source);

    [MapperIgnoreTarget(nameof(PsuSpecification.Id))]
    [MapperIgnoreTarget(nameof(PsuSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(PsuSpecification.Product))]
    [MapperIgnoreTarget(nameof(PsuSpecification.FormFactor))]
    public override partial void Map(CreateUpdatePsuSpecificationDto source, PsuSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateCaseSpecToEntityMapper : MapperBase<CreateUpdateCaseSpecificationDto, CaseSpecification>
{
    [MapperIgnoreTarget(nameof(CaseSpecification.Id))]
    [MapperIgnoreTarget(nameof(CaseSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(CaseSpecification.Product))]
    [MapperIgnoreTarget(nameof(CaseSpecification.FormFactor))]
    [MapperIgnoreTarget(nameof(CaseSpecification.Materials))]
    public override partial CaseSpecification Map(CreateUpdateCaseSpecificationDto source);

    [MapperIgnoreTarget(nameof(CaseSpecification.Id))]
    [MapperIgnoreTarget(nameof(CaseSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(CaseSpecification.Product))]
    [MapperIgnoreTarget(nameof(CaseSpecification.FormFactor))]
    [MapperIgnoreTarget(nameof(CaseSpecification.Materials))]
    public override partial void Map(CreateUpdateCaseSpecificationDto source, CaseSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateCpuCoolerSpecToEntityMapper : MapperBase<CreateUpdateCpuCoolerSpecificationDto, CpuCoolerSpecification>
{
    [MapperIgnoreTarget(nameof(CpuCoolerSpecification.Id))]
    [MapperIgnoreTarget(nameof(CpuCoolerSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(CpuCoolerSpecification.Product))]
    [MapperIgnoreTarget(nameof(CpuCoolerSpecification.SupportedSockets))]
    public override partial CpuCoolerSpecification Map(CreateUpdateCpuCoolerSpecificationDto source);

    [MapperIgnoreTarget(nameof(CpuCoolerSpecification.Id))]
    [MapperIgnoreTarget(nameof(CpuCoolerSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(CpuCoolerSpecification.Product))]
    [MapperIgnoreTarget(nameof(CpuCoolerSpecification.SupportedSockets))]
    public override partial void Map(CreateUpdateCpuCoolerSpecificationDto source, CpuCoolerSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateKeyboardSpecToEntityMapper : MapperBase<CreateUpdateKeyboardSpecificationDto, KeyboardSpecification>
{
    [MapperIgnoreTarget(nameof(KeyboardSpecification.Id))]
    [MapperIgnoreTarget(nameof(KeyboardSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(KeyboardSpecification.Product))]
    [MapperIgnoreTarget(nameof(KeyboardSpecification.SwitchType))]
    public override partial KeyboardSpecification Map(CreateUpdateKeyboardSpecificationDto source);

    [MapperIgnoreTarget(nameof(KeyboardSpecification.Id))]
    [MapperIgnoreTarget(nameof(KeyboardSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(KeyboardSpecification.Product))]
    [MapperIgnoreTarget(nameof(KeyboardSpecification.SwitchType))]
    public override partial void Map(CreateUpdateKeyboardSpecificationDto source, KeyboardSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateMonitorSpecToEntityMapper : MapperBase<CreateUpdateMonitorSpecificationDto, MonitorSpecification>
{
    [MapperIgnoreTarget(nameof(MonitorSpecification.Id))]
    [MapperIgnoreTarget(nameof(MonitorSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(MonitorSpecification.Product))]
    [MapperIgnoreTarget(nameof(MonitorSpecification.PanelType))]
    public override partial MonitorSpecification Map(CreateUpdateMonitorSpecificationDto source);

    [MapperIgnoreTarget(nameof(MonitorSpecification.Id))]
    [MapperIgnoreTarget(nameof(MonitorSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(MonitorSpecification.Product))]
    [MapperIgnoreTarget(nameof(MonitorSpecification.PanelType))]
    public override partial void Map(CreateUpdateMonitorSpecificationDto source, MonitorSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateMouseSpecToEntityMapper : MapperBase<CreateUpdateMouseSpecificationDto, MouseSpecification>
{
    [MapperIgnoreTarget(nameof(MouseSpecification.Id))]
    [MapperIgnoreTarget(nameof(MouseSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(MouseSpecification.Product))]
    public override partial MouseSpecification Map(CreateUpdateMouseSpecificationDto source);

    [MapperIgnoreTarget(nameof(MouseSpecification.Id))]
    [MapperIgnoreTarget(nameof(MouseSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(MouseSpecification.Product))]
    public override partial void Map(CreateUpdateMouseSpecificationDto source, MouseSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateLaptopSpecToEntityMapper : MapperBase<CreateUpdateLaptopSpecificationDto, LaptopSpecification>
{
    [MapperIgnoreTarget(nameof(LaptopSpecification.Id))]
    [MapperIgnoreTarget(nameof(LaptopSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(LaptopSpecification.Product))]
    public override partial LaptopSpecification Map(CreateUpdateLaptopSpecificationDto source);

    [MapperIgnoreTarget(nameof(LaptopSpecification.Id))]
    [MapperIgnoreTarget(nameof(LaptopSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(LaptopSpecification.Product))]
    public override partial void Map(CreateUpdateLaptopSpecificationDto source, LaptopSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateHeadsetSpecToEntityMapper : MapperBase<CreateUpdateHeadsetSpecificationDto, HeadsetSpecification>
{
    [MapperIgnoreTarget(nameof(HeadsetSpecification.Id))]
    [MapperIgnoreTarget(nameof(HeadsetSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(HeadsetSpecification.Product))]
    public override partial HeadsetSpecification Map(CreateUpdateHeadsetSpecificationDto source);

    [MapperIgnoreTarget(nameof(HeadsetSpecification.Id))]
    [MapperIgnoreTarget(nameof(HeadsetSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(HeadsetSpecification.Product))]
    public override partial void Map(CreateUpdateHeadsetSpecificationDto source, HeadsetSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateCaseFanSpecToEntityMapper : MapperBase<CreateUpdateCaseFanSpecificationDto, CaseFanSpecification>
{
    [MapperIgnoreTarget(nameof(CaseFanSpecification.Id))]
    [MapperIgnoreTarget(nameof(CaseFanSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(CaseFanSpecification.Product))]
    public override partial CaseFanSpecification Map(CreateUpdateCaseFanSpecificationDto source);

    [MapperIgnoreTarget(nameof(CaseFanSpecification.Id))]
    [MapperIgnoreTarget(nameof(CaseFanSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(CaseFanSpecification.Product))]
    public override partial void Map(CreateUpdateCaseFanSpecificationDto source, CaseFanSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateMemoryCardSpecToEntityMapper : MapperBase<CreateUpdateMemoryCardSpecificationDto, MemoryCardSpecification>
{
    [MapperIgnoreTarget(nameof(MemoryCardSpecification.Id))]
    [MapperIgnoreTarget(nameof(MemoryCardSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(MemoryCardSpecification.Product))]
    public override partial MemoryCardSpecification Map(CreateUpdateMemoryCardSpecificationDto source);

    [MapperIgnoreTarget(nameof(MemoryCardSpecification.Id))]
    [MapperIgnoreTarget(nameof(MemoryCardSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(MemoryCardSpecification.Product))]
    public override partial void Map(CreateUpdateMemoryCardSpecificationDto source, MemoryCardSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateSpeakerSpecToEntityMapper : MapperBase<CreateUpdateSpeakerSpecificationDto, SpeakerSpecification>
{
    [MapperIgnoreTarget(nameof(SpeakerSpecification.Id))]
    [MapperIgnoreTarget(nameof(SpeakerSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(SpeakerSpecification.Product))]
    public override partial SpeakerSpecification Map(CreateUpdateSpeakerSpecificationDto source);

    [MapperIgnoreTarget(nameof(SpeakerSpecification.Id))]
    [MapperIgnoreTarget(nameof(SpeakerSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(SpeakerSpecification.Product))]
    public override partial void Map(CreateUpdateSpeakerSpecificationDto source, SpeakerSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateMicrophoneSpecToEntityMapper : MapperBase<CreateUpdateMicrophoneSpecificationDto, MicrophoneSpecification>
{
    [MapperIgnoreTarget(nameof(MicrophoneSpecification.Id))]
    [MapperIgnoreTarget(nameof(MicrophoneSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(MicrophoneSpecification.Product))]
    public override partial MicrophoneSpecification Map(CreateUpdateMicrophoneSpecificationDto source);

    [MapperIgnoreTarget(nameof(MicrophoneSpecification.Id))]
    [MapperIgnoreTarget(nameof(MicrophoneSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(MicrophoneSpecification.Product))]
    public override partial void Map(CreateUpdateMicrophoneSpecificationDto source, MicrophoneSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateWebcamSpecToEntityMapper : MapperBase<CreateUpdateWebcamSpecificationDto, WebcamSpecification>
{
    [MapperIgnoreTarget(nameof(WebcamSpecification.Id))]
    [MapperIgnoreTarget(nameof(WebcamSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(WebcamSpecification.Product))]
    public override partial WebcamSpecification Map(CreateUpdateWebcamSpecificationDto source);

    [MapperIgnoreTarget(nameof(WebcamSpecification.Id))]
    [MapperIgnoreTarget(nameof(WebcamSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(WebcamSpecification.Product))]
    public override partial void Map(CreateUpdateWebcamSpecificationDto source, WebcamSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateMousePadSpecToEntityMapper : MapperBase<CreateUpdateMousePadSpecificationDto, MousePadSpecification>
{
    [MapperIgnoreTarget(nameof(MousePadSpecification.Id))]
    [MapperIgnoreTarget(nameof(MousePadSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(MousePadSpecification.Product))]
    public override partial MousePadSpecification Map(CreateUpdateMousePadSpecificationDto source);

    [MapperIgnoreTarget(nameof(MousePadSpecification.Id))]
    [MapperIgnoreTarget(nameof(MousePadSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(MousePadSpecification.Product))]
    public override partial void Map(CreateUpdateMousePadSpecificationDto source, MousePadSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateChairSpecToEntityMapper : MapperBase<CreateUpdateChairSpecificationDto, ChairSpecification>
{
    [MapperIgnoreTarget(nameof(ChairSpecification.Id))]
    [MapperIgnoreTarget(nameof(ChairSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(ChairSpecification.Product))]
    public override partial ChairSpecification Map(CreateUpdateChairSpecificationDto source);

    [MapperIgnoreTarget(nameof(ChairSpecification.Id))]
    [MapperIgnoreTarget(nameof(ChairSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(ChairSpecification.Product))]
    public override partial void Map(CreateUpdateChairSpecificationDto source, ChairSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateDeskSpecToEntityMapper : MapperBase<CreateUpdateDeskSpecificationDto, DeskSpecification>
{
    [MapperIgnoreTarget(nameof(DeskSpecification.Id))]
    [MapperIgnoreTarget(nameof(DeskSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(DeskSpecification.Product))]
    public override partial DeskSpecification Map(CreateUpdateDeskSpecificationDto source);

    [MapperIgnoreTarget(nameof(DeskSpecification.Id))]
    [MapperIgnoreTarget(nameof(DeskSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(DeskSpecification.Product))]
    public override partial void Map(CreateUpdateDeskSpecificationDto source, DeskSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateSoftwareSpecToEntityMapper : MapperBase<CreateUpdateSoftwareSpecificationDto, SoftwareSpecification>
{
    [MapperIgnoreTarget(nameof(SoftwareSpecification.Id))]
    [MapperIgnoreTarget(nameof(SoftwareSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(SoftwareSpecification.Product))]
    public override partial SoftwareSpecification Map(CreateUpdateSoftwareSpecificationDto source);

    [MapperIgnoreTarget(nameof(SoftwareSpecification.Id))]
    [MapperIgnoreTarget(nameof(SoftwareSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(SoftwareSpecification.Product))]
    public override partial void Map(CreateUpdateSoftwareSpecificationDto source, SoftwareSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateNetworkHardwareSpecToEntityMapper : MapperBase<CreateUpdateNetworkHardwareSpecificationDto, NetworkHardwareSpecification>
{
    [MapperIgnoreTarget(nameof(NetworkHardwareSpecification.Id))]
    [MapperIgnoreTarget(nameof(NetworkHardwareSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(NetworkHardwareSpecification.Product))]
    public override partial NetworkHardwareSpecification Map(CreateUpdateNetworkHardwareSpecificationDto source);

    [MapperIgnoreTarget(nameof(NetworkHardwareSpecification.Id))]
    [MapperIgnoreTarget(nameof(NetworkHardwareSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(NetworkHardwareSpecification.Product))]
    public override partial void Map(CreateUpdateNetworkHardwareSpecificationDto source, NetworkHardwareSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateHandheldSpecToEntityMapper : MapperBase<CreateUpdateHandheldSpecificationDto, HandheldSpecification>
{
    [MapperIgnoreTarget(nameof(HandheldSpecification.Id))]
    [MapperIgnoreTarget(nameof(HandheldSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(HandheldSpecification.Product))]
    public override partial HandheldSpecification Map(CreateUpdateHandheldSpecificationDto source);

    [MapperIgnoreTarget(nameof(HandheldSpecification.Id))]
    [MapperIgnoreTarget(nameof(HandheldSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(HandheldSpecification.Product))]
    public override partial void Map(CreateUpdateHandheldSpecificationDto source, HandheldSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateConsoleSpecToEntityMapper : MapperBase<CreateUpdateConsoleSpecificationDto, ConsoleSpecification>
{
    [MapperIgnoreTarget(nameof(ConsoleSpecification.Id))]
    [MapperIgnoreTarget(nameof(ConsoleSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(ConsoleSpecification.Product))]
    public override partial ConsoleSpecification Map(CreateUpdateConsoleSpecificationDto source);

    [MapperIgnoreTarget(nameof(ConsoleSpecification.Id))]
    [MapperIgnoreTarget(nameof(ConsoleSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(ConsoleSpecification.Product))]
    public override partial void Map(CreateUpdateConsoleSpecificationDto source, ConsoleSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateHubSpecToEntityMapper : MapperBase<CreateUpdateHubSpecificationDto, HubSpecification>
{
    [MapperIgnoreTarget(nameof(HubSpecification.Id))]
    [MapperIgnoreTarget(nameof(HubSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(HubSpecification.Product))]
    public override partial HubSpecification Map(CreateUpdateHubSpecificationDto source);

    [MapperIgnoreTarget(nameof(HubSpecification.Id))]
    [MapperIgnoreTarget(nameof(HubSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(HubSpecification.Product))]
    public override partial void Map(CreateUpdateHubSpecificationDto source, HubSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateCableSpecToEntityMapper : MapperBase<CreateUpdateCableSpecificationDto, CableSpecification>
{
    [MapperIgnoreTarget(nameof(CableSpecification.Id))]
    [MapperIgnoreTarget(nameof(CableSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(CableSpecification.Product))]
    public override partial CableSpecification Map(CreateUpdateCableSpecificationDto source);

    [MapperIgnoreTarget(nameof(CableSpecification.Id))]
    [MapperIgnoreTarget(nameof(CableSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(CableSpecification.Product))]
    public override partial void Map(CreateUpdateCableSpecificationDto source, CableSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateChargerSpecToEntityMapper : MapperBase<CreateUpdateChargerSpecificationDto, ChargerSpecification>
{
    [MapperIgnoreTarget(nameof(ChargerSpecification.Id))]
    [MapperIgnoreTarget(nameof(ChargerSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(ChargerSpecification.Product))]
    public override partial ChargerSpecification Map(CreateUpdateChargerSpecificationDto source);

    [MapperIgnoreTarget(nameof(ChargerSpecification.Id))]
    [MapperIgnoreTarget(nameof(ChargerSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(ChargerSpecification.Product))]
    public override partial void Map(CreateUpdateChargerSpecificationDto source, ChargerSpecification destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdatePowerBankSpecToEntityMapper : MapperBase<CreateUpdatePowerBankSpecificationDto, PowerBankSpecification>
{
    [MapperIgnoreTarget(nameof(PowerBankSpecification.Id))]
    [MapperIgnoreTarget(nameof(PowerBankSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(PowerBankSpecification.Product))]
    public override partial PowerBankSpecification Map(CreateUpdatePowerBankSpecificationDto source);

    [MapperIgnoreTarget(nameof(PowerBankSpecification.Id))]
    [MapperIgnoreTarget(nameof(PowerBankSpecification.ProductId))]
    [MapperIgnoreTarget(nameof(PowerBankSpecification.Product))]
    public override partial void Map(CreateUpdatePowerBankSpecificationDto source, PowerBankSpecification destination);
}

#endregion