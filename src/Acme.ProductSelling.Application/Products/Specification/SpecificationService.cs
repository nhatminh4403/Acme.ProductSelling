using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Specification.Handler;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
namespace Acme.ProductSelling.Products.Specification
{
    public class SpecificationService : ISpecificationService
    {
        private readonly Dictionary<SpecificationType, ISpecificationHandler> _handlers;

        public SpecificationService(
            // Existing repositories
            IRepository<MonitorSpecification, Guid> monitorSpecRepository,
            IRepository<MouseSpecification, Guid> mouseSpecRepository,
            IRepository<LaptopSpecification, Guid> laptopSpecRepository,
            IRepository<CpuSpecification, Guid> cpuSpecRepository,
            IRepository<GpuSpecification, Guid> gpuSpecRepository,
            IRepository<RamSpecification, Guid> ramSpecRepository,
            IRepository<MotherboardSpecification, Guid> motherboardSpecRepository,
            IRepository<StorageSpecification, Guid> storageSpecRepository,
            IRepository<PsuSpecification, Guid> psuSpecRepository,
            IRepository<CaseSpecification, Guid> caseSpecRepository,
            IRepository<CpuCoolerSpecification, Guid> cpuCoolerSpecRepository,
            IRepository<KeyboardSpecification, Guid> keyboardSpecRepository,
            IRepository<HeadsetSpecification, Guid> headsetSpecRepository,
            // New repositories
            IRepository<SpeakerSpecification, Guid> speakerSpecRepository,
            IRepository<WebcamSpecification, Guid> webcamSpecRepository,
            IRepository<CableSpecification, Guid> cableSpecRepository,
            IRepository<SoftwareSpecification, Guid> softwareSpecRepository,
            IRepository<CaseFanSpecification, Guid> caseFanSpecRepository,
            IRepository<ChairSpecification, Guid> chairSpecRepository,
            IRepository<DeskSpecification, Guid> deskSpecRepository,
            IRepository<ChargerSpecification, Guid> chargerSpecRepository,
            IRepository<ConsoleSpecification, Guid> consoleSpecRepository,
            IRepository<HandheldSpecification, Guid> handheldSpecRepository,
            IRepository<HubSpecification, Guid> hubSpecRepository,
            IRepository<MemoryCardSpecification, Guid> memoryCardSpecRepository,
            IRepository<MicrophoneSpecification, Guid> microphoneSpecRepository,
            IRepository<MousePadSpecification, Guid> mousepadSpecRepository,
            IRepository<NetworkHardwareSpecification, Guid> networkHardwareSpecRepository,
            IRepository<PowerBankSpecification, Guid> powerBankSpecRepository,
            IObjectMapper objectMapper)
        {
            _handlers = InitializeHandlers(
                monitorSpecRepository, mouseSpecRepository, laptopSpecRepository,
                cpuSpecRepository, gpuSpecRepository, ramSpecRepository,
                motherboardSpecRepository, storageSpecRepository, psuSpecRepository,
                caseSpecRepository, cpuCoolerSpecRepository, keyboardSpecRepository,
                headsetSpecRepository, speakerSpecRepository, webcamSpecRepository,
                cableSpecRepository, softwareSpecRepository, caseFanSpecRepository,
                chairSpecRepository, deskSpecRepository, chargerSpecRepository,
                consoleSpecRepository, handheldSpecRepository, hubSpecRepository,
                memoryCardSpecRepository, microphoneSpecRepository, mousepadSpecRepository,
                networkHardwareSpecRepository, powerBankSpecRepository, objectMapper);
        }

        public async Task CreateSpecificationAsync(Guid productId, CreateUpdateProductDto dto, SpecificationType specType)
        {
            if (_handlers.TryGetValue(specType, out var handler))
            {
                await handler.CreateAsync(productId, dto);
            }
        }

        public async Task DeleteAllSpecificationsAsync(Guid productId)
        {
            var deleteTasks = _handlers.Values.Select(handler => handler.DeleteIfExistsAsync(productId));
            await Task.WhenAll(deleteTasks);
        }

        public async Task HandleCategoryChangeAsync(Guid productId, SpecificationType currentSpecType, SpecificationType newSpecType)
        {
            if (currentSpecType != newSpecType && _handlers.TryGetValue(currentSpecType, out var oldHandler))
            {
                await oldHandler.DeleteIfExistsAsync(productId);
            }
        }

        public async Task UpdateSpecificationAsync(Guid productId, CreateUpdateProductDto dto, SpecificationType specType)
        {
            if (_handlers.TryGetValue(specType, out var handler))
            {
                await handler.UpdateAsync(productId, dto);
            }
        }

