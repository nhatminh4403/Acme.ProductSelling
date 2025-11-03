using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Specifications.Junctions;
using Acme.ProductSelling.Specifications.Models;
using Acme.ProductSelling.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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
            IRepository<CaseMaterial> caseMaterialRepository,
            IRepository<CaseSpecification, Guid> caseSpecificationRepository,
            IRepository<CpuCoolerSpecification, Guid> cpuCoolerSpecificationRepository,
            ISpecificationService specificationService)
            : base(productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _caseMaterialRepository = caseMaterialRepository;
            _caseSpecificationRepository = caseSpecificationRepository;
            _cpuCoolerSpecificationRepository = cpuCoolerSpecificationRepository;
            _specificationService = specificationService;
            ConfigurePolicies();
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
