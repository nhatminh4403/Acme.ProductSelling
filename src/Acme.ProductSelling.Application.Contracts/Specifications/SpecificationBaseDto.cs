using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Specifications
{
    [JsonDerivedType(typeof(MonitorSpecificationDto), "Monitor")]
    [JsonDerivedType(typeof(MouseSpecificationDto), "Mouse")]
    [JsonDerivedType(typeof(LaptopSpecificationDto), "Laptop")]
    [JsonDerivedType(typeof(CpuSpecificationDto), "CPU")]
    [JsonDerivedType(typeof(GpuSpecificationDto), "GPU")]
    [JsonDerivedType(typeof(RamSpecificationDto), "RAM")]
    [JsonDerivedType(typeof(MotherboardSpecificationDto), "Motherboard")]
    [JsonDerivedType(typeof(StorageSpecificationDto), "Storage")]
    [JsonDerivedType(typeof(PsuSpecificationDto), "PSU")]
    [JsonDerivedType(typeof(CaseSpecificationDto), "Case")]
    [JsonDerivedType(typeof(CpuCoolerSpecificationDto), "CPUCooler")]
    [JsonDerivedType(typeof(KeyboardSpecificationDto), "Keyboard")]
    [JsonDerivedType(typeof(HeadsetSpecificationDto), "Headset")]
    [JsonDerivedType(typeof(SpeakerSpecificationDto), "Speaker")]
    [JsonDerivedType(typeof(WebcamSpecificationDto), "Webcam")]
    [JsonDerivedType(typeof(CableSpecificationDto), "Cable")]
    [JsonDerivedType(typeof(SoftwareSpecificationDto), "Software")]
    [JsonDerivedType(typeof(CaseFanSpecificationDto), "CaseFan")]
    [JsonDerivedType(typeof(ChairSpecificationDto), "Chair")]
    [JsonDerivedType(typeof(DeskSpecificationDto), "Desk")]
    [JsonDerivedType(typeof(ChargerSpecificationDto), "Charger")]
    [JsonDerivedType(typeof(ConsoleSpecificationDto), "Console")]
    [JsonDerivedType(typeof(HandheldSpecificationDto), "Handheld")]
    [JsonDerivedType(typeof(HubSpecificationDto), "Hub")]
    [JsonDerivedType(typeof(MemoryCardSpecificationDto), "MemoryCard")]
    [JsonDerivedType(typeof(MicrophoneSpecificationDto), "Microphone")]
    [JsonDerivedType(typeof(MousePadSpecificationDto), "MousePad")]
    [JsonDerivedType(typeof(NetworkHardwareSpecificationDto), "NetworkHardware")]
    [JsonDerivedType(typeof(PowerBankSpecificationDto), "PowerBank")]
    public abstract class SpecificationBaseDto : EntityDto<Guid>
    {
        public Guid ProductId { get; set; }
    }
}
