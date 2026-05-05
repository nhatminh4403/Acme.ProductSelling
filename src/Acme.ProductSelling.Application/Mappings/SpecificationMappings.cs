using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Models;
using Riok.Mapperly.Abstractions;
using System;
using System.Linq;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Products;


#region Entity to DTO

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CpuSpecificationToDtoMapper : MapperBase<CpuSpecification, CpuSpecificationDto>
{
    [MapProperty(nameof(CpuSpecification.Socket.Name), nameof(CpuSpecificationDto.SocketName))]
    public override  CpuSpecificationDto Map(CpuSpecification source)
    {
        var destination = new CpuSpecificationDto();
        Map(source, destination);
        AfterMap(source, destination);
        return destination;
    }
    public override partial void Map(CpuSpecification source, CpuSpecificationDto destination);

    public override void AfterMap(CpuSpecification source, CpuSpecificationDto destination)
    {
        destination.SocketName = source.Socket?.Name;
    }
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
    [MapperIgnoreTarget(nameof(RamSpecificationDto.RamTypeName))]
    public override RamSpecificationDto Map(RamSpecification source)
    {
        var destination = new RamSpecificationDto();
        Map(source, destination);
        AfterMap(source, destination);
        return destination;
    }
    [MapperIgnoreTarget(nameof(RamSpecificationDto.RamTypeName))]
    public override partial void Map(RamSpecification source, RamSpecificationDto destination);

    public override void AfterMap(RamSpecification source, RamSpecificationDto destination)
    {
        destination.RamTypeName = source.RamType?.Name ?? string.Empty;
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MotherboardSpecificationToDtoMapper : MapperBase<MotherboardSpecification, MotherboardSpecificationDto>
{
    [MapProperty(nameof(MotherboardSpecification.Socket.Name), nameof(MotherboardSpecificationDto.SocketName))]
    [MapProperty(nameof(MotherboardSpecification.Chipset.Name), nameof(MotherboardSpecificationDto.ChipsetName))]
    [MapProperty(nameof(MotherboardSpecification.FormFactor.Name), nameof(MotherboardSpecificationDto.FormFactorName))]
    [MapProperty(nameof(MotherboardSpecification.SupportedRamTypes.Name), nameof(MotherboardSpecificationDto.SupportedRamTypeName))]
    [MapperIgnoreTarget(nameof(MotherboardSpecificationDto.SocketName))]
    [MapperIgnoreTarget(nameof(MotherboardSpecificationDto.ChipsetName))]
    [MapperIgnoreTarget(nameof(MotherboardSpecificationDto.FormFactorName))]
    [MapperIgnoreTarget(nameof(MotherboardSpecificationDto.SupportedRamTypeName))]
    public override MotherboardSpecificationDto Map(MotherboardSpecification source)
    {
        var destination = new MotherboardSpecificationDto();
        Map(source, destination);
        AfterMap(source, destination);
        return destination;
    }
    [MapperIgnoreTarget(nameof(MotherboardSpecificationDto.SocketName))]
    [MapperIgnoreTarget(nameof(MotherboardSpecificationDto.ChipsetName))]
    [MapperIgnoreTarget(nameof(MotherboardSpecificationDto.FormFactorName))]
    [MapperIgnoreTarget(nameof(MotherboardSpecificationDto.SupportedRamTypeName))]
    public override partial void Map(MotherboardSpecification source, MotherboardSpecificationDto destination);
    public override void AfterMap(MotherboardSpecification source, MotherboardSpecificationDto destination)
    {
        destination.SocketName = source.Socket?.Name;
        destination.ChipsetName = source.Chipset?.Name;
        destination.FormFactorName = source.FormFactor?.Name;
        destination.SupportedRamTypeName = source.SupportedRamTypes?.Name;
    }
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
    [MapperIgnoreTarget(nameof(PsuSpecificationDto.FormFactorName))]
    public override PsuSpecificationDto Map(PsuSpecification source)
    {
        var dest = new PsuSpecificationDto();
        Map(source, dest);
        AfterMap(source, dest);
        return dest;
    }
    [MapperIgnoreTarget(nameof(PsuSpecificationDto.FormFactorName))]
    public override partial void Map(PsuSpecification source, PsuSpecificationDto destination);

    public override void AfterMap(PsuSpecification source, PsuSpecificationDto destination)
    {
        destination.FormFactorName = source.FormFactor?.Name;
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CaseSpecificationToDtoMapper : MapperBase<CaseSpecification, CaseSpecificationDto>
{
    [MapProperty(nameof(CaseSpecification.FormFactor.Name), nameof(CaseSpecificationDto.SupportedMbFormFactorName))]
    [MapperIgnoreTarget(nameof(CaseSpecificationDto.MaterialNames))] // Calculated in AfterMap
    public override CaseSpecificationDto Map(CaseSpecification source)
    {
        var destination = new CaseSpecificationDto();
        Map(source, destination);
        AfterMap(source, destination);
        return destination;
    }

    [MapperIgnoreTarget(nameof(CaseSpecificationDto.MaterialNames))]
    public override partial void Map(CaseSpecification source, CaseSpecificationDto destination);

    public override void AfterMap(CaseSpecification source, CaseSpecificationDto destination)
    {
        destination.MaterialNames = source.Materials?.Select(m => m.Material?.Name).Where(x => x != null).ToList();
        destination.SupportedMbFormFactorName = source.FormFactor?.Name;
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CpuCoolerSpecificationToDtoMapper : MapperBase<CpuCoolerSpecification, CpuCoolerSpecificationDto>
{
    [MapperIgnoreTarget(nameof(CpuCoolerSpecificationDto.SupportedSocketNames))] // Calculated in AfterMap
    public override CpuCoolerSpecificationDto Map(CpuCoolerSpecification source)
    {
        var destination = new CpuCoolerSpecificationDto();
        Map(source, destination);
        AfterMap(source, destination);
        return destination;
    }

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
    [MapProperty(nameof(KeyboardSpecification.SwitchType.Name),
        nameof(KeyboardSpecificationDto.SwitchTypeName))]
    [MapperIgnoreTarget(nameof(KeyboardSpecificationDto.SwitchTypeName))]
    public override KeyboardSpecificationDto Map(KeyboardSpecification source)
    {
        var destination = new KeyboardSpecificationDto();
        Map(source, destination);
        AfterMap(source, destination);
        return destination;
    }
    [MapperIgnoreTarget(nameof(KeyboardSpecificationDto.SwitchTypeName))]
    public override partial void Map(KeyboardSpecification source, KeyboardSpecificationDto destination);

    public override void AfterMap(KeyboardSpecification source, KeyboardSpecificationDto destination)
    {
        destination.SwitchTypeName = source.SwitchType?.Name;
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MonitorSpecificationToDtoMapper : MapperBase<MonitorSpecification, MonitorSpecificationDto>
{
    //[MapProperty(nameof(MonitorSpecification.PanelType.Name), nameof(MonitorSpecificationDto.PanelTypeName))]
    [MapperIgnoreTarget(nameof(MonitorSpecificationDto.PanelTypeName))]

    public override MonitorSpecificationDto Map(MonitorSpecification source)
    {
        var destination = new MonitorSpecificationDto();
        Map(source, destination);
        AfterMap(source, destination);
        return destination;
    }
    //[MapProperty(nameof(MonitorSpecification.PanelType.Name), nameof(MonitorSpecificationDto.PanelTypeName))]
    [MapperIgnoreTarget(nameof(MonitorSpecificationDto.PanelTypeName))]
    public override partial void Map(MonitorSpecification source, MonitorSpecificationDto destination);

    public override void AfterMap(MonitorSpecification source, MonitorSpecificationDto destination)
    {
        destination.PanelTypeName = source.PanelType?.Name;
    }
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

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CaseFanSpecificationToDtoMapper : MapperBase<CaseFanSpecification, CaseFanSpecificationDto>
{
    public override partial CaseFanSpecificationDto Map(CaseFanSpecification source);
    public override partial void Map(CaseFanSpecification source, CaseFanSpecificationDto destination);
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
public partial class CableSpecificationToDtoMapper : MapperBase<CableSpecification, CableSpecificationDto>
{
    public override partial CableSpecificationDto Map(CableSpecification source);
    public override partial void Map(CableSpecification source, CableSpecificationDto destination);
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
public partial class CableSpecDtoToCreateUpdateMapper : MapperBase<CableSpecificationDto, CreateUpdateCableSpecificationDto>
{
    public override partial CreateUpdateCableSpecificationDto Map(CableSpecificationDto source);
    public override partial void Map(CableSpecificationDto source, CreateUpdateCableSpecificationDto destination);
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


#endregion

#region Polymorphic Base Coordinator
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SpecificationBaseToDtoMapper : MapperBase<SpecificationBase, SpecificationBaseDto>
{
    // Inject sub-mappers as fields with [UseMapper]
    [UseMapper] private readonly MonitorSpecificationToDtoMapper _monitorMapper = new();
    [UseMapper] private readonly MouseSpecificationToDtoMapper _mouseMapper = new();
    [UseMapper] private readonly LaptopSpecificationToDtoMapper _laptopMapper = new();
    [UseMapper] private readonly CpuSpecificationToDtoMapper _cpuMapper = new();
    [UseMapper] private readonly GpuSpecificationToDtoMapper _gpuMapper = new();
    [UseMapper] private readonly RamSpecificationToDtoMapper _ramMapper = new();
    [UseMapper] private readonly MotherboardSpecificationToDtoMapper _motherboardMapper = new();
    [UseMapper] private readonly StorageSpecificationToDtoMapper _storageMapper = new();
    [UseMapper] private readonly PsuSpecificationToDtoMapper _psuMapper = new();
    [UseMapper] private readonly CaseSpecificationToDtoMapper _caseMapper = new();
    [UseMapper] private readonly CpuCoolerSpecificationToDtoMapper _cpuCoolerMapper = new();
    [UseMapper] private readonly KeyboardSpecificationToDtoMapper _keyboardMapper = new();
    [UseMapper] private readonly HeadsetSpecificationToDtoMapper _headsetMapper = new();
    [UseMapper] private readonly SpeakerSpecificationToDtoMapper _speakerMapper = new();
    [UseMapper] private readonly WebcamSpecificationToDtoMapper _webcamMapper = new();
    [UseMapper] private readonly CableSpecificationToDtoMapper _cableMapper = new();
    [UseMapper] private readonly MicrophoneSpecificationToDtoMapper _microphoneMapper = new();
    [UseMapper] private readonly MousePadSpecificationToDtoMapper _mousePadMapper = new();

    [MapDerivedType(typeof(MonitorSpecification), typeof(MonitorSpecificationDto))]
    [MapDerivedType(typeof(MouseSpecification), typeof(MouseSpecificationDto))]
    [MapDerivedType(typeof(LaptopSpecification), typeof(LaptopSpecificationDto))]
    [MapDerivedType(typeof(CpuSpecification), typeof(CpuSpecificationDto))]
    [MapDerivedType(typeof(GpuSpecification), typeof(GpuSpecificationDto))]
    [MapDerivedType(typeof(RamSpecification), typeof(RamSpecificationDto))]
    [MapDerivedType(typeof(MotherboardSpecification), typeof(MotherboardSpecificationDto))]
    [MapDerivedType(typeof(StorageSpecification), typeof(StorageSpecificationDto))]
    [MapDerivedType(typeof(PsuSpecification), typeof(PsuSpecificationDto))]
    [MapDerivedType(typeof(CaseSpecification), typeof(CaseSpecificationDto))]
    [MapDerivedType(typeof(CpuCoolerSpecification), typeof(CpuCoolerSpecificationDto))]
    [MapDerivedType(typeof(KeyboardSpecification), typeof(KeyboardSpecificationDto))]
    [MapDerivedType(typeof(HeadsetSpecification), typeof(HeadsetSpecificationDto))]
    [MapDerivedType(typeof(SpeakerSpecification), typeof(SpeakerSpecificationDto))]
    [MapDerivedType(typeof(WebcamSpecification), typeof(WebcamSpecificationDto))]
    [MapDerivedType(typeof(CableSpecification), typeof(CableSpecificationDto))]
    [MapDerivedType(typeof(MicrophoneSpecification), typeof(MicrophoneSpecificationDto))]
    [MapDerivedType(typeof(MousePadSpecification), typeof(MousePadSpecificationDto))]
    public override partial SpecificationBaseDto Map(SpecificationBase source);

    public override void Map(SpecificationBase source, SpecificationBaseDto destination)
    {
        throw new NotSupportedException("Cannot map into an already instantiated polymorphic base destination.");
    }
}
#endregion