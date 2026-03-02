using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products.BackgroundJobs.ProductRelease;
using Acme.ProductSelling.Products.Caching;
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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
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
        //private readonly IRepository<CaseSpecification, Guid> _caseSpecificationRepository;
        //private readonly IRepository<CpuCoolerSpecification, Guid> _cpuCoolerSpecificationRepository;
        private readonly IStoreInventoryRepository _storeInventoryRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly ILogger<ProductAppService> _logger;
        private readonly IRecentlyViewedProductAppService _recentlyViewedService;

        private readonly ProductToProductDtoMapper _productToProductDtoMapper;
        private readonly ProductDtoToCreateUpdateProductDtoMapper _productDtoToCreateUpdateProductDtoMapper;
        private readonly CreateUpdateProductDtoToProductMapper _createUpdateProductDtoToProductMapper;

        private readonly IDistributedCache<ProductDto, Guid> _productDetailCache;
        private readonly IDistributedCache<ProductDto, string> _productSlugCache;
        private readonly IDistributedCache<List<FeaturedCategoryProductsDto>, string> _featuredCache;

        public ProductAppService(IRepository<Product, Guid> repository,
                                 IRepository<Category, Guid> categoryRepository,
                                 IRepository<CaseMaterial> caseMaterialRepository,
                                 //IRepository<CaseSpecification, Guid> caseSpecificationRepository,
                                 //IRepository<CpuCoolerSpecification, Guid> cpuCoolerSpecificationRepository,
                                 ISpecificationService specificationService,
                                 IProductRepository productRepository,
                                 IStoreInventoryRepository storeInventoryRepository,
                                 IStoreRepository storeRepository,
                                 IBackgroundJobManager backgroundJobManager,
                                 ILogger<ProductAppService> logger,
                                 IRecentlyViewedProductAppService recentlyViewedService,
                                 ProductToProductDtoMapper productToProductDtoMapper,
                                 ProductDtoToCreateUpdateProductDtoMapper productDtoToCreateUpdateProductDtoMapper,
                                 CreateUpdateProductDtoToProductMapper createUpdateProductDtoToProductMapper,
                                 IDistributedCache<ProductDto, Guid> productDetailCache,
                                 IDistributedCache<ProductDto, string> productSlugCache,
                                 IDistributedCache<List<FeaturedCategoryProductsDto>, string> featuredCache)
            : base(repository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _caseMaterialRepository = caseMaterialRepository;
            //_caseSpecificationRepository = caseSpecificationRepository;
            //_cpuCoolerSpecificationRepository = cpuCoolerSpecificationRepository;
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
            _productDetailCache = productDetailCache;
            _productSlugCache = productSlugCache;
            _featuredCache = featuredCache;
        }
        private void ConfigurePolicies()
        {
            GetPolicyName = null;
            CreatePolicyName = ProductSellingPermissions.Products.Create;
            UpdatePolicyName = ProductSellingPermissions.Products.Edit;
            DeletePolicyName = ProductSellingPermissions.Products.Delete;
        }
        [Authorize(ProductSellingPermissions.Products.Default)]
        public override async Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.AsNoTracking()
                .IncludeAllRelations();


            var totalCount = await AsyncExecuter.CountAsync(query);
            query = query.OrderBy(input.Sorting ?? nameof(Product.ProductName));
            query = query.PageBy(input);
            var products = await AsyncExecuter.ToListAsync(query);

            var productDtos = products.Select(p => _productToProductDtoMapper.Map(p)).ToList();

            _ = Task.Run(async () =>
            {
                foreach (var dto in productDtos)
                {
                    await _productDetailCache.SetAsync(
                        dto.Id,
                        dto,
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ProductCacheKeys.DetailTtlMinutes) },
                        considerUow: false);
                }
            });

            return new PagedResultDto<ProductDto>(
                totalCount,
                productDtos
            );
        }
        [Authorize(ProductSellingPermissions.Products.Default)]
        public override async Task<ProductDto> GetAsync(Guid id)
        {
            // Try cache first
            var cached = await _productDetailCache.GetAsync(id);
            if (cached != null)
            {
                _logger.LogDebug("[GetAsync] Cache HIT - ProductId: {ProductId}", id);
                _ = Task.Run(async () =>
                {
                    try { await _recentlyViewedService.TrackProductViewAsync(id); }
                    catch (Exception ex) { _logger.LogWarning(ex, "Failed to track product view for ProductId: {ProductId}", id); }
                });
                return cached;
            }


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

                var product = _createUpdateProductDtoToProductMapper.Map(input);

                product.UrlSlug = UrlHelperMethod.RemoveDiacritics(product.ProductName);

                await Repository.InsertAsync(product, autoSave: true);

                await Task.WhenAll(
                    _specificationService.CreateSpecificationAsync(product.Id, input, category.SpecificationType),
                    HandleManyToManyAsync(product.Id, input)
                );

                await InvalidateFeaturedCacheAsync();

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
            var oldSlug = product.UrlSlug;

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
            await InvalidateProductCacheAsync(id, oldSlug);
            await InvalidateFeaturedCacheAsync();
            return await GetAsync(product.Id);
        }
        [Authorize(ProductSellingPermissions.Products.Delete)]
        public override async Task DeleteAsync(Guid id)
        {
            var product = await Repository.FindAsync(id);
            var slug = product?.UrlSlug;

            await base.DeleteAsync(id);

            await InvalidateProductCacheAsync(id, slug);
            await InvalidateFeaturedCacheAsync();
        }


        private Task InvalidateFeaturedCacheAsync()
            => _featuredCache.RemoveAsync(ProductCacheKeys.FeaturedCarousels);
        private async Task InvalidateProductCacheAsync(Guid productId, string slug = null)
        {
            await _productDetailCache.RemoveAsync(productId);

            if (!string.IsNullOrWhiteSpace(slug))
                await _productSlugCache.RemoveAsync(ProductCacheKeys.DetailBySlug + slug.ToLower());
        }
        private async Task ScheduleProductReleaseJobAsync(Product product)
        {
            if (!product.ReleaseDate.HasValue || product.ReleaseDate.Value <= Clock.Now)
            {
                return; // Already released or no release date
            }

            var delay = product.ReleaseDate.Value - Clock.Now;

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
            var caseProduct = await _productRepository.GetAsync(s => s.Id == productId, includeDetails: true);

            var spec = caseProduct.SpecificationBase as CaseSpecification;

            spec.Materials.Clear();

            foreach (var materialId in materialIds)
            {
                spec.Materials.Add(new CaseMaterial
                {
                    CaseSpecificationId = spec.Id,
                    MaterialId = materialId
                });
            }

            await _productRepository.UpdateAsync(caseProduct, autoSave: false);
        }
        private async Task UpdateCpuCoolerSocketsAsync(Guid productId, List<Guid> socketIds)
        {
            var cpuCoolerProduct = await _productRepository
                .GetAsync(s => s.Id == productId, includeDetails: true);

            var spec = cpuCoolerProduct.SpecificationBase as CpuCoolerSpecification;

            spec.SupportedSockets.Clear();

            foreach (var socketId in socketIds)
            {
                spec.SupportedSockets.Add(new CpuCoolerSocketSupport
                {
                    CpuCoolerSpecificationId = spec.Id,
                    SocketId = socketId
                });
            }

            await _productRepository.UpdateAsync(cpuCoolerProduct, autoSave: false);
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

    }
}
