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
// Note: Removed Volo.Abp.ObjectMapping usage

namespace Acme.ProductSelling.Products.Specification
{
    public class SpecificationService : ISpecificationService
    {
        private readonly Dictionary<SpecificationType, ISpecificationHandler> _handlers;

        // The constructor becomes large because we inject specific mappers instead of generic factory.
        // This is necessary for type-safe static Mapperly mappings.
        public SpecificationService(
            // --- Repositories ---
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

            // --- Mappers (Generated via Mapperly) ---
            CreateUpdateMonitorSpecToEntityMapper monitorMapper,
            CreateUpdateMouseSpecToEntityMapper mouseMapper,
            CreateUpdateLaptopSpecToEntityMapper laptopMapper,
            CreateUpdateCpuSpecToEntityMapper cpuMapper,
            CreateUpdateGpuSpecToEntityMapper gpuMapper,
            CreateUpdateRamSpecToEntityMapper ramMapper,
            CreateUpdateMotherboardSpecToEntityMapper motherboardMapper,
            CreateUpdateStorageSpecToEntityMapper storageMapper,
            CreateUpdatePsuSpecToEntityMapper psuMapper,
            CreateUpdateCaseSpecToEntityMapper caseMapper,
            CreateUpdateCpuCoolerSpecToEntityMapper cpuCoolerMapper,
            CreateUpdateKeyboardSpecToEntityMapper keyboardMapper,
            CreateUpdateHeadsetSpecToEntityMapper headsetMapper,
            CreateUpdateSpeakerSpecToEntityMapper speakerMapper,
            CreateUpdateWebcamSpecToEntityMapper webcamMapper,
            CreateUpdateCableSpecToEntityMapper cableMapper,
            CreateUpdateSoftwareSpecToEntityMapper softwareMapper,
            CreateUpdateCaseFanSpecToEntityMapper caseFanMapper,
            CreateUpdateChairSpecToEntityMapper chairMapper,
            CreateUpdateDeskSpecToEntityMapper deskMapper,
            CreateUpdateChargerSpecToEntityMapper chargerMapper,
            CreateUpdateConsoleSpecToEntityMapper consoleMapper,
            CreateUpdateHandheldSpecToEntityMapper handheldMapper,
            CreateUpdateHubSpecToEntityMapper hubMapper,
            CreateUpdateMemoryCardSpecToEntityMapper memoryCardMapper,
            CreateUpdateMicrophoneSpecToEntityMapper microphoneMapper,
            CreateUpdateMousePadSpecToEntityMapper mousePadMapper,
            CreateUpdateNetworkHardwareSpecToEntityMapper networkMapper,
            CreateUpdatePowerBankSpecToEntityMapper powerBankMapper
        )
        {
            // Init dictionary
            _handlers = new Dictionary<SpecificationType, ISpecificationHandler>();

            // Helper to clean up instantiation
            void AddHandler<TEntity, TDto, TMapper>(
                SpecificationType type,
                IRepository<TEntity, Guid> repo,
                TMapper mapper,
                Func<CreateUpdateProductDto, TDto> selector,
                Func<TMapper, TDto, TEntity> createMap,
                Action<TMapper, TDto, TEntity> updateMap)
                where TEntity : SpecificationBase, new()
                where TDto : class
            {
                _handlers.Add(type, new SpecificationHandler<TEntity, TDto>(
                    repo,
                    dto => selector(dto), // Get specific DTO part from ProductDto
                    specDto => createMap(mapper, specDto), // Function to create entity
                    (specDto, entity) => updateMap(mapper, specDto, entity) // Function to update entity
                ));
            }

            // --- Register Handlers ---
            AddHandler(SpecificationType.Monitor, monitorRepo, monitorMapper,
                d => d.MonitorSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Mouse, mouseRepo, mouseMapper,
                d => d.MouseSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Laptop, laptopRepo, laptopMapper,
                d => d.LaptopSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.CPU, cpuRepo, cpuMapper,
                d => d.CpuSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.GPU, gpuRepo, gpuMapper,
                d => d.GpuSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.RAM, ramRepo, ramMapper,
                d => d.RamSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Motherboard, motherboardRepo, motherboardMapper,
                d => d.MotherboardSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Storage, storageRepo, storageMapper,
                d => d.StorageSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.PSU, psuRepo, psuMapper,
                d => d.PsuSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Case, caseRepo, caseMapper,
                d => d.CaseSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.CPUCooler, cpuCoolerRepo, cpuCoolerMapper,
                d => d.CpuCoolerSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Keyboard, keyboardRepo, keyboardMapper,
                d => d.KeyboardSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Headset, headsetRepo, headsetMapper,
                d => d.HeadsetSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Speaker, speakerRepo, speakerMapper,
                d => d.SpeakerSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Webcam, webcamRepo, webcamMapper,
                d => d.WebcamSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Cable, cableRepo, cableMapper,
                d => d.CableSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Software, softwareRepo, softwareMapper,
                d => d.SoftwareSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.CaseFan, caseFanRepo, caseFanMapper,
                d => d.CaseFanSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Chair, chairRepo, chairMapper,
                d => d.ChairSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Desk, deskRepo, deskMapper,
                d => d.DeskSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Charger, chargerRepo, chargerMapper,
                d => d.ChargerSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Console, consoleRepo, consoleMapper,
                d => d.ConsoleSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Handheld, handheldRepo, handheldMapper,
                d => d.HandheldSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Hub, hubRepo, hubMapper,
                d => d.HubSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.MemoryCard, memoryCardRepo, memoryCardMapper,
                d => d.MemoryCardSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.Microphone, microphoneRepo, microphoneMapper,
                d => d.MicrophoneSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.MousePad, mousepadRepo, mousePadMapper,
                d => d.MousepadSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.NetworkHardware, networkHardwareRepo, networkMapper,
                d => d.NetworkHardwareSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));

            AddHandler(SpecificationType.PowerBank, powerBankRepo, powerBankMapper,
                d => d.PowerBankSpecification, (m, s) => m.Map(s), (m, s, e) => m.Map(s, e));
        }

        // Methods below delegate to handlers without changes
        public async Task CreateSpecificationAsync(Guid productId, CreateUpdateProductDto dto, SpecificationType specType)
        {
            if (_handlers.TryGetValue(specType, out var handler))
                await handler.CreateAsync(productId, dto);
        }

        public async Task DeleteAllSpecificationsAsync(Guid productId)
        {
            var deleteTasks = _handlers.Values.Select(handler => handler.DeleteIfExistsAsync(productId));
            await Task.WhenAll(deleteTasks);
        }

        public async Task HandleCategoryChangeAsync(Guid productId, SpecificationType currentSpecType, SpecificationType newSpecType)
        {
            if (currentSpecType != newSpecType && _handlers.TryGetValue(currentSpecType, out var oldHandler))
                await oldHandler.DeleteIfExistsAsync(productId);
        }

        public async Task UpdateSpecificationAsync(Guid productId, CreateUpdateProductDto dto, SpecificationType specType)
        {
            if (_handlers.TryGetValue(specType, out var handler))
                await handler.UpdateAsync(productId, dto);
        }
    }
}