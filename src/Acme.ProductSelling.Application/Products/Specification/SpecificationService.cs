using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Specification.Handler;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
// Note: Removed Volo.Abp.ObjectMapping usage

namespace Acme.ProductSelling.Products.Specification
{
    public class SpecificationService : ISpecificationService
    {
        //private readonly Dictionary<SpecificationType, ISpecificationHandler> _handlers;
        private readonly IRepository<SpecificationBase, Guid> _specRepository;
        private readonly CreateUpdateMonitorSpecToEntityMapper _monitorMapper;
        private readonly CreateUpdateMouseSpecToEntityMapper _mouseMapper;
        private readonly CreateUpdateLaptopSpecToEntityMapper _laptopMapper;
        private readonly CreateUpdateCpuSpecToEntityMapper _cpuMapper;
        private readonly CreateUpdateGpuSpecToEntityMapper _gpuMapper;
        private readonly CreateUpdateRamSpecToEntityMapper _ramMapper;
        private readonly CreateUpdateMotherboardSpecToEntityMapper _motherboardMapper;
        private readonly CreateUpdateStorageSpecToEntityMapper _storageMapper;
        private readonly CreateUpdatePsuSpecToEntityMapper _psuMapper;
        private readonly CreateUpdateCaseSpecToEntityMapper _caseMapper;
        private readonly CreateUpdateCpuCoolerSpecToEntityMapper _cpuCoolerMapper;
        private readonly CreateUpdateKeyboardSpecToEntityMapper _keyboardMapper;
        private readonly CreateUpdateHeadsetSpecToEntityMapper _headsetMapper;
        private readonly CreateUpdateSpeakerSpecToEntityMapper _speakerMapper;
        private readonly CreateUpdateWebcamSpecToEntityMapper _webcamMapper;
        private readonly CreateUpdateCableSpecToEntityMapper _cableMapper;
        private readonly CreateUpdateSoftwareSpecToEntityMapper _softwareMapper;
        private readonly CreateUpdateCaseFanSpecToEntityMapper _caseFanMapper;
        private readonly CreateUpdateChairSpecToEntityMapper _chairMapper;
        private readonly CreateUpdateDeskSpecToEntityMapper _deskMapper;
        private readonly CreateUpdateChargerSpecToEntityMapper _chargerMapper;
        private readonly CreateUpdateConsoleSpecToEntityMapper _consoleMapper;
        private readonly CreateUpdateHandheldSpecToEntityMapper _handheldMapper;
        private readonly CreateUpdateHubSpecToEntityMapper _hubMapper;
        private readonly CreateUpdateMemoryCardSpecToEntityMapper _memoryCardMapper;
        private readonly CreateUpdateMicrophoneSpecToEntityMapper _microphoneMapper;
        private readonly CreateUpdateMousePadSpecToEntityMapper _mousePadMapper;
        private readonly CreateUpdateNetworkHardwareSpecToEntityMapper _networkMapper;
        private readonly CreateUpdatePowerBankSpecToEntityMapper _powerBankMapper;
        private readonly IServiceProvider _serviceProvider;
        private record HandlerRegistry(Func<IServiceProvider, CreateUpdateProductDto, SpecificationBase> BuildAsync,
            Action<IServiceProvider, SpecificationBase, CreateUpdateProductDto> UpdateAsync);
        private readonly Dictionary<SpecificationType, HandlerRegistry> _registry;

        public SpecificationService(
            Dictionary<SpecificationType, ISpecificationHandler> handlers,
            IRepository<SpecificationBase, Guid> specRepository,
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
            CreateUpdatePowerBankSpecToEntityMapper powerBankMapper,
            IServiceProvider serviceProvider)
        {
            _registry = new Dictionary<SpecificationType, HandlerRegistry>();

            _specRepository = specRepository;
            _monitorMapper = monitorMapper;
            _mouseMapper = mouseMapper;
            _laptopMapper = laptopMapper;
            _cpuMapper = cpuMapper;
            _gpuMapper = gpuMapper;
            _ramMapper = ramMapper;
            _motherboardMapper = motherboardMapper;
            _storageMapper = storageMapper;
            _psuMapper = psuMapper;
            _caseMapper = caseMapper;
            _cpuCoolerMapper = cpuCoolerMapper;
            _keyboardMapper = keyboardMapper;
            _headsetMapper = headsetMapper;
            _speakerMapper = speakerMapper;
            _webcamMapper = webcamMapper;
            _cableMapper = cableMapper;
            _softwareMapper = softwareMapper;
            _caseFanMapper = caseFanMapper;
            _chairMapper = chairMapper;
            _deskMapper = deskMapper;
            _chargerMapper = chargerMapper;
            _consoleMapper = consoleMapper;
            _handheldMapper = handheldMapper;
            _hubMapper = hubMapper;
            _memoryCardMapper = memoryCardMapper;
            _microphoneMapper = microphoneMapper;
            _mousePadMapper = mousePadMapper;
            _networkMapper = networkMapper;
            _powerBankMapper = powerBankMapper;
            _serviceProvider = serviceProvider;


        }

