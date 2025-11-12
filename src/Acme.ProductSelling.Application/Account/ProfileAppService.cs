using Acme.ProductSelling.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Account
{
    public class ProfileAppService : ApplicationService, IProfileAppService, ITransientDependency
    {
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IdentityUserManager _userManager;
        private readonly IRepository<Customer, Guid> _customerRepository;

        public ProfileAppService(IRepository<IdentityUser, Guid> identityUserRepository, ICurrentUser currentUser, IdentityUserManager userManager, IRepository<Customer, Guid> customerRepository)
        {
            _identityUserRepository = identityUserRepository;
            _currentUser = currentUser;
            _userManager = userManager;
            _customerRepository = customerRepository;
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
            var user = await _userManager.GetByIdAsync(_currentUser.GetId()) as AppUser;
            if (user == null)
            {
                throw new UserFriendlyException("User not found.");
            }

            var profileDto = new UpdateProfileDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber
            };

            if (user.IsCustomer())
            {
                var customer = await _customerRepository.FindAsync(user.Customer.Id);
                if (customer != null)
                {
                    profileDto.ShippingAddress = customer.ShippingAddress;
                    profileDto.DateOfBirth = customer.DateOfBirth;
                    profileDto.Gender = customer.Gender;

                    profileDto.Name = customer.Name;
                    profileDto.Surname = customer.Surname;
                }
            }

            return profileDto;
        }

        public async Task<UpdateProfileDto> UpdateAsync(UpdateProfileDto input)
        {
            var user = await _userManager.GetByIdAsync(_currentUser.GetId()) as AppUser;
            if (user == null)
            {
                throw new UserFriendlyException("User not found.");
            }

            user.Name = input.Name;
            user.Surname = input.Surname;
            if (user.UserName != input.UserName) await _userManager.SetUserNameAsync(user, input.UserName);
            if (user.Email != input.Email) await _userManager.SetEmailAsync(user, input.Email);
            if (user.PhoneNumber != input.PhoneNumber) await _userManager.SetPhoneNumberAsync(user, input.PhoneNumber);

            await _userManager.UpdateAsync(user);

            if (user.IsCustomer())
            {
                var customer = await _customerRepository.GetAsync(user.Customer.Id);
                customer.UpdateProfile(
                    input.Name,
                    input.Surname,
                    input.PhoneNumber,
                    input.ShippingAddress,
                    input.DateOfBirth,
                    input.Gender
                );
                customer.Email = input.Email; // Sync email

                await _customerRepository.UpdateAsync(customer, autoSave: true);
            }

            return input;
        }
    }
}
