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
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;
        private readonly ICurrentUser _currentUser; 
        private readonly IdentityUserManager _userManager;

        public ProfileAppService(IRepository<IdentityUser, Guid> identityUserRepository, ICurrentUser currentUser, IdentityUserManager userManager)
        {
            _identityUserRepository = identityUserRepository;
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
            var user = await _identityUserRepository.GetAsync(_currentUser.Id.Value);
            
            return new UpdateProfileDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber,
                ShippingAddress = user.GetProperty<string>("ShippingAddress"),
                DateOfBirth = user.GetProperty<DateTime?>("DateOfBirth"),
                Gender = user.GetProperty<UserGender>("Gender")
            };
        }

        public async Task<UpdateProfileDto> UpdateAsync(UpdateProfileDto input)
        {
            var user = await _identityUserRepository.GetAsync(_currentUser.Id.Value);

            user.Name = input.Name;
            user.Surname = input.Surname;
            if (user.UserName != input.UserName)
            {
                await _userManager.SetUserNameAsync(user, input.UserName);
            }
            if (user.Email != input.Email)
            {
                await _userManager.SetEmailAsync(user, input.Email);
            }

            // Use the provided method to set phone number, since the setter is protected internal
            user.SetPhoneNumber(input.PhoneNumber, user.PhoneNumberConfirmed);

            user.SetProperty("ShippingAddress", input.ShippingAddress);
            user.SetProperty("DateOfBirth", input.DateOfBirth);
            user.SetProperty("Gender", input.Gender);

            await _userManager.UpdateAsync(user);

            return input;
        }
    }
}