        // Methods below delegate to handlers without changes
        public async Task CreateSpecificationAsync(Guid productId, CreateUpdateProductDto dto, SpecificationType specType)
        {
            var spec = BuildNewSpec(dto, specType); // switch → mapper.Map(dto.XxxSpec)
            spec.ProductId = productId;
            await _specRepository.InsertAsync(spec, autoSave: true);
        }

        public async Task DeleteAllSpecificationsAsync(Guid productId)
        {
            var existing = await _specRepository.FindAsync(s => s.ProductId == productId);
            if (existing != null)
                await _specRepository.DeleteAsync(existing, autoSave: true);

        }

        public async Task HandleCategoryChangeAsync(Guid productId, SpecificationType currentSpecType, SpecificationType newSpecType)
        {
            if (currentSpecType == newSpecType) return;

            var existing = await _specRepository.FindAsync(s => s.ProductId == productId);
            if (existing != null)
                await _specRepository.DeleteAsync(existing, autoSave: true);
        }

        public async Task UpdateSpecificationAsync(Guid productId, CreateUpdateProductDto dto, SpecificationType specType)
        {
            var existing = await _specRepository.FindAsync(s => s.ProductId == productId);

            if (existing == null)
            {
                await CreateSpecificationAsync(productId, dto, specType);
                return;
            }

            // Type matches — update in place
            ApplyUpdate(existing, dto);
            await _specRepository.UpdateAsync(existing, autoSave: true);
        }
        private SpecificationBase BuildNewSpec(CreateUpdateProductDto dto, SpecificationType specType)
        {
            return specType switch
            {
                SpecificationType.Cable => _cableMapper.Map(dto.CableSpecification),
                SpecificationType.CaseFan => _caseFanMapper.Map(dto.CaseFanSpecification),
                SpecificationType.Case => _caseMapper.Map(dto.CaseSpecification),
                SpecificationType.Chair => _chairMapper.Map(dto.ChairSpecification),
                SpecificationType.Charger => _chargerMapper.Map(dto.ChargerSpecification),
                SpecificationType.Console => _consoleMapper.Map(dto.ConsoleSpecification),
                SpecificationType.CPUCooler => _cpuCoolerMapper.Map(dto.CpuCoolerSpecification),
                SpecificationType.CPU => _cpuMapper.Map(dto.CpuSpecification),
                SpecificationType.Desk => _deskMapper.Map(dto.DeskSpecification),
                SpecificationType.GPU => _gpuMapper.Map(dto.GpuSpecification),
                SpecificationType.Handheld => _handheldMapper.Map(dto.HandheldSpecification),
                SpecificationType.Headset => _headsetMapper.Map(dto.HeadsetSpecification),
                SpecificationType.Hub => _hubMapper.Map(dto.HubSpecification),
                SpecificationType.Keyboard => _keyboardMapper.Map(dto.KeyboardSpecification),
                SpecificationType.Laptop => _laptopMapper.Map(dto.LaptopSpecification),
                SpecificationType.MemoryCard => _memoryCardMapper.Map(dto.MemoryCardSpecification),
                SpecificationType.Microphone => _microphoneMapper.Map(dto.MicrophoneSpecification),
                SpecificationType.Monitor => _monitorMapper.Map(dto.MonitorSpecification),
                SpecificationType.Motherboard => _motherboardMapper.Map(dto.MotherboardSpecification),
                SpecificationType.MousePad => _mousePadMapper.Map(dto.MousepadSpecification),
                SpecificationType.Mouse => _mouseMapper.Map(dto.MouseSpecification),
                SpecificationType.NetworkHardware => _networkMapper.Map(dto.NetworkHardwareSpecification),
                SpecificationType.PowerBank => _powerBankMapper.Map(dto.PowerBankSpecification),
                SpecificationType.PSU => _psuMapper.Map(dto.PsuSpecification),
                SpecificationType.RAM => _ramMapper.Map(dto.RamSpecification),
                SpecificationType.Software => _softwareMapper.Map(dto.SoftwareSpecification),
                SpecificationType.Speaker => _speakerMapper.Map(dto.SpeakerSpecification),
                SpecificationType.Storage => _storageMapper.Map(dto.StorageSpecification),
                SpecificationType.Webcam => _webcamMapper.Map(dto.WebcamSpecification),
                _ => throw new ArgumentOutOfRangeException(nameof(specType))
            };
        }
        private void ApplyUpdate(SpecificationBase existing, CreateUpdateProductDto dto)
        {
            switch (existing)
            {
                case CableSpecification e:
                    _cableMapper.Map(dto.CableSpecification, e);
                    break;

                case CaseFanSpecification e:
                    _caseFanMapper.Map(dto.CaseFanSpecification, e);
                    break;

                case CaseSpecification e:
                    _caseMapper.Map(dto.CaseSpecification, e);
                    break;

                case ChairSpecification e:
                    _chairMapper.Map(dto.ChairSpecification, e);
                    break;

                case ChargerSpecification e:
                    _chargerMapper.Map(dto.ChargerSpecification, e);
                    break;

                case ConsoleSpecification e:
                    _consoleMapper.Map(dto.ConsoleSpecification, e);
                    break;

                case CpuCoolerSpecification e:
                    _cpuCoolerMapper.Map(dto.CpuCoolerSpecification, e);
                    break;

                case CpuSpecification e:
                    _cpuMapper.Map(dto.CpuSpecification, e);
                    break;

                case DeskSpecification e:
                    _deskMapper.Map(dto.DeskSpecification, e);
                    break;

                case GpuSpecification e:
                    _gpuMapper.Map(dto.GpuSpecification, e);
                    break;

                case HandheldSpecification e:
                    _handheldMapper.Map(dto.HandheldSpecification, e);
                    break;

                case HeadsetSpecification e:
                    _headsetMapper.Map(dto.HeadsetSpecification, e);
                    break;

                case HubSpecification e:
                    _hubMapper.Map(dto.HubSpecification, e);
                    break;

                case KeyboardSpecification e:
                    _keyboardMapper.Map(dto.KeyboardSpecification, e);
                    break;

                case LaptopSpecification e:
                    _laptopMapper.Map(dto.LaptopSpecification, e);
                    break;

                case MemoryCardSpecification e:
                    _memoryCardMapper.Map(dto.MemoryCardSpecification, e);
                    break;

                case MicrophoneSpecification e:
                    _microphoneMapper.Map(dto.MicrophoneSpecification, e);
                    break;

                case MonitorSpecification e:
                    _monitorMapper.Map(dto.MonitorSpecification, e);
                    break;

                case MotherboardSpecification e:
                    _motherboardMapper.Map(dto.MotherboardSpecification, e);
                    break;

                case MousePadSpecification e:
                    _mousePadMapper.Map(dto.MousepadSpecification, e);
                    break;

                case MouseSpecification e:
                    _mouseMapper.Map(dto.MouseSpecification, e);
                    break;

                case NetworkHardwareSpecification e:
                    _networkMapper.Map(dto.NetworkHardwareSpecification, e);
                    break;

                case PowerBankSpecification e:
                    _powerBankMapper.Map(dto.PowerBankSpecification, e);
                    break;

                case PsuSpecification e:
                    _psuMapper.Map(dto.PsuSpecification, e);
                    break;

                case RamSpecification e:
                    _ramMapper.Map(dto.RamSpecification, e);
                    break;

                case SoftwareSpecification e:
                    _softwareMapper.Map(dto.SoftwareSpecification, e);
                    break;

                case SpeakerSpecification e:
                    _speakerMapper.Map(dto.SpeakerSpecification, e);
                    break;

                case StorageSpecification e:
                    _storageMapper.Map(dto.StorageSpecification, e);
                    break;

                case WebcamSpecification e:
                    _webcamMapper.Map(dto.WebcamSpecification, e);
                    break;
            }
        }
    }
}