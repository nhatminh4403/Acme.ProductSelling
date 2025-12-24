using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Categories.Dtos;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products.BackgroundJobs.ProductRelease;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Helpers;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Products.Specification;
using Acme.ProductSelling.Specifications.Junctions;
using Acme.ProductSelling.Specifications.Models;
using Acme.ProductSelling.StoreInventories;
using Acme.ProductSelling.Stores;
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
        private readonly IRecentlyViewedProductAppService _recentlyViewedService;

        private readonly ProductToProductDtoMapper _productToProductDtoMapper;
        private readonly ProductDtoToCreateUpdateProductDtoMapper _productDtoToCreateUpdateProductDtoMapper;
        private readonly CreateUpdateProductDtoToProductMapper _createUpdateProductDtoToProductMapper;
        public ProductAppService(IRepository<Product, Guid> repository,
                                 IRepository<Category, Guid> categoryRepository,
                                 IRepository<CaseMaterial> caseMaterialRepository,
                                 IRepository<CaseSpecification, Guid> caseSpecificationRepository,
                                 IRepository<CpuCoolerSpecification, Guid> cpuCoolerSpecificationRepository,
                                 ISpecificationService specificationService,
                                 IProductRepository productRepository,
                                 IStoreInventoryRepository storeInventoryRepository,
                                 IStoreRepository storeRepository,
                                 IBackgroundJobManager backgroundJobManager,
                                 ILogger<ProductAppService> logger,
                                 IRecentlyViewedProductAppService recentlyViewedService,
                                 ProductToProductDtoMapper productToProductDtoMapper,
                                 ProductDtoToCreateUpdateProductDtoMapper productDtoToCreateUpdateProductDtoMapper,
                                 CreateUpdateProductDtoToProductMapper createUpdateProductDtoToProductMapper)
            : base(repository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _caseMaterialRepository = caseMaterialRepository;
            _caseSpecificationRepository = caseSpecificationRepository;
            _cpuCoolerSpecificationRepository = cpuCoolerSpecificationRepository;
            _specificationService = specificationService;
            _storeInventoryRepository = storeInventoryRepository;
            _storeRepository = storeRepository;
            _backgroundJobManager = backgroundJobManager;
            _logger = logger;
            _recentlyViewedService = recentlyViewedService;
            _productToProductDtoMapper = productToProductDtoMapper;
            _productDtoToCreateUpdateProductDtoMapper = productDtoToCreateUpdateProductDtoMapper;
            _productToProductDtoMapper = productToProductDtoMapper;
            _productDtoToCreateUpdateProductDtoMapper = productDtoToCreateUpdateProductDtoMapper;
            _createUpdateProductDtoToProductMapper = createUpdateProductDtoToProductMapper;

            ConfigurePolicies();
        }
        private void ConfigurePolicies()
        {
            GetPolicyName = null;
            CreatePolicyName = ProductSellingPermissions.Products.Create;
            UpdatePolicyName = ProductSellingPermissions.Products.Edit;
            DeletePolicyName = ProductSellingPermissions.Products.Delete;
        }

        [AllowAnonymous]
        public override async Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.StoreInventories);
            var totalCount = await AsyncExecuter.CountAsync(query);
            query = query.OrderBy(input.Sorting ?? nameof(Product.ProductName));
            query = query.PageBy(input);
            var products = await AsyncExecuter.ToListAsync(query);

            var productDtos = products.Select(p => _productToProductDtoMapper.Map(p)).ToList();

            return new PagedResultDto<ProductDto>(
                totalCount,
                productDtos
            );
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

            var productDto = _productToProductDtoMapper.Map(product);

            await PopulateStoreInventoryAsync(productDto, id);
            _ = Task.Run(async () =>
            {
                try
                {
                    await _recentlyViewedService.TrackProductViewAsync(id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to track product view for ProductId: {ProductId}", id);
                }
            });
            return productDto;
        }


        [Authorize(ProductSellingPermissions.Products.Create)]
        public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {
            try
            {
                await CheckCreatePolicyAsync();
                var category = await _categoryRepository.GetAsync(input.CategoryId);

                // Use Mapperly
                var product = _createUpdateProductDtoToProductMapper.Map(input);

                // Set explicit/calculated properties
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

            // Use Mapperly to update existing entity
            _createUpdateProductDtoToProductMapper.Map(input, product);

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
            //var inventories = await _storeInventoryRepository.GetByProductAsync(productId);
            var inventoryQuery = await _storeInventoryRepository.GetQueryableAsync();
            var storeQuery = await _storeRepository.GetQueryableAsync();
            var availability = await (from inv in inventoryQuery
                                      join store in storeQuery on inv.StoreId equals store.Id
                                      where inv.ProductId == productId
                                      select new { inv, store.Name })
                          .ToListAsync();
            productDto.StoreAvailability = new List<ProductStoreAvailabilityDto>();
            productDto.TotalStockAcrossAllStores = 0;


            foreach (var item in availability)
            {
                productDto.StoreAvailability.Add(new ProductStoreAvailabilityDto
                {
                    StoreId = item.inv.StoreId,
                    StoreName = item.Name, // We got this via Join, no extra DB call
                    Quantity = item.inv.Quantity,
                    IsAvailableForSale = item.inv.IsAvailableForSale,
                    NeedsReorder = item.inv.NeedsReorder()
                });

                productDto.TotalStockAcrossAllStores += item.inv.Quantity;
            }
        }

        public async Task<List<FeaturedCategoryProductsDto>> GetFeaturedProductCarouselsAsync()
        {
            var result = new List<FeaturedCategoryProductsDto>();

            // 1. Define the Spec Types we want
            var featuredSpecTypes = new[]
            {
                SpecificationType.Mouse,
                SpecificationType.Laptop,
                SpecificationType.Monitor,
                SpecificationType.Keyboard
            };
            var categories = await _categoryRepository.GetListAsync(c => featuredSpecTypes.Contains(c.SpecificationType));

            var categoriesToFeature = categories.Take(4).ToList();

            // 2. Get Categories (Lightweight query)
            foreach (var category in categoriesToFeature)
            {
                var query = await _productRepository.GetQueryableAsync();

                var rawProducts = await query
                    .AsNoTracking()
                    .Where(p => p.CategoryId == category.Id && p.StockCount > 0)
                    .OrderBy(p => Guid.NewGuid())
                    .Take(20) // Get 20, then shuffle and take 10
                              //.Select(p => new  // Project only needed fields
                              //{
                              //    p.Id,
                              //    p.ProductName,
                              //    p.UrlSlug,
                              //    p.ImageUrl,
                              //    p.OriginalPrice,
                              //    p.DiscountedPrice,
                              //    p.DiscountPercent,
                              //    CategoryName = p.Category.Name,
                              //    ManufacturerName = p.Manufacturer.Name,
                              //    p.StockCount
                              //})
                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer)
                    .Include(p => p.StoreInventories)
                    .ToListAsync();


                rawProducts = rawProducts.OrderBy(x => Guid.NewGuid()).Shuffle().ToList();
                if (!rawProducts.Any()) continue;

                // Map to DTO
                var productDtos = rawProducts.Select(p => _productToProductDtoMapper.Map(p)).ToList();

                result.Add(new FeaturedCategoryProductsDto
                {
                    Category = ObjectMapper.Map<Category, CategoryDto>(category),
                    Products = productDtos
                });
            }
            return result;

        }
    }
}
