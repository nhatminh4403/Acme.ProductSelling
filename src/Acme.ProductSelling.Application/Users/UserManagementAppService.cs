using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace Acme.ProductSelling.Users
{
    [Authorize(ProductSellingPermissions.Users.Default)]
    public class UserManagementAppService : ApplicationService, IUserManagementAppService
    {
        private readonly IIdentityUserRepository _userRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IStoreRepository _storeRepository;
        private readonly IAppUserRepository _appUserRepository;
        public UserManagementAppService(
            IIdentityUserRepository userRepository,
            IdentityUserManager userManager,
            IStoreRepository storeRepository,
            IAppUserRepository appUserRepository)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _storeRepository = storeRepository;
            _appUserRepository = appUserRepository;
        }

        public async Task<AppUserDto> GetStaffUserAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId) as AppUser;
            return await MapToAppUserDtoAsync(user);
        }

        [Authorize(ProductSellingPermissions.Users.AssignToStore)]
        public async Task<AppUserDto> AssignStaffToStoreAsync(Guid userId, Guid? storeId)
        {
            var user = await _userRepository.GetAsync(userId) as AppUser;

            if (user.IsCustomer())
            {
                throw new UserFriendlyException("Cannot assign store to customer accounts.");
            }

            if (storeId.HasValue)
            {
                // Validate store exists
                await _storeRepository.GetAsync(storeId.Value);
            }

            user.AssignToStore(storeId);
            await _userRepository.UpdateAsync(user, autoSave: true);

            Logger.LogInformation(
                "User {UserId} ({UserName}) assigned to store {StoreId}",
                userId, user.UserName, storeId
            );

            return await MapToAppUserDtoAsync(user);
        }

        [Authorize(ProductSellingPermissions.Users.AssignToStore)]
        public async Task<AppUserDto> UnassignStaffFromStoreAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId) as AppUser;
            user.UnassignFromStore();
            await _userRepository.UpdateAsync(user, autoSave: true);

            Logger.LogInformation(
                "User {UserId} ({UserName}) unassigned from store",
                userId, user.UserName
            );

            return await MapToAppUserDtoAsync(user);
        }

        public async Task<PagedResultDto<AppUserDto>> GetStoreStaffAsync(Guid storeId, PagedAndSortedResultRequestDto input)
        {
            var query = await _appUserRepository.GetQueryableAsync();

            // Cast to AppUser to access AssignedStoreId
            query = query.Where(u => ((AppUser)u).AssignedStoreId == storeId);

            var totalCount = await AsyncExecuter.CountAsync(query);

            var users = await AsyncExecuter.ToListAsync(
                query
                    .OrderBy(input.Sorting ?? "UserName")
                    .PageBy(input)
            );

            var userDtos = new List<AppUserDto>();
            foreach (var user in users)
            {
                userDtos.Add(await MapToAppUserDtoAsync(user as AppUser));
            }

            return new PagedResultDto<AppUserDto>(totalCount, userDtos);
        }

        public async Task<PagedResultDto<AppUserDto>> GetUnassignedStaffAsync(PagedAndSortedResultRequestDto input)
        {
            var query = await _appUserRepository.GetQueryableAsync();

            // Get users without store assignment and without customer profile
            query = query.Where(u =>
               ((AppUser)u).AssignedStoreId == null &&
               ((AppUser)u).Customer == null
           );

            var totalCount = await AsyncExecuter.CountAsync(query);

            var users = await AsyncExecuter.ToListAsync(
                query
                    .OrderBy(input.Sorting ?? "UserName")
                    .PageBy(input)
            );

            var userDtos = new List<AppUserDto>();
            foreach (var user in users)
            {
                userDtos.Add(await MapToAppUserDtoAsync(user as AppUser));
            }

            return new PagedResultDto<AppUserDto>(totalCount, userDtos);
        }

        // Helper method
        private async Task<AppUserDto> MapToAppUserDtoAsync(AppUser user)
        {
            var roles = await _userRepository.GetRolesAsync(user.Id);

            var dto = new AppUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber,
                AssignedStoreId = user.AssignedStoreId,
                UserType = user.GetUserType(),
                Roles = roles.Select(r => r.Name).ToArray(),
                IsCustomer = user.IsCustomer(),
                CustomerId = user.Customer?.Id
            };

            if (user.AssignedStoreId.HasValue)
            {
                var store = await _storeRepository.GetAsync(user.AssignedStoreId.Value);
                dto.AssignedStoreName = store.Name;
            }

            return dto;
        }
    }
}