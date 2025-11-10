using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Identity;

namespace Acme.ProductSelling.Identity
{
    [Authorize(Permissions.ProductSellingPermissions.Users.ManageStoreAssignment)]
    public class UserStoreAppService : ApplicationService, IUserStoreAppService
    {
        private readonly IIdentityUserRepository _userRepository;

        public UserStoreAppService(IIdentityUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AssignUserToStoreAsync(AssignUserToStoreDto input)
        {
            var user = await _userRepository.GetAsync(input.UserId);
            user.SetProperty("AssignedStoreId", input.StoreId);
            await _userRepository.UpdateAsync(user);
        }

        public async Task<Guid?> GetUserAssignedStoreAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            return user.GetProperty<Guid?>("AssignedStoreId");
        }
    }
}
