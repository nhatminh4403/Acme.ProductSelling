using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
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
        //private readonly IRepository<MonitorSpecification, Guid> _monitorSpecRepository;
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly ISpecificationService _specificationService;

        /*        private readonly IRepository<MouseSpecification, Guid> _mouseSpecRepository;
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
                private readonly IRepository<HeadsetSpecification, Guid> _headsetSpecRepository;*/

        public ProductAppService(
            IRepository<Product, Guid> productRepository,
            IRepository<Category, Guid> categoryRepository,
            /*            IRepository<MonitorSpecification, Guid> monitorSpecRepository,
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
                        IRepository<HeadsetSpecification, Guid> headsetSpecRepository,*/
            ISpecificationService specificationService)
            : base(productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            ConfigurePolicies();

            _specificationService = specificationService;
        }
        private void ConfigurePolicies()
        {
            GetPolicyName = null;
            CreatePolicyName = ProductSellingPermissions.Products.Create;
            UpdatePolicyName = ProductSellingPermissions.Products.Edit;
            DeletePolicyName = ProductSellingPermissions.Products.Delete;
        }
        public override async Task<ProductDto> GetAsync(Guid id)
        {
            var query = await Repository.GetQueryableAsync();
            var product = await query
                .IncludeAllRelations()
                .FirstOrDefaultAsync(p => p.Id == id);

            return product == null ? throw new EntityNotFoundException(typeof(Product), id) : ObjectMapper.Map<Product, ProductDto>(product);
        }


        public async Task<ProductDto> GetProductBySlug(string slug)
        {
            var query = await Repository.GetQueryableAsync();
            var product = await query
                .AsNoTracking()
                .IncludeAllRelations()
                .FirstOrDefaultAsync(p => p.UrlSlug.ToLower() == slug.ToLower());

            if (product == null)
            {
                throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Product), slug);
            }
            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        [Authorize(ProductSellingPermissions.Products.Create)]
        public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {
            try
            {
                await CheckCreatePolicyAsync();

                var product = await CreateProductEntityAsync(input);
                await Repository.InsertAsync(product, autoSave: true);

                return await GetAsync(product.Id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating product with name: {ProductName}", input.ProductName);
                throw;
            }
        }
        /*       private async Task CreateAndLinkSpecificationsAsync(Product product, CreateUpdateProductDto dto, SpecificationType specType)
               {
                   // This method handles creating the specific spec entity, saving it, and linking its ID to the product.
                   switch (specType)
                   {
                       case SpecificationType.Monitor:
                           if (dto.MonitorSpecification != null)
                           {
                               var spec = ObjectMapper.Map<CreateUpdateMonitorSpecificationDto, MonitorSpecification>(dto.MonitorSpecification);
                               spec = await _monitorSpecRepository.InsertAsync(spec, autoSave: true);
                               product.MonitorSpecificationId = spec.Id;
                           }
                           break;
                       case SpecificationType.Mouse:
                           if (dto.MouseSpecification != null)
                           {
                               var spec = ObjectMapper.Map<CreateUpdateMouseSpecificationDto, MouseSpecification>(dto.MouseSpecification);
                               spec = await _mouseSpecRepository.InsertAsync(spec, autoSave: true);
                               product.MouseSpecificationId = spec.Id;
                           }
                           break;
                       case SpecificationType.Laptop:
                           if (dto.LaptopSpecification != null)
                           {
                               var spec = ObjectMapper.Map<CreateUpdateLaptopSpecificationDto, LaptopSpecification>(dto.LaptopSpecification);
                               spec = await _laptopSpecRepository.InsertAsync(spec, autoSave: true);
                               product.LaptopSpecificationId = spec.Id;
                           }
                           break;
                       case SpecificationType.CPU:
                           if (dto.CpuSpecification != null)
                           {
                               var spec = ObjectMapper.Map<CreateUpdateCpuSpecificationDto, CpuSpecification>(dto.CpuSpecification);
                               spec = await _cpuSpecRepository.InsertAsync(spec, autoSave: true);
                               product.CpuSpecificationId = spec.Id;
                           }
                           break;
                       case SpecificationType.GPU:
                           if (dto.GpuSpecification != null)
                           {
                               var spec = ObjectMapper.Map<CreateUpdateGpuSpecificationDto, GpuSpecification>(dto.GpuSpecification);
                               spec = await _gpuSpecRepository.InsertAsync(spec, autoSave: true);
                               product.GpuSpecificationId = spec.Id;
                           }
                           break;
                       case SpecificationType.RAM:
                           if (dto.RamSpecification != null)
                           {
                               var spec = ObjectMapper.Map<CreateUpdateRamSpecificationDto, RamSpecification>(dto.RamSpecification);
                               spec = await _ramSpecRepository.InsertAsync(spec, autoSave: true);
                               product.RamSpecificationId = spec.Id;
                           }
                           break;
                       case SpecificationType.Motherboard:
                           if (dto.MotherboardSpecification != null)
                           {
                               var spec = ObjectMapper.Map<CreateUpdateMotherboardSpecificationDto, MotherboardSpecification>(dto.MotherboardSpecification);
                               spec = await _motherboardSpecRepository.InsertAsync(spec, autoSave: true);
                               product.MotherboardSpecificationId = spec.Id;
                           }
                           break;
                       case SpecificationType.Storage:
                           if (dto.StorageSpecification != null)
                           {
                               var spec = ObjectMapper.Map<CreateUpdateStorageSpecificationDto, StorageSpecification>(dto.StorageSpecification);
                               spec = await _storageSpecRepository.InsertAsync(spec, autoSave: true);
                               product.StorageSpecificationId = spec.Id;
                           }
                           break;
                       case SpecificationType.PSU:
                           if (dto.PsuSpecification != null)
                           {
                               var spec = ObjectMapper.Map<CreateUpdatePsuSpecificationDto, PsuSpecification>(dto.PsuSpecification);
                               spec = await _psuSpecRepository.InsertAsync(spec, autoSave: true);
                               product.PsuSpecificationId = spec.Id;
                           }
                           break;
                       case SpecificationType.Case:
                           if (dto.CaseSpecification != null)
                           {
                               var spec = ObjectMapper.Map<CreateUpdateCaseSpecificationDto, CaseSpecification>(dto.CaseSpecification);
                               spec = await _caseSpecRepository.InsertAsync(spec, autoSave: true);
                               product.CaseSpecificationId = spec.Id;
                           }
                           break;
                       case SpecificationType.CPUCooler:
                           if (dto.CpuCoolerSpecification != null)
                           {
                               var spec = ObjectMapper.Map<CreateUpdateCpuCoolerSpecificationDto, CpuCoolerSpecification>(dto.CpuCoolerSpecification);
                               spec = await _cpuCoolerSpecRepository.InsertAsync(spec, autoSave: true);
                               product.CpuCoolerSpecificationId = spec.Id;
                           }
                           break;
                       case SpecificationType.Keyboard:
                           if (dto.KeyboardSpecification != null)
                           {
                               var spec = ObjectMapper.Map<CreateUpdateKeyboardSpecificationDto, KeyboardSpecification>(dto.KeyboardSpecification);
                               spec = await _keyboardSpecRepository.InsertAsync(spec, autoSave: true);
                               product.KeyboardSpecificationId = spec.Id;
                           }
                           break;
                       case SpecificationType.Headset:
                           if (dto.HeadsetSpecification != null)
                           {
                               var spec = ObjectMapper.Map<CreateUpdateHeadsetSpecificationDto, HeadsetSpecification>(dto.HeadsetSpecification);
                               spec = await _headsetSpecRepository.InsertAsync(spec, autoSave: true);
                               product.HeadsetSpecificationId = spec.Id;
                           }
                           break;
                       case SpecificationType.None:
                       default:
                           // No specific specification to create or link
                           break;
                   }
               }*/

        [Authorize(ProductSellingPermissions.Products.Edit)]
        public override async Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto input)
        {
            await CheckUpdatePolicyAsync();

            var product = await Repository.GetAsync(id);
            var category = await _categoryRepository.GetAsync(input.CategoryId);

            await UpdateProductAsync(product, input, category);
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
        /*     private async Task DeleteOldSpecificationAsync(Product product, SpecificationType? newSpecType)
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
             }*/
        public override async Task DeleteAsync(Guid id)
        {
            await CheckDeletePolicyAsync();

            var product = await Repository.FindAsync(id);
            if (product == null) return;

            await _specificationService.DeleteAllSpecificationsAsync(product);
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
            var queryable = await BuildCategoryQueryAsync(input.CategoryId);
            return await ExecutePagedQueryAsync(queryable, input);
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetListByProductPrice(GetProductsByPrice input)
        {
            var queryable = await BuildPriceRangeQueryAsync(input);
            return await ExecutePagedQueryAsync(queryable, input);
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetProductsByName(GetProductByName input)
        {
            var queryable = await BuildNameSearchQueryAsync(input.Filter);
            return await ExecutePagedQueryAsync(queryable, input);
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetProductByManufacturer(GetProductsByManufacturer input)
        {
            var queryable = await BuildManufacturerQueryAsync(input);
            return await ExecutePagedQueryAsync(queryable, input);
        }
        private async Task<Product> GetProductWithRelationsAsync(Guid id)
        {
            var query = await Repository.GetQueryableAsync();
            return await query
                .IncludeAllRelations()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        private async Task<Product> CreateProductEntityAsync(CreateUpdateProductDto input)
        {
            var product = ObjectMapper.Map<CreateUpdateProductDto, Product>(input);
            product.UrlSlug = UrlHelperMethod.RemoveDiacritics(product.ProductName);

            var category = await _categoryRepository.GetAsync(input.CategoryId);
            await _specificationService.CreateSpecificationAsync(product, input, category.SpecificationType);

            return product;
        }

        private async Task UpdateProductAsync(Product product, CreateUpdateProductDto input, Category newCategory)
        {
            var oldCategoryId = product.CategoryId;

            if (oldCategoryId != newCategory.Id)
            {
                await _specificationService.HandleCategoryChangeAsync(product, newCategory.SpecificationType);
            }

            ObjectMapper.Map(input, product);
            product.CategoryId = newCategory.Id;

            await _specificationService.UpdateSpecificationAsync(product, input, newCategory.SpecificationType);
        }

        private async Task<IQueryable<Product>> BuildCategoryQueryAsync(Guid categoryId)
        {
            var queryable = await Repository.GetQueryableAsync();
            return queryable
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId);
        }

        private async Task<IQueryable<Product>> BuildPriceRangeQueryAsync(GetProductsByPrice input)
        {
            var queryable = await Repository.GetQueryableAsync();
            return queryable
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .AsNoTracking()
                .Where(p => p.CategoryId == input.CategoryId)
                .Where(p => (p.DiscountedPrice ?? p.OriginalPrice) >= input.MinPrice &&
                           (p.DiscountedPrice ?? p.OriginalPrice) <= input.MaxPrice);
        }

        private async Task<IQueryable<Product>> BuildNameSearchQueryAsync(string searchTerm)
        {
            var queryable = await Repository.GetQueryableAsync();
            return queryable
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .AsNoTracking()
                .Where(p => p.ProductName.Contains(searchTerm));
        }

        private async Task<IQueryable<Product>> BuildManufacturerQueryAsync(GetProductsByManufacturer input)
        {
            var queryable = await Repository.GetQueryableAsync();
            return queryable
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Where(p => p.CategoryId == input.CategoryId && p.ManufacturerId == input.ManufacturerId);
        }

        private async Task<PagedResultDto<ProductDto>> ExecutePagedQueryAsync<TInput>(
            IQueryable<Product> queryable,
            TInput input)
            where TInput : PagedAndSortedResultRequestDto
        {
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var products = await AsyncExecuter.ToListAsync(
                queryable
                    .OrderBy(input.Sorting ?? nameof(Product.ProductName))
                    .PageBy(input));

            var productDtos = ObjectMapper.Map<List<Product>, List<ProductDto>>(products);

            return new PagedResultDto<ProductDto>(totalCount, productDtos);
        }
    }


}
