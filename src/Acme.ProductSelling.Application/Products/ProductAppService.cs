using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
namespace Acme.ProductSelling.Products
{
    public class ProductAppService : CrudAppService<
    Product,
    ProductDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateProductDto>, IProductAppService
    {
        private readonly IRepository<Category, Guid> _categoryRepository;
        private readonly IRepository<MonitorSpecification, Guid> _monitorSpecRepository;
        private readonly IRepository<MouseSpecification, Guid> _mouseSpecRepository;
        private readonly IRepository<LaptopSpecification, Guid> _laptopSpecRepository;
        private readonly IRepository<CpuSpecification, Guid> _cpuSpecRepository;
        private readonly IRepository<GpuSpecification, Guid> _gpuSpecRepository;
        private readonly IRepository<RamSpecification, Guid> _ramSpecRepository;
        private readonly IRepository<MotherboardSpecification, Guid> _motherboardSpecRepository;
        private readonly IRepository<StorageSpecification, Guid> _storageSpecRepository;
        private readonly IRepository<PsuSpecification, Guid> _psuSpecRepository;
        private readonly IRepository<CaseSpecification, Guid> _caseSpecRepository;
        private readonly IRepository<CpuCoolerSpecification, Guid> _cpuCoolerSpecRepository;
        private readonly IRepository<KeyboardSpecification, Guid> _keyboardSpecRepository;
        private readonly IRepository<HeadsetSpecification, Guid> _headsetSpecRepository;
        public ProductAppService(
            IRepository<Product, Guid> productRepository,
            IRepository<Category, Guid> categoryRepository,
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
            IRepository<HeadsetSpecification, Guid> headsetSpecRepository)
            : base(productRepository)
        {
            _categoryRepository = categoryRepository;
            GetPolicyName = null;
            CreatePolicyName = ProductSellingPermissions.Products.Create;
            UpdatePolicyName = ProductSellingPermissions.Products.Edit;
            DeletePolicyName = ProductSellingPermissions.Products.Delete;
            _monitorSpecRepository = monitorSpecRepository;
            _mouseSpecRepository = mouseSpecRepository;
            _laptopSpecRepository = laptopSpecRepository;
            _cpuSpecRepository = cpuSpecRepository;
            _gpuSpecRepository = gpuSpecRepository;
            _ramSpecRepository = ramSpecRepository;
            _motherboardSpecRepository = motherboardSpecRepository;
            _storageSpecRepository = storageSpecRepository;
            _psuSpecRepository = psuSpecRepository;
            _caseSpecRepository = caseSpecRepository;
            _cpuCoolerSpecRepository = cpuCoolerSpecRepository;
            _keyboardSpecRepository = keyboardSpecRepository;
            _headsetSpecRepository = headsetSpecRepository;
        }
        public override async Task<ProductDto> GetAsync(Guid id)
        {
            var query = await Repository.GetQueryableAsync();
            var product = await query.Include(p => p.Category)
                                  .Include(p => p.MonitorSpecification)
                                  .Include(p => p.MouseSpecification)
                                  .Include(p => p.LaptopSpecification)
                                  .Include(p => p.CpuSpecification)
                                  .Include(p => p.GpuSpecification)
                                  .Include(p => p.RamSpecification)
                                  .Include(p => p.MotherboardSpecification)
                                  .Include(p => p.StorageSpecification)
                                  .Include(p => p.PsuSpecification)
                                  .Include(p => p.CaseSpecification)
                                  .Include(p => p.CpuCoolerSpecification)
                                  .Include(p => p.KeyboardSpecification)
                                  .Include(p => p.HeadsetSpecification)
                                  .Include(p => p.Manufacturer)
                                  .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Product), id);
            }
            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        public virtual async Task<ProductDto> GetProductBySlug(string slug)
        {
            var query = await Repository.GetQueryableAsync();
            var product = await query.AsNoTracking().Include(p => p.Category)
                                  .Include(p => p.MonitorSpecification)
                                  .Include(p => p.MouseSpecification)
                                  .Include(p => p.LaptopSpecification)
                                  .Include(p => p.CpuSpecification)
                                  .Include(p => p.GpuSpecification)
                                  .Include(p => p.RamSpecification)
                                  .Include(p => p.MotherboardSpecification)
                                  .Include(p => p.StorageSpecification)
                                  .Include(p => p.PsuSpecification)
                                  .Include(p => p.CaseSpecification)
                                  .Include(p => p.CpuCoolerSpecification)
                                  .Include(p => p.KeyboardSpecification)
                                  .Include(p => p.HeadsetSpecification)
                                  .Include(p => p.Manufacturer)
                                  .FirstOrDefaultAsync(p => p.UrlSlug.ToLower() == slug.ToLower());
            if (product == null)
            {
                throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Product), slug);
            }
            return ObjectMapper.Map<Product, ProductDto>(product);
        }
        public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {
            await CheckCreatePolicyAsync();

            var category = await _categoryRepository.GetAsync(input.CategoryId);
            var productEntity = ObjectMapper.Map<CreateUpdateProductDto, Product>(input);
            typeof(Product).GetProperty(nameof(Product.Id))?.SetValue(productEntity.Id, GuidGenerator.Create());
            async Task<Guid?> CreateSpecificationAsync<TSpecDto, TSpecEntity>(
                TSpecDto specDto,
                IRepository<TSpecEntity, Guid> specRepository)
                where TSpecEntity : class, Volo.Abp.Domain.Entities.IEntity<Guid>, new()
                where TSpecDto : class
            {
                if (specDto == null) return null;

                var specEntity = ObjectMapper.Map<TSpecDto, TSpecEntity>(specDto);
                typeof(Product).GetProperty(nameof(Product.Id))?.SetValue(specEntity.Id, GuidGenerator.Create()); 
                await specRepository.InsertAsync(specEntity, autoSave: false);
                return specEntity.Id;
            }

            switch (category.SpecificationType)
            {
                case SpecificationType.Monitor:
                    productEntity.MonitorSpecificationId = await CreateSpecificationAsync(input.MonitorSpecification, _monitorSpecRepository);
                    break;
                case SpecificationType.Mouse:
                    productEntity.MouseSpecificationId = await CreateSpecificationAsync(input.MouseSpecification, _mouseSpecRepository);
                    break;
                case SpecificationType.Laptop:
                    productEntity.LaptopSpecificationId = await CreateSpecificationAsync(input.LaptopSpecification, _laptopSpecRepository);
                    break;
                case SpecificationType.CPU:
                    productEntity.CpuSpecificationId = await CreateSpecificationAsync(input.CpuSpecification, _cpuSpecRepository);
                    break;
                case SpecificationType.GPU:
                    productEntity.GpuSpecificationId = await CreateSpecificationAsync(input.GpuSpecification, _gpuSpecRepository);
                    break;
                case SpecificationType.RAM:
                    productEntity.RamSpecificationId = await CreateSpecificationAsync(input.RamSpecification, _ramSpecRepository);
                    break;
                case SpecificationType.Motherboard:
                    productEntity.MotherboardSpecificationId = await CreateSpecificationAsync(input.MotherboardSpecification, _motherboardSpecRepository);
                    break;
                case SpecificationType.Storage:
                    productEntity.StorageSpecificationId = await CreateSpecificationAsync(input.StorageSpecification, _storageSpecRepository);
                    break;
                case SpecificationType.PSU:
                    productEntity.PsuSpecificationId = await CreateSpecificationAsync(input.PsuSpecification, _psuSpecRepository);
                    break;
                case SpecificationType.Case:
                    productEntity.CaseSpecificationId = await CreateSpecificationAsync(input.CaseSpecification, _caseSpecRepository);
                    break;
                case SpecificationType.CPUCooler:
                    productEntity.CpuCoolerSpecificationId = await CreateSpecificationAsync(input.CpuCoolerSpecification, _cpuCoolerSpecRepository);
                    break;
                case SpecificationType.Keyboard:
                    productEntity.KeyboardSpecificationId = await CreateSpecificationAsync(input.KeyboardSpecification, _keyboardSpecRepository);
                    break;
                case SpecificationType.Headset:
                    productEntity.HeadsetSpecificationId = await CreateSpecificationAsync(input.HeadsetSpecification, _headsetSpecRepository);
                    break;
                default: break;
            }
            await Repository.InsertAsync(productEntity, autoSave: true);
            return await GetAsync(productEntity.Id);
            
        }
        public override async Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto input)
        {
            await CheckUpdatePolicyAsync();
            var product = await Repository.GetAsync(id);
            var oldCategoryId = product.CategoryId;
            var newCategory = await _categoryRepository.GetAsync(input.CategoryId);
            if (oldCategoryId != newCategory.Id)
            {
                await DeleteOldSpecificationAsync(product, newCategory.SpecificationType);
            }
            ObjectMapper.Map(input, product);
            product.CategoryId = newCategory.Id;
            switch (newCategory.SpecificationType)
            {
                case SpecificationType.Monitor:
                    await HandleSpecificationUpdateAsync(product.MonitorSpecificationId, input.MonitorSpecification, _monitorSpecRepository, (specId) => product.MonitorSpecificationId = specId);
                    break;
                case SpecificationType.Mouse:
                    await HandleSpecificationUpdateAsync(product.MouseSpecificationId, input.MouseSpecification, _mouseSpecRepository, (specId) => product.MouseSpecificationId = specId);
                    break;
                case SpecificationType.Laptop:
                    await HandleSpecificationUpdateAsync(product.LaptopSpecificationId, input.LaptopSpecification, _laptopSpecRepository, (specId) => product.LaptopSpecificationId = specId);
                    break;
                case SpecificationType.CPU:
                    await HandleSpecificationUpdateAsync(product.CpuSpecificationId, input.CpuSpecification, _cpuSpecRepository, (specId) => product.CpuSpecificationId = specId);
                    break;
                case SpecificationType.GPU:
                    await HandleSpecificationUpdateAsync(product.GpuSpecificationId, input.GpuSpecification, _gpuSpecRepository, (specId) => product.GpuSpecificationId = specId);
                    break;
                case SpecificationType.RAM:
                    await HandleSpecificationUpdateAsync(product.RamSpecificationId, input.RamSpecification, _ramSpecRepository, (specId) => product.RamSpecificationId = specId);
                    break;
                case SpecificationType.Motherboard:
                    await HandleSpecificationUpdateAsync(product.MotherboardSpecificationId, input.MotherboardSpecification, _motherboardSpecRepository, (specId) => product.MotherboardSpecificationId = specId);
                    break;
                case SpecificationType.Storage:
                    await HandleSpecificationUpdateAsync(product.StorageSpecificationId, input.StorageSpecification, _storageSpecRepository, (specId) => product.StorageSpecificationId = specId);
                    break;
                case SpecificationType.PSU:
                    await HandleSpecificationUpdateAsync(product.PsuSpecificationId, input.PsuSpecification, _psuSpecRepository, (specId) => product.PsuSpecificationId = specId);
                    break;
                case SpecificationType.Case:
                    await HandleSpecificationUpdateAsync(product.CaseSpecificationId, input.CaseSpecification, _caseSpecRepository, (specId) => product.CaseSpecificationId = specId);
                    break;
                case SpecificationType.CPUCooler:
                    await HandleSpecificationUpdateAsync(product.CpuCoolerSpecificationId, input.CpuCoolerSpecification, _cpuCoolerSpecRepository, (specId) => product.CpuCoolerSpecificationId = specId);
                    break;
                case SpecificationType.Keyboard:
                    await HandleSpecificationUpdateAsync(product.KeyboardSpecificationId, input.KeyboardSpecification, _keyboardSpecRepository, (specId) => product.KeyboardSpecificationId = specId);
                    break;
                case SpecificationType.Headset:
                    await HandleSpecificationUpdateAsync(product.HeadsetSpecificationId, input.HeadsetSpecification, _headsetSpecRepository, (specId) => product.HeadsetSpecificationId = specId);
                    break;
                default:
                    await DeleteOldSpecificationAsync(product, SpecificationType.None);
                    break;
            }
            await Repository.UpdateAsync(product, autoSave: true);
            return await GetAsync(product.Id);
        }
        private async Task HandleSpecificationUpdateAsync<TSpecEntity, TSpecDto>(
            Guid? currentSpecId,
            TSpecDto inputDto,
            IRepository<TSpecEntity, Guid> specRepository,  
            Action<Guid?> setProductSpecIdAction)
            where TSpecEntity : class, Volo.Abp.Domain.Entities.IEntity<Guid>
            where TSpecDto : class
        {
            if (inputDto != null)
            {
                if (currentSpecId.HasValue)
                {
                    var existingSpec = await specRepository.GetAsync(currentSpecId.Value);
                    ObjectMapper.Map(inputDto, existingSpec);
                    await specRepository.UpdateAsync(existingSpec, autoSave: true);
                }
                else
                {
                    var newSpec = ObjectMapper.Map<TSpecDto, TSpecEntity>(inputDto);
                    typeof(Product).GetProperty(nameof(Product.Id))?.SetValue(newSpec, GuidGenerator.Create());
                    await specRepository.InsertAsync(newSpec, autoSave: true);
                    setProductSpecIdAction(newSpec.Id);
                }
            }
            else if (currentSpecId.HasValue)
            {
                await specRepository.DeleteAsync(currentSpecId.Value, autoSave: true);
                setProductSpecIdAction(null);
            }
        }
        private async Task DeleteOldSpecificationAsync(Product product, SpecificationType? newSpecType)
        {
            if (product.MonitorSpecificationId.HasValue && newSpecType != SpecificationType.Monitor)
            {
                await _monitorSpecRepository.DeleteAsync(product.MonitorSpecificationId.Value, autoSave: true);
                product.MonitorSpecificationId = null;
            }
            if (product.MouseSpecificationId.HasValue && newSpecType != SpecificationType.Mouse)
            {
                await _mouseSpecRepository.DeleteAsync(product.MouseSpecificationId.Value, autoSave: true);
                product.MouseSpecificationId = null;
            }
            if (product.LaptopSpecificationId.HasValue && newSpecType != SpecificationType.Laptop)
            {
                await _laptopSpecRepository.DeleteAsync(product.LaptopSpecificationId.Value, autoSave: true);
                product.LaptopSpecificationId = null;
            }
            if (product.CpuSpecificationId.HasValue && newSpecType != SpecificationType.CPU)
            {
                await _cpuSpecRepository.DeleteAsync(product.CpuSpecificationId.Value, autoSave: true);
                product.CpuSpecificationId = null;
            }
            if (product.GpuSpecificationId.HasValue && newSpecType != SpecificationType.GPU)
            {
                await _gpuSpecRepository.DeleteAsync(product.GpuSpecificationId.Value, autoSave: true);
                product.GpuSpecificationId = null;
            }
            if (product.RamSpecificationId.HasValue && newSpecType != SpecificationType.RAM)
            {
                await _ramSpecRepository.DeleteAsync(product.RamSpecificationId.Value, autoSave: true);
                product.RamSpecificationId = null;
            }
            if (product.MotherboardSpecificationId.HasValue && newSpecType != SpecificationType.Motherboard)
            {
                await _motherboardSpecRepository.DeleteAsync(product.MotherboardSpecificationId.Value, autoSave: true);
                product.MotherboardSpecificationId = null;
            }
            if (product.StorageSpecificationId.HasValue && newSpecType != SpecificationType.Storage)
            {
                await _storageSpecRepository.DeleteAsync(product.StorageSpecificationId.Value, autoSave: true);
                product.StorageSpecificationId = null;
            }
            if (product.PsuSpecificationId.HasValue && newSpecType != SpecificationType.PSU)
            {
                await _psuSpecRepository.DeleteAsync(product.PsuSpecificationId.Value, autoSave: true);
                product.PsuSpecificationId = null;
            }
            if (product.CaseSpecificationId.HasValue && newSpecType != SpecificationType.Case)
            {
                await _caseSpecRepository.DeleteAsync(product.CaseSpecificationId.Value, autoSave: true);
                product.CaseSpecificationId = null;
            }
            if (product.CpuCoolerSpecificationId.HasValue && newSpecType != SpecificationType.CPUCooler)
            {
                await _cpuCoolerSpecRepository.DeleteAsync(product.CpuCoolerSpecificationId.Value, autoSave: true);
                product.CpuCoolerSpecificationId = null;
            }
            if (product.KeyboardSpecificationId.HasValue && newSpecType != SpecificationType.Keyboard)
            {
                await _keyboardSpecRepository.DeleteAsync(product.KeyboardSpecificationId.Value, autoSave: true);
                product.KeyboardSpecificationId = null;
            }
            if (product.HeadsetSpecificationId.HasValue && newSpecType != SpecificationType.Headset)
            {
                await _headsetSpecRepository.DeleteAsync(product.HeadsetSpecificationId.Value, autoSave: true);
                product.HeadsetSpecificationId = null;
            }
        }
        public override async Task DeleteAsync(Guid id)
        {
            await CheckDeletePolicyAsync();

            var product = await Repository.FindAsync(id);
            if (product == null) return;
            await DeleteOldSpecificationAsync(product, SpecificationType.None);
            await Repository.DeleteAsync(id, autoSave: true);
        }
        protected override async Task<ProductDto> MapToGetOutputDtoAsync(Product entity)
        {
            var dto = ObjectMapper.Map<Product, ProductDto>(entity);
            var category = await _categoryRepository.GetAsync(entity.CategoryId);
            dto.CategoryName = category.Name;
            return dto;
        }
        public virtual async Task<PagedResultDto<ProductDto>> GetListByCategoryAsync(GetProductsByCategoryInput input)
        {
            var queryable = await Repository.GetQueryableAsync();
            queryable = queryable.Include(p => p.Category);
            queryable = queryable.Where(p => p.CategoryId == input.CategoryId);
            var totalCount = await AsyncExecuter.CountAsync(queryable);
            queryable = queryable
                .OrderBy(input.Sorting ?? nameof(Product.ProductName))
                .PageBy(input);
            var products = await AsyncExecuter.ToListAsync(queryable);
            var productDtos = ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
            return new PagedResultDto<ProductDto>(
                totalCount,
                productDtos
            );
        }
        public virtual async Task<PagedResultDto<ProductDto>> GetListByProductPrice(GetProductsByPrice input)
        {
            var queryable = await Repository.GetQueryableAsync();
            queryable = queryable.Include(p => p.Category).Include(p => p.Manufacturer);
            queryable = queryable.Where(p => p.CategoryId == input.CategoryId);
            queryable = queryable.AsNoTracking().Where(p =>
                    (p.DiscountedPrice ?? p.OriginalPrice) >= input.MinPrice &&
                    (p.DiscountedPrice ?? p.OriginalPrice) <= input.MaxPrice
                );
            var totalCount = await AsyncExecuter.CountAsync(queryable);
            queryable = queryable
                .OrderBy(input.Sorting ?? nameof(Product.ProductName))
                .PageBy(input);
            var products = await AsyncExecuter.ToListAsync(queryable);
            var productDtos = ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
            return new PagedResultDto<ProductDto>(
                totalCount,
                productDtos
            );
        }
        public virtual async Task<PagedResultDto<ProductDto>> GetProductsByName(GetProductByName input)
        {
            var queryable = await Repository.GetQueryableAsync();
            queryable = queryable.Include(p => p.Category).Include(p => p.Manufacturer);
            queryable = queryable.AsNoTracking().Where(p => p.ProductName.Contains(input.Filter));

            queryable = queryable
                .OrderBy(input.Sorting ?? nameof(Product.ProductName))
                .PageBy(input);
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var products = await AsyncExecuter.ToListAsync(queryable);
            var productDtos = ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
            return new PagedResultDto<ProductDto>(
                totalCount,
                productDtos
            );
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetProductByManufacturer(GetProductsByManufacturer input)
        {
            var queryable = await Repository.GetQueryableAsync();
            queryable = queryable.Include(p => p.Category).Include(p => p.Manufacturer);
            queryable = queryable.Where(p => p.CategoryId == input.CategoryId);
            queryable = queryable.Where(p => p.ManufacturerId == input.ManufacturerId);
            queryable = queryable
                .OrderBy(input.Sorting ?? nameof(Product.ProductName))
                .PageBy(input);
            var products = await AsyncExecuter.ToListAsync(queryable);
            var productDtos = ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
            var totalCount = await AsyncExecuter.CountAsync(queryable);
            return new PagedResultDto<ProductDto>(
                totalCount,
                productDtos
            );
        }
    }
}
