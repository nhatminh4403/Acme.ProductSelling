using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Users
{
    public class ProfileAppService : ApplicationService, Acme.ProductSelling.Users.IProfileAppService, ITransientDependency
    {
        private readonly IAppUserRepository _userRepository;
        private readonly ICurrentUser _currentUser; 
        private readonly IdentityUserManager _userManager;

        public ProfileAppService(IAppUserRepository userRepository, ICurrentUser currentUser, IdentityUserManager userManager)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
            _userManager = userManager;
        }

        public async Task ChangePasswordAsync(ChangePasswordDto input)
        {
            var user = await _userManager.GetByIdAsync(_currentUser.Id.Value);

            var result = await _userManager.ChangePasswordAsync(
                user,
                input.CurrentPassword,
                input.NewPassword
            );

            if (!result.Succeeded)
            {
                throw new UserFriendlyException(
                    result.Errors.Select(e => e.Description).JoinAsString(", ")
                );
            }
        }

        public async Task<UpdateProfileDto> GetAsync()
        {
            var user = await _userRepository.GetAsync(_currentUser.Id.Value);
            return ObjectMapper.Map<AppUser, UpdateProfileDto>(user);
        }

        public async Task<UpdateProfileDto> UpdateAsync(UpdateProfileDto input)
        {
            var user = await _userRepository.GetAsync(_currentUser.Id.Value);

            user.SetProperty(user.UserName, input.UserName);
            user.SetProperty(user.Email, input.Email);
            user.SetProperty(user.PhoneNumber, input.PhoneNumber);
            user.Name = input.Name;
            user.Surname = input.Surname;
            user.ShippingAddress = input.ShippingAddress;
            user.DateOfBirth = input.DateOfBirth;
            user.Gender = input.Gender;
            await _userRepository.UpdateAsync(user);

            return ObjectMapper.Map<AppUser, UpdateProfileDto>(user);
        }
    }
}
