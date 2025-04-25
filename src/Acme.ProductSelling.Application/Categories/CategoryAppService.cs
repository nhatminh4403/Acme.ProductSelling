using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Categories
{
    public class CategoryAppService : CrudAppService<Category, CategoryDto,
        Guid, PagedAndSortedResultRequestDto, CreateUpdateCategoryDto>, ICategoryAppService

    {
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public CategoryAppService(
                 ICategoryRepository categoryRepository,
                 IRepository<Product, Guid> productRepository)
                 : base(categoryRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            GetPolicyName = ProductSellingPermissions.Products.Default;
            CreatePolicyName = ProductSellingPermissions.Products.Create;
            UpdatePolicyName = ProductSellingPermissions.Products.Edit;
            DeletePolicyName = ProductSellingPermissions.Products.Delete;
        }


        [AllowAnonymous] // Ai cũng có thể gọi để lấy danh sách cho dropdown
        public async Task<ListResultDto<CategoryLookupDto>> GetCategoryLookupAsync()
        {
            var categories = await Repository.GetListAsync();
            return new ListResultDto<CategoryLookupDto>(
                ObjectMapper.Map<List<Category>, List<CategoryLookupDto>>(categories)
            );
        }
    }
}
