// Acme.ProductSelling.Application/Account/AccountAppService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace Acme.ProductSelling.Account
{
    public class AccountAppService : ApplicationService, IAccountAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IIdentityUserRepository _userRepository;

        public AccountAppService(
            IdentityUserManager userManager,
            IIdentityUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
        {
            // Validate if username already exists
            var existingUserByUsername = await _userRepository.FindByNormalizedUserNameAsync(
                _userManager.NormalizeName(input.UserName)
            );

            if (existingUserByUsername != null)
            {
                throw new UserFriendlyException("Username already exists!");
            }

            // Validate if email already exists
            var existingUserByEmail = await _userRepository.FindByNormalizedEmailAsync(
                _userManager.NormalizeEmail(input.EmailAddress)
            );

            if (existingUserByEmail != null)
            {
                throw new UserFriendlyException("Email already exists!");
            }

            // Create new user
            var user = new Volo.Abp.Identity.IdentityUser(
                GuidGenerator.Create(),
                input.UserName,
                input.EmailAddress,
                CurrentTenant.Id
            );

            // Set name and surname
            user.Name = input.Name;
            user.Surname = input.Surname;

            // Create user with password
            var result = await _userManager.CreateAsync(user, input.Password);

            if (!result.Succeeded)
            {
                throw new UserFriendlyException(
                    result.Errors
                        .Select(e => e.Description)
                        .JoinAsString(", ")
                );
            }

            // Assign default role (optional)
            await _userManager.AddToRoleAsync(user, "Customer"); // or any default role

            return ObjectMapper.Map<Volo.Abp.Identity.IdentityUser, IdentityUserDto>(user);
        }
    }
}