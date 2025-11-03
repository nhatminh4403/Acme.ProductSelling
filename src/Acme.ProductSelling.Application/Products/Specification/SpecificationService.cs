using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Models;
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
                    psuRepo, objectMapper,dto => dto.PsuSpecification)
                },
                {
                    SpecificationType.Case, new SpecificationHandler<CaseSpecification, CreateUpdateCaseSpecificationDto>(
                    caseRepo, objectMapper,dto => dto.CaseSpecification)
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
                    headsetRepo, objectMapper,dto => dto.HeadsetSpecification)
                }
            };
        }
    }
}
