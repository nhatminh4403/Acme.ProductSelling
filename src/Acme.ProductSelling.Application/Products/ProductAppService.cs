using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Permissions;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
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
        public ProductAppService(
            IRepository<Product, Guid> productRepository,
            IRepository<Category, Guid> categoryRepository)
            : base(productRepository)
        {
            _categoryRepository = categoryRepository;
            GetPolicyName = ProductSellingPermissions.Products.Default;
            CreatePolicyName = ProductSellingPermissions.Products.Create;
            UpdatePolicyName = ProductSellingPermissions.Products.Edit;
            DeletePolicyName = ProductSellingPermissions.Products.Delete;
        }

        protected override async Task<ProductDto> MapToGetOutputDtoAsync(Product entity)
        {
            var dto = ObjectMapper.Map<Product, ProductDto>(entity);

            var category = await _categoryRepository.GetAsync(entity.CategoryId);
            dto.CategoryName = category.Name;

            return dto;
        }
      

    }
}
