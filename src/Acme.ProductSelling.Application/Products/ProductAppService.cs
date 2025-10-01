using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Junctions;
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
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly ISpecificationService _specificationService;
        private readonly IRepository<CaseMaterial> _caseMaterialRepository;
        private readonly IRepository<CaseSpecification, Guid> _caseSpecificationRepository;
        private readonly IRepository<CpuCoolerSpecification, Guid> _cpuCoolerSpecificationRepository;

        public ProductAppService(
            IRepository<Product, Guid> productRepository,
            IRepository<Category, Guid> categoryRepository,

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
                var category = await _categoryRepository.GetAsync(input.CategoryId);

                //var product = await CreateProductEntityAsync(input);
                //await Repository.InsertAsync(product, autoSave: true);
                var product = ObjectMapper.Map<CreateUpdateProductDto, Product>(input);
                product.UrlSlug = UrlHelperMethod.RemoveDiacritics(product.ProductName);
                await Repository.InsertAsync(product, autoSave: true);
                await _specificationService.CreateSpecificationAsync(product.Id, input, category.SpecificationType);
                await HandleManyToManys(product.Id, input);

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

            var oldSpecType = product.Category.SpecificationType;
            ObjectMapper.Map(input, product);

            var newCategory = await _categoryRepository.GetAsync(input.CategoryId);

            if (product.CategoryId != input.CategoryId)
            {
                await _specificationService.HandleCategoryChangeAsync(product.Id, oldSpecType, newCategory.SpecificationType);
                product.CategoryId = newCategory.Id;
            }
            await _specificationService.UpdateSpecificationAsync(product.Id, input, newCategory.SpecificationType);

            await HandleManyToManys(product.Id, input);

            await Repository.UpdateAsync(product, autoSave: true);

            return await GetAsync(product.Id);
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
        private async Task HandleManyToManys(Guid productId, CreateUpdateProductDto input)
        {
            if (input.CaseSpecification != null)
            {
                var spec = await _caseSpecificationRepository.GetAsync(s => s.ProductId == productId, includeDetails: true);
                spec.Materials.Clear();
                foreach (var materialId in input.CaseSpecification.MaterialIds)
                {
                    spec.Materials.Add(new CaseMaterial { CaseSpecificationId = spec.Id, MaterialId = materialId });
                }
                await _caseSpecificationRepository.UpdateAsync(spec, autoSave: true);
            }

            if (input.CpuCoolerSpecification != null)
            {
                var spec = await _cpuCoolerSpecificationRepository.GetAsync(s => s.ProductId == productId, includeDetails: true);
                spec.SupportedSockets.Clear();
                foreach (var socketId in input.CpuCoolerSpecification.SupportedSocketIds)
                {
                    spec.SupportedSockets.Add(new CpuCoolerSocketSupport { CpuCoolerSpecificationId = spec.Id, SocketId = socketId });
                }
                await _cpuCoolerSpecificationRepository.UpdateAsync(spec, autoSave: true);
            }
        }
    }


}
