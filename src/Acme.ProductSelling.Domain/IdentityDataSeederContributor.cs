using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
//using Acme.ProductSelling.;

namespace Acme.ProductSelling
{
    public class IdentityDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IdentityUserManager _identityUserManager;
        private readonly ILookupNormalizer _lookupNormalizer;
        private readonly IIdentityRoleRepository _identityRoleRepository;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IGuidGenerator _guidGenerator;

        public IdentityDataSeederContributor(IdentityUserManager identityUserManager,
            ILookupNormalizer lookupNormalizer, IIdentityRoleRepository roleRepository,
            IIdentityUserRepository userRepository, IGuidGenerator guidGenerator)
        {
            _identityUserManager = identityUserManager;
            _lookupNormalizer = lookupNormalizer;
            _identityRoleRepository = roleRepository;
            _identityUserRepository = userRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            var adminRole = await CreateRoleIfNotExistsAsync(Acme.ProductSelling.Identity.IdentityRoleConsts.Admin);
            var managerRole = await CreateRoleIfNotExistsAsync(Acme.ProductSelling.Identity.IdentityRoleConsts.Manager);
            var editorRole = await CreateRoleIfNotExistsAsync(Acme.ProductSelling.Identity.IdentityRoleConsts.Blogger);

            await CreateUserIfNotExistsAsync(
                username: "manager",
                email: "manager@abp.io",
                password: "123456",
                roleNameToAssign: managerRole.Name
            //ProductSellingPermissions.Blogs.Default
            );


            await CreateUserIfNotExistsAsync(
                username: "editor",
                email: "blogger@abp.io",
                password: "123456",
                roleNameToAssign: editorRole.Name/*,
                ProductSellingPermissions.Blogs.Default*/
            );
        }



        private async Task<Volo.Abp.Identity.IdentityRole> CreateRoleIfNotExistsAsync(string roleName, bool isDefault = false, bool isPublic = false)
        {
            var normalizedRoleName = _lookupNormalizer.NormalizeName(roleName);
            var existingRole = await _identityRoleRepository.FindByNormalizedNameAsync(normalizedRoleName);
            if (existingRole != null) return existingRole;

            var role = new Volo.Abp.Identity.IdentityRole(_guidGenerator.Create(), roleName)
            {
                IsDefault = isDefault,
                IsPublic = isPublic
            };
            role.ChangeName(roleName);

            await _identityRoleRepository.InsertAsync(role);
            return role;
        }
        private async Task<Volo.Abp.Identity.IdentityUser> CreateUserIfNotExistsAsync(string username, string email, string password, string roleNameToAssign = null)
        {

            var normalizedUserName = _lookupNormalizer.NormalizeName(username);
            var existingUser = await _identityUserRepository.FindByNormalizedUserNameAsync(normalizedUserName);
            if (existingUser != null)
            {
                return existingUser;
            }

            var user = new Volo.Abp.Identity.IdentityUser(
                _guidGenerator.Create(),
                username,
                email
            )
            {
                Name = username,
            };
            var creationResult = await _identityUserManager.CreateAsync(user, password);
            if (!creationResult.Succeeded)
            {
                var errorDetails = string.Join(", ", creationResult.Errors.Select(e => e.Description));
                throw new Exception($"Không thể tạo người dùng '{username}'. Lý do: {errorDetails}");
            }

            if (!string.IsNullOrWhiteSpace(roleNameToAssign))
            {
                var roleResult = await _identityUserManager.AddToRoleAsync(user, roleNameToAssign);
                if (!roleResult.Succeeded)
                {
                    var errorDetails = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Không thể gán vai trò '{roleNameToAssign}' cho người dùng '{username}'. Lý do: {errorDetails}");
                }
            }

            return user;
        }
    }
}
