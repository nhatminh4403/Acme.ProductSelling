using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace Acme.ProductSelling.Categories
{
    public interface ICategoryAppService : ICrudAppService<
     CategoryDto,
     Guid,
     PagedAndSortedResultRequestDto,
     CreateUpdateCategoryDto>
    {
        //Task<ListResultDto<CategoryDto>> GetAllAsync();
        Task<ListResultDto<CategoryLookupDto>> GetCategoryLookupAsync();
        Task<ListResultDto<CategoryWithManufacturersDto>> GetListWithManufacturersAsync();
        Task<GroupedCategoriesResultDto> GetGroupedCategoriesAsync();
        Task<ListResultDto<CategoryWithManufacturersDto>> GetCategoriesWithManufacturersAsync();
    }
}

