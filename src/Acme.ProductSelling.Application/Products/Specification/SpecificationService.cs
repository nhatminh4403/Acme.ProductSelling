using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Acme.ProductSelling.Products
{
    public class SpecificationService : ISpecificationService
    {
        private readonly Dictionary<SpecificationType, ISpecificationHandler> _handlers;

        public SpecificationService(
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
            IObjectMapper objectMapper)
        {
            _handlers = InitializeHandlers(
                monitorSpecRepository, mouseSpecRepository, laptopSpecRepository,
                cpuSpecRepository, gpuSpecRepository, ramSpecRepository,
                motherboardSpecRepository, storageSpecRepository, psuSpecRepository,
                caseSpecRepository, cpuCoolerSpecRepository, keyboardSpecRepository,
                headsetSpecRepository, objectMapper);
        }

        public async Task CreateSpecificationAsync(Product product, CreateUpdateProductDto dto, SpecificationType specType)
        {
            if (_handlers.TryGetValue(specType, out var handler))
            {
                await handler.CreateAsync(product, dto);
            }
        }

        public async Task UpdateSpecificationAsync(Product product, CreateUpdateProductDto dto, SpecificationType specType)
        {
            if (_handlers.TryGetValue(specType, out var handler))
            {
                await handler.UpdateAsync(product, dto);
            }
        }

        public async Task HandleCategoryChangeAsync(Product product, SpecificationType newSpecType)
        {
            var tasksToDelete = _handlers
                .Where(kvp => kvp.Key != newSpecType)
                .Select(kvp => kvp.Value.DeleteIfExistsAsync(product));

            await Task.WhenAll(tasksToDelete);
        }

        public async Task DeleteAllSpecificationsAsync(Product product)
        {
            var deleteTasks = _handlers.Values.Select(handler => handler.DeleteIfExistsAsync(product));
            await Task.WhenAll(deleteTasks);
        }

        private Dictionary<SpecificationType, ISpecificationHandler> InitializeHandlers(
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
            IObjectMapper objectMapper)
        {
            return new Dictionary<SpecificationType, ISpecificationHandler>
            {
                {
                    SpecificationType.Monitor, new SpecificationHandler<MonitorSpecification, CreateUpdateMonitorSpecificationDto>(
                    monitorRepo, objectMapper, p => p.MonitorSpecificationId, (p, id) => p.MonitorSpecificationId = id)
                },
                {
                    SpecificationType.Mouse, new SpecificationHandler<MouseSpecification, CreateUpdateMouseSpecificationDto>(
                    mouseRepo, objectMapper, p => p.MouseSpecificationId, (p, id) => p.MouseSpecificationId = id)
                },
                {
                    SpecificationType.Laptop, new SpecificationHandler<LaptopSpecification, CreateUpdateLaptopSpecificationDto>(
                    laptopRepo, objectMapper, p => p.LaptopSpecificationId, (p, id) => p.LaptopSpecificationId = id)
                },
                {
                    SpecificationType.CPU, new SpecificationHandler<CpuSpecification, CreateUpdateCpuSpecificationDto>(
                    cpuRepo, objectMapper, p => p.CpuSpecificationId, (p, id) => p.CpuSpecificationId = id)
                },
                {
                    SpecificationType.GPU, new SpecificationHandler<GpuSpecification, CreateUpdateGpuSpecificationDto>(
                    gpuRepo, objectMapper, p => p.GpuSpecificationId, (p, id) => p.GpuSpecificationId = id)
                },
                {
                    SpecificationType.RAM, new SpecificationHandler<RamSpecification, CreateUpdateRamSpecificationDto>(
                    ramRepo, objectMapper, p => p.RamSpecificationId, (p, id) => p.RamSpecificationId = id)
                },
                {
                    SpecificationType.Motherboard, new SpecificationHandler<MotherboardSpecification, CreateUpdateMotherboardSpecificationDto>(
                    motherboardRepo, objectMapper, p => p.MotherboardSpecificationId, (p, id) => p.MotherboardSpecificationId = id)
                },
                {
                    SpecificationType.Storage, new SpecificationHandler<StorageSpecification, CreateUpdateStorageSpecificationDto>(
                    storageRepo, objectMapper, p => p.StorageSpecificationId, (p, id) => p.StorageSpecificationId = id)
                },
                {
                    SpecificationType.PSU, new SpecificationHandler<PsuSpecification, CreateUpdatePsuSpecificationDto>(
                    psuRepo, objectMapper, p => p.PsuSpecificationId, (p, id) => p.PsuSpecificationId = id)
                },
                {
                    SpecificationType.Case, new SpecificationHandler<CaseSpecification, CreateUpdateCaseSpecificationDto>(
                    caseRepo, objectMapper, p => p.CaseSpecificationId, (p, id) => p.CaseSpecificationId = id)
                },
                {
                    SpecificationType.CPUCooler, new SpecificationHandler<CpuCoolerSpecification, CreateUpdateCpuCoolerSpecificationDto>(
                    cpuCoolerRepo, objectMapper, p => p.CpuCoolerSpecificationId, (p, id) => p.CpuCoolerSpecificationId = id)
                },
                {
                    SpecificationType.Keyboard, new SpecificationHandler<KeyboardSpecification, CreateUpdateKeyboardSpecificationDto>(
                    keyboardRepo, objectMapper, p => p.KeyboardSpecificationId, (p, id) => p.KeyboardSpecificationId = id)
                },
                {
                    SpecificationType.Headset, new SpecificationHandler<HeadsetSpecification, CreateUpdateHeadsetSpecificationDto>(
                    headsetRepo, objectMapper, p => p.HeadsetSpecificationId, (p, id) => p.HeadsetSpecificationId = id)
                }
            };
        }
    }
}
