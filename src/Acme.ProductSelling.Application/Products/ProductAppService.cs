using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products.BackgroundJobs;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Specifications.Junctions;
using Acme.ProductSelling.Specifications.Models;
using Acme.ProductSelling.Stores;
using Acme.ProductSelling.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
namespace Acme.ProductSelling.Products
{
    public class ProductAppService :
        CrudAppService<Product, ProductDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProductDto>,
        IProductAppService
    {
        private readonly IRepository<Category, Guid> _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISpecificationService _specificationService;
        private readonly IRepository<CaseMaterial> _caseMaterialRepository;
        private readonly IRepository<CaseSpecification, Guid> _caseSpecificationRepository;
        private readonly IRepository<CpuCoolerSpecification, Guid> _cpuCoolerSpecificationRepository;
        private readonly IStoreInventoryRepository _storeInventoryRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly ILogger<ProductAppService> _logger;
        public ProductAppService(
            IRepository<Product, Guid> repository,
            IRepository<Category, Guid> categoryRepository,
            IRepository<CaseMaterial> caseMaterialRepository,
            IRepository<CaseSpecification, Guid> caseSpecificationRepository,
            IRepository<CpuCoolerSpecification, Guid> cpuCoolerSpecificationRepository,
            ISpecificationService specificationService,
            IProductRepository productRepository,
            IStoreInventoryRepository storeInventoryRepository,
            IStoreRepository storeRepository,
            IBackgroundJobManager backgroundJobManager,
            ILogger<ProductAppService> logger)
            : base(repository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _caseMaterialRepository = caseMaterialRepository;
            _caseSpecificationRepository = caseSpecificationRepository;
            _cpuCoolerSpecificationRepository = cpuCoolerSpecificationRepository;
            _specificationService = specificationService;


            ConfigurePolicies();
            _storeInventoryRepository = storeInventoryRepository;
            _storeRepository = storeRepository;
            _backgroundJobManager = backgroundJobManager;
            _logger = logger;
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
                .AsNoTracking()
                .IncludeAllRelations()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new EntityNotFoundException(typeof(Product), id);
            }

            var productDto = ObjectMapper.Map<Product, ProductDto>(product);

            // NEW: Add store inventory information
            await PopulateStoreInventoryAsync(productDto, id);

            return productDto;
        }

        [Authorize(ProductSellingPermissions.Products.Create)]
        public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {
            try
            {
                await CheckCreatePolicyAsync();
                var category = await _categoryRepository.GetAsync(input.CategoryId);

                //var product = await CreateProductEntityAsync(input);
                //await Repository.InsertAsync(product, autoSave: true);
                var product = ObjectMapper.Map<CreateUpdateProductDto, Product>(input);

                product.UrlSlug = UrlHelperMethod.RemoveDiacritics(product.ProductName);
                await Repository.InsertAsync(product, autoSave: true);
                await Task.WhenAll(
                    _specificationService.CreateSpecificationAsync(product.Id, input, category.SpecificationType),
                    HandleManyToManyAsync(product.Id, input)
                );
                await ScheduleProductReleaseJobAsync(product);

                return await GetAsync(product.Id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating product with name: {ProductName}", input.ProductName);
                throw;
            }
        }

        [Authorize(ProductSellingPermissions.Products.Edit)]
        public override async Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto input)
        {
            await CheckUpdatePolicyAsync();

            var product = await (await Repository.GetQueryableAsync())
                .Include(p => p.Category)
                .FirstAsync(p => p.Id == id);
            var oldReleaseDate = product.ReleaseDate;
            var oldSpecType = product.Category.SpecificationType;
            ObjectMapper.Map(input, product);

            var newCategory = await _categoryRepository.GetAsync(input.CategoryId);

            if (product.CategoryId != input.CategoryId)
            {
                await _specificationService.HandleCategoryChangeAsync(product.Id, oldSpecType, newCategory.SpecificationType);
                product.CategoryId = newCategory.Id;
            }
            await _specificationService.UpdateSpecificationAsync(product.Id, input, newCategory.SpecificationType);

            await HandleManyToManyAsync(product.Id, input);

            await Repository.UpdateAsync(product, autoSave: true);
            if (oldReleaseDate != product.ReleaseDate)
            {
                await ScheduleProductReleaseJobAsync(product);
            }
            return await GetAsync(product.Id);
        }
        private async Task ScheduleProductReleaseJobAsync(Product product)
        {
            if (!product.ReleaseDate.HasValue || product.ReleaseDate.Value <= DateTime.Now)
            {
                return; // Already released or no release date
            }

            var delay = product.ReleaseDate.Value - DateTime.Now;

            await _backgroundJobManager.EnqueueAsync(
                new ProductReleaseJobArgs { ProductId = product.Id },
                delay: delay
            );

            Logger.LogInformation(
                "Scheduled release job for Product {ProductId} ({ProductName}) at {ReleaseDate}",
                product.Id,
                product.ProductName,
                product.ReleaseDate.Value
            );
        }
        private async Task HandleManyToManyAsync(Guid productId, CreateUpdateProductDto input)
        {
            var tasks = new List<Task>();

            // Handle Case materials
            if (input.CaseSpecification?.MaterialIds?.Count > 0)
            {
                tasks.Add(UpdateCaseMaterialsAsync(productId, input.CaseSpecification.MaterialIds));
            }

            // Handle CPU Cooler sockets
            if (input.CpuCoolerSpecification?.SupportedSocketIds?.Count > 0)
            {
                tasks.Add(UpdateCpuCoolerSocketsAsync(productId, input.CpuCoolerSpecification.SupportedSocketIds));
            }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }
        }
        private async Task UpdateCaseMaterialsAsync(Guid productId, List<Guid> materialIds)
        {
            var spec = await _caseSpecificationRepository
                .GetAsync(s => s.ProductId == productId, includeDetails: true);

            spec.Materials.Clear();

            foreach (var materialId in materialIds)
            {
                spec.Materials.Add(new CaseMaterial
                {
                    CaseSpecificationId = spec.Id,
                    MaterialId = materialId
                });
            }

            await _caseSpecificationRepository.UpdateAsync(spec, autoSave: false);
        }

        private async Task UpdateCpuCoolerSocketsAsync(Guid productId, List<Guid> socketIds)
        {
            var spec = await _cpuCoolerSpecificationRepository
                .GetAsync(s => s.ProductId == productId, includeDetails: true);

            spec.SupportedSockets.Clear();

            foreach (var socketId in socketIds)
            {
                spec.SupportedSockets.Add(new CpuCoolerSocketSupport
                {
                    CpuCoolerSpecificationId = spec.Id,
                    SocketId = socketId
                });
            }

            await _cpuCoolerSpecificationRepository.UpdateAsync(spec, autoSave: false);
        }
        private async Task PopulateStoreInventoryAsync(ProductDto productDto, Guid productId)
        {
            var inventories = await _storeInventoryRepository.GetByProductAsync(productId);

            productDto.StoreAvailability = new List<ProductStoreAvailabilityDto>();
            productDto.TotalStockAcrossAllStores = 0;

            foreach (var inventory in inventories)
            {
                var store = await _storeRepository.GetAsync(inventory.StoreId);

                productDto.StoreAvailability.Add(new ProductStoreAvailabilityDto
                {
                    StoreId = inventory.StoreId,
                    StoreName = store.Name,
                    Quantity = inventory.Quantity,
                    IsAvailableForSale = inventory.IsAvailableForSale,
                    NeedsReorder = inventory.NeedsReorder()
                });

                productDto.TotalStockAcrossAllStores += inventory.Quantity;
            }
        }
    }
}
