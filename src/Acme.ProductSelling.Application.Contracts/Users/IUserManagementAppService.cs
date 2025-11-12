using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Users
{
    public interface IUserManagementAppService : IApplicationService
    {
        Task<AppUserDto> GetStaffUserAsync(Guid userId);
        Task<AppUserDto> AssignStaffToStoreAsync(Guid userId, Guid? storeId);
        Task<AppUserDto> UnassignStaffFromStoreAsync(Guid userId);
        Task<PagedResultDto<AppUserDto>> GetStoreStaffAsync(Guid storeId, PagedAndSortedResultRequestDto input);
        Task<PagedResultDto<AppUserDto>> GetUnassignedStaffAsync(PagedAndSortedResultRequestDto input);
    }
}