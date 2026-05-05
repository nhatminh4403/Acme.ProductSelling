using Acme.ProductSelling.Account.Dtos;
using Acme.ProductSelling.Account.Services;
using Acme.ProductSelling.Users;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class ProfileAppService : ApplicationService, IProfileAppService, ITransientDependency
    {
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IdentityUserManager _userManager;
        private readonly ICustomerRepository _customerRepository;

        public ProfileAppService(IRepository<IdentityUser, Guid> identityUserRepository,
            ICurrentUser currentUser, IdentityUserManager userManager, ICustomerRepository customerRepository)
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

        public async Task<CustomerProfileDto> GetAsync()
        {
            var user = await _userManager.GetByIdAsync(_currentUser.GetId()) as AppUser;
            if (user == null)
            {
                throw new UserFriendlyException("User not found.");
            }

            var customerdto = new CustomerProfileDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber
            };

            var customer = await _customerRepository.FindAsync(
                   c => c.AppUserId == _currentUser.GetId());

            if (customer != null)
            {
                customerdto.DateOfBirth = customer.DateOfBirth;
                customerdto.Gender = customer.Gender;
                customerdto.Name = customer.Name;
                customerdto.Surname = customer.Surname;
            }

            return customerdto;
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

            var customer = await _customerRepository.FindAsync(c => c.AppUserId == user.Id);

            if (customer != null)
            {
                customer.UpdateProfile(
                    input.Name,
                    input.Surname,
                    input.PhoneNumber,
                    input.DateOfBirth,
                    input.Gender
                );
                customer.Email = input.Email; // Sync email

                await _customerRepository.UpdateAsync(customer, autoSave: true);
            }

            return input;
        }
        //Customer
        public async Task UpdateShippingAddressAsync(UpdateShippingAddressDto input)
        {
            var customer = await _customerRepository.FindAsync(c => c.AppUserId == _currentUser.GetId());

            if (customer == null)
                throw new UserFriendlyException("Only customers have a shipping address.");

            customer.UpdateProfile(
                customer.Name,
                customer.Surname,
                customer.PhoneNumber,
                customer.DateOfBirth,
                customer.Gender
            );
            await _customerRepository.UpdateAsync(customer, autoSave: true);
        }
    }
}
