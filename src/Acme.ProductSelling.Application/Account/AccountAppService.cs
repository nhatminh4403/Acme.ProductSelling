using Acme.ProductSelling.Users;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Acme.ProductSelling.Account
{
    public class AccountAppService : ApplicationService, IAccountAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IRepository<Customer, Guid> _customerRepository;

        public AccountAppService(
            IdentityUserManager userManager,
            IIdentityUserRepository userRepository,
            IRepository<Customer, Guid> customerRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
        }

        public async Task<RolePrefixDto> GetRolePrefixAsync()
        {
            var result = new RolePrefixDto
            {
                Prefix = "admin",
                HasAdminAccess = false
            };
            if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
            {
                return result;
            }
            var user = await _userRepository.GetAsync(CurrentUser.Id.Value);
            var roles = await _userRepository.GetRolesAsync(user.Id);
            var roleNames = roles.Select(r => r.Name).ToList();
            result.HasAdminAccess = roleNames.Any(r =>
                           r.Equals(Acme.ProductSelling.Identity.IdentityRoleConsts.Admin, StringComparison.OrdinalIgnoreCase) ||
                           r.Equals(Acme.ProductSelling.Identity.IdentityRoleConsts.Blogger, StringComparison.OrdinalIgnoreCase) ||
                           r.Equals(Acme.ProductSelling.Identity.IdentityRoleConsts.Manager, StringComparison.OrdinalIgnoreCase) ||
                           r.Equals(Acme.ProductSelling.Identity.IdentityRoleConsts.Seller, StringComparison.OrdinalIgnoreCase) ||
                           r.Equals(Acme.ProductSelling.Identity.IdentityRoleConsts.Cashier, StringComparison.OrdinalIgnoreCase) ||
                           r.Equals(Acme.ProductSelling.Identity.IdentityRoleConsts.WarehouseStaff, StringComparison.OrdinalIgnoreCase)
                       );
            if (roleNames.Any(r => r.Equals(Acme.ProductSelling.Identity.IdentityRoleConsts.Blogger, StringComparison.OrdinalIgnoreCase)))
            {
                result.Prefix = "blogger";
            }
            else if (roleNames.Any(r => r.Equals(Acme.ProductSelling.Identity.IdentityRoleConsts.Manager, StringComparison.OrdinalIgnoreCase)))
            {
                result.Prefix = "manager";
            }
            else if (roleNames.Any(r => r.Equals(Acme.ProductSelling.Identity.IdentityRoleConsts.Seller, StringComparison.OrdinalIgnoreCase)))
            {
                result.Prefix = "seller";
            }
            else if (roleNames.Any(r => r.Equals(Acme.ProductSelling.Identity.IdentityRoleConsts.Cashier, StringComparison.OrdinalIgnoreCase)))
            {
                result.Prefix = "cashier";
            }
            else if (roleNames.Any(r => r.Equals(Acme.ProductSelling.Identity.IdentityRoleConsts.WarehouseStaff, StringComparison.OrdinalIgnoreCase)))
            {
                result.Prefix = "warehouse";
            }
            // Default is "admin" (already set)

            return result;
        }

        public async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
        {
            if (await _userRepository.FindByNormalizedUserNameAsync(_userManager.NormalizeName(input.UserName)) != null)
            {
                throw new UserFriendlyException("Username already exists!");
            }
            if (await _userRepository.FindByNormalizedEmailAsync(_userManager.NormalizeEmail(input.EmailAddress)) != null)
            {
                throw new UserFriendlyException("Email already exists!");
            }

            // Create new user
            var user = new AppUser( // Use your custom AppUser
                            GuidGenerator.Create(),
                            input.UserName,
                            input.EmailAddress,
                            CurrentTenant.Id
                        );
            user.Name = input.Name;
            user.Surname = input.Surname;

            var result = await _userManager.CreateAsync(user, input.Password);
            if (!result.Succeeded)
            {
                throw new UserFriendlyException(result.Errors.Select(e => e.Description).JoinAsString(", "));
            }

            // 2. Create the customer domain entity
            var customer = new Customer(
                GuidGenerator.Create(),
                user.Id, // Link to AppUser
                input.Name,
                input.Surname,
                user.Email,
                phoneNumber: null,
                shippingAddress: null, // Initially null
                dateOfBirth: null,
                gender: UserGender.NONE
            );

            await _customerRepository.InsertAsync(customer, autoSave: true);

            // 3. Link the AppUser to the new Customer record
            user.SetCustomer(customer);
            await _userManager.UpdateAsync(user);

            // 4. Assign the "Customer" role
            await _userManager.AddToRoleAsync(user, Acme.ProductSelling.Identity.IdentityRoleConsts.Customer);
            Logger.LogInformation(
                "New customer registered: {UserId} ({UserName}), Customer: {CustomerId}",
                user.Id, user.UserName, customer.Id
            );
            return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
        }
    }
}