        private Dictionary<SpecificationType, ISpecificationHandler> InitializeHandlers(
            // Existing repositories
            IRepository<MonitorSpecification, Guid> monitorRepo,
            IRepository<MouseSpecification, Guid> mouseRepo,
            IRepository<LaptopSpecification, Guid> laptopRepo,
            IRepository<CpuSpecification, Guid> cpuRepo,
            IRepository<GpuSpecification, Guid> gpuRepo,
            IRepository<RamSpecification, Guid> ramRepo,
            IRepository<MotherboardSpecification, Guid> motherboardRepo,
            IRepository<StorageSpecification, Guid> storageRepo,
            IRepository<PsuSpecification, Guid> psuRepo,
            IRepository<CaseSpecification, Guid> caseRepo,
            IRepository<CpuCoolerSpecification, Guid> cpuCoolerRepo,
            IRepository<KeyboardSpecification, Guid> keyboardRepo,
            IRepository<HeadsetSpecification, Guid> headsetRepo,
            // New repositories
            IRepository<SpeakerSpecification, Guid> speakerRepo,
            IRepository<WebcamSpecification, Guid> webcamRepo,
            IRepository<CableSpecification, Guid> cableRepo,
            IRepository<SoftwareSpecification, Guid> softwareRepo,
            IRepository<CaseFanSpecification, Guid> caseFanRepo,
            IRepository<ChairSpecification, Guid> chairRepo,
            IRepository<DeskSpecification, Guid> deskRepo,
            IRepository<ChargerSpecification, Guid> chargerRepo,
            IRepository<ConsoleSpecification, Guid> consoleRepo,
            IRepository<HandheldSpecification, Guid> handheldRepo,
            IRepository<HubSpecification, Guid> hubRepo,
            IRepository<MemoryCardSpecification, Guid> memoryCardRepo,
            IRepository<MicrophoneSpecification, Guid> microphoneRepo,
            IRepository<MousePadSpecification, Guid> mousepadRepo,
            IRepository<NetworkHardwareSpecification, Guid> networkHardwareRepo,
            IRepository<PowerBankSpecification, Guid> powerBankRepo,
            IObjectMapper objectMapper)
        {
            return new Dictionary<SpecificationType, ISpecificationHandler>
            {
                // Existing handlers
                {
                    SpecificationType.Monitor, new SpecificationHandler<MonitorSpecification, CreateUpdateMonitorSpecificationDto>(
                    monitorRepo, objectMapper, dto => dto.MonitorSpecification)
                },
                {
                    SpecificationType.Mouse, new SpecificationHandler<MouseSpecification, CreateUpdateMouseSpecificationDto>(
                    mouseRepo, objectMapper, dto => dto.MouseSpecification)
                },
                {
                    SpecificationType.Laptop, new SpecificationHandler<LaptopSpecification, CreateUpdateLaptopSpecificationDto>(
                    laptopRepo, objectMapper, dto => dto.LaptopSpecification)
                },
                {
                    SpecificationType.CPU, new SpecificationHandler<CpuSpecification, CreateUpdateCpuSpecificationDto>(
                    cpuRepo, objectMapper, dto => dto.CpuSpecification)
                },
                {
                    SpecificationType.GPU, new SpecificationHandler<GpuSpecification, CreateUpdateGpuSpecificationDto>(
                    gpuRepo, objectMapper, dto => dto.GpuSpecification)
                },
                {
                    SpecificationType.RAM, new SpecificationHandler<RamSpecification, CreateUpdateRamSpecificationDto>(
                    ramRepo, objectMapper, dto => dto.RamSpecification)
                },
                {
                    SpecificationType.Motherboard, new SpecificationHandler<MotherboardSpecification, CreateUpdateMotherboardSpecificationDto>(
                    motherboardRepo, objectMapper, dto => dto.MotherboardSpecification)
                },
                {
                    SpecificationType.Storage, new SpecificationHandler<StorageSpecification, CreateUpdateStorageSpecificationDto>(
                    storageRepo, objectMapper, dto => dto.StorageSpecification)
                },
                {
                    SpecificationType.PSU, new SpecificationHandler<PsuSpecification, CreateUpdatePsuSpecificationDto>(
                    psuRepo, objectMapper, dto => dto.PsuSpecification)
                },
                {
                    SpecificationType.Case, new SpecificationHandler<CaseSpecification, CreateUpdateCaseSpecificationDto>(
                    caseRepo, objectMapper, dto => dto.CaseSpecification)
                },
                {
                    SpecificationType.CPUCooler, new SpecificationHandler<CpuCoolerSpecification, CreateUpdateCpuCoolerSpecificationDto>(
                    cpuCoolerRepo, objectMapper, dto => dto.CpuCoolerSpecification)
                },
                {
                    SpecificationType.Keyboard, new SpecificationHandler<KeyboardSpecification, CreateUpdateKeyboardSpecificationDto>(
                    keyboardRepo, objectMapper, dto => dto.KeyboardSpecification)
                },
                {
                    SpecificationType.Headset, new SpecificationHandler<HeadsetSpecification, CreateUpdateHeadsetSpecificationDto>(
                    headsetRepo, objectMapper, dto => dto.HeadsetSpecification)
                },
                // New handlers
                {
                    SpecificationType.Speaker, new SpecificationHandler<SpeakerSpecification, CreateUpdateSpeakerSpecificationDto>(
                    speakerRepo, objectMapper, dto => dto.SpeakerSpecification)
                },
                {
                    SpecificationType.Webcam, new SpecificationHandler<WebcamSpecification, CreateUpdateWebcamSpecificationDto>(
                    webcamRepo, objectMapper, dto => dto.WebcamSpecification)
                },
                {
                    SpecificationType.Cable, new SpecificationHandler<CableSpecification, CreateUpdateCableSpecificationDto>(
                    cableRepo, objectMapper, dto => dto.CableSpecification)
                },
                {
                    SpecificationType.Software, new SpecificationHandler<SoftwareSpecification, CreateUpdateSoftwareSpecificationDto>(
                    softwareRepo, objectMapper, dto => dto.SoftwareSpecification)
                },
                {
                    SpecificationType.CaseFan, new SpecificationHandler<CaseFanSpecification, CreateUpdateCaseFanSpecificationDto>(
                    caseFanRepo, objectMapper, dto => dto.CaseFanSpecification)
                },
                {
                    SpecificationType.Chair, new SpecificationHandler<ChairSpecification, CreateUpdateChairSpecificationDto>(
                    chairRepo, objectMapper, dto => dto.ChairSpecification)
                },
                {
                    SpecificationType.Desk, new SpecificationHandler<DeskSpecification, CreateUpdateDeskSpecificationDto>(
                    deskRepo, objectMapper, dto => dto.DeskSpecification)
                },
                {
                    SpecificationType.Charger, new SpecificationHandler<ChargerSpecification, CreateUpdateChargerSpecificationDto>(
                    chargerRepo, objectMapper, dto => dto.ChargerSpecification)
                },
                {
                    SpecificationType.Console, new SpecificationHandler<ConsoleSpecification, CreateUpdateConsoleSpecificationDto>(
                    consoleRepo, objectMapper, dto => dto.ConsoleSpecification)
                },
                {
                    SpecificationType.Handheld, new SpecificationHandler<HandheldSpecification, CreateUpdateHandheldSpecificationDto>(
                    handheldRepo, objectMapper, dto => dto.HandheldSpecification)
                },
                {
                    SpecificationType.Hub, new SpecificationHandler<HubSpecification, CreateUpdateHubSpecificationDto>(
                    hubRepo, objectMapper, dto => dto.HubSpecification)
                },
                {
                    SpecificationType.MemoryCard, new SpecificationHandler<MemoryCardSpecification, CreateUpdateMemoryCardSpecificationDto>(
                    memoryCardRepo, objectMapper, dto => dto.MemoryCardSpecification)
                },
                {
                    SpecificationType.Microphone, new SpecificationHandler<MicrophoneSpecification, CreateUpdateMicrophoneSpecificationDto>(
                    microphoneRepo, objectMapper, dto => dto.MicrophoneSpecification)
                },
                {
                    SpecificationType.MousePad, new SpecificationHandler<MousePadSpecification, CreateUpdateMousePadSpecificationDto>(
                    mousepadRepo, objectMapper, dto => dto.MousepadSpecification)
                },
                {
                    SpecificationType.NetworkHardware, new SpecificationHandler<NetworkHardwareSpecification, CreateUpdateNetworkHardwareSpecificationDto>(
                    networkHardwareRepo, objectMapper, dto => dto.NetworkHardwareSpecification)
                },
                {
                    SpecificationType.PowerBank, new SpecificationHandler<PowerBankSpecification, CreateUpdatePowerBankSpecificationDto>(
                    powerBankRepo, objectMapper, dto => dto.PowerBankSpecification)
                }
            };
        }
    }
}