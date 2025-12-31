using Acme.ProductSelling.Data.BaseSeeder;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace Acme.ProductSelling.Data.Identity
{
    public class IdentityDataSeederContributor : IDataSeederContributor
    {
        private readonly IdentityUserManager _identityUserManager;
        private readonly ILookupNormalizer _lookupNormalizer;
        private readonly IIdentityRoleRepository _identityRoleRepository;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IPermissionDataSeeder _permissionDataSeeder;
        private readonly IGuidGenerator _guidGenerator;

        public IdentityDataSeederContributor(
            IdentityUserManager identityUserManager,
            ILookupNormalizer lookupNormalizer,
            IIdentityRoleRepository identityRoleRepository,
            IIdentityUserRepository identityUserRepository,
            IPermissionDataSeeder permissionDataSeeder,
            IGuidGenerator guidGenerator)
        {
            _identityUserManager = identityUserManager;
            _lookupNormalizer = lookupNormalizer;
            _identityRoleRepository = identityRoleRepository;
            _identityUserRepository = identityUserRepository;
            _permissionDataSeeder = permissionDataSeeder;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync()
        {
            // Create roles
            var adminRole = await CreateRoleIfNotExistsAsync(
                Acme.ProductSelling.Identity.IdentityRoleConsts.Admin);
            var managerRole = await CreateRoleIfNotExistsAsync(
                Acme.ProductSelling.Identity.IdentityRoleConsts.Manager);
            var editorRole = await CreateRoleIfNotExistsAsync(
                Acme.ProductSelling.Identity.IdentityRoleConsts.Blogger);
            var sellerRole = await CreateRoleIfNotExistsAsync(
                Acme.ProductSelling.Identity.IdentityRoleConsts.Seller);
            var cashierRole = await CreateRoleIfNotExistsAsync(
                Acme.ProductSelling.Identity.IdentityRoleConsts.Cashier);
            var warehouseRole = await CreateRoleIfNotExistsAsync(
                Acme.ProductSelling.Identity.IdentityRoleConsts.WarehouseStaff);
            var customerRole = await CreateRoleIfNotExistsAsync(
                Acme.ProductSelling.Identity.IdentityRoleConsts.Customer, isDefault: true);

            // Create users (NOTE: Store IDs should be passed from StoreSeeder)
            await CreateUserIfNotExistsAsync("manager", "manager@abp.io", "123456", managerRole.Name);
            await CreateUserIfNotExistsAsync("editor", "blogger@abp.io", "123456", editorRole.Name);

            // Grant permissions to roles
            await GrantPermissionsToRoleAsync(managerRole.Name, new[]
            {
                ProductSellingPermissions.Blogs.Default,
                ProductSellingPermissions.Blogs.Create,
                ProductSellingPermissions.Blogs.Edit,
                ProductSellingPermissions.Orders.Default,
                ProductSellingPermissions.Orders.ViewAll,
                ProductSellingPermissions.Products.Default,
                ProductSellingPermissions.Products.Create,
                ProductSellingPermissions.Products.Edit,
            });

            await GrantPermissionsToRoleAsync(editorRole.Name, new[]
            {
                ProductSellingPermissions.Blogs.Default,
                ProductSellingPermissions.Blogs.Edit,
                ProductSellingPermissions.Blogs.Delete,
                ProductSellingPermissions.Blogs.Create
            });

            await GrantPermissionsToRoleAsync(cashierRole.Name, new[]
            {
                ProductSellingPermissions.Orders.Default,
                ProductSellingPermissions.Orders.Complete,
                ProductSellingPermissions.Orders.Edit,
            });

            await GrantPermissionsToRoleAsync(warehouseRole.Name, new[]
            {
                ProductSellingPermissions.Orders.Default,
                ProductSellingPermissions.Orders.ViewCompleted,
                ProductSellingPermissions.Orders.Fulfill,
                ProductSellingPermissions.Products.Default,
            });

            await GrantPermissionsToRoleAsync(sellerRole.Name, new[]
            {
                ProductSellingPermissions.Products.Default,
                ProductSellingPermissions.Orders.Default,
                ProductSellingPermissions.Orders.Create,
                ProductSellingPermissions.Orders.Edit,
            });

            await GrantPermissionsToRoleAsync(customerRole.Name, new[]
            {
                ProductSellingPermissions.Products.Default,
                ProductSellingPermissions.Orders.Default,
                ProductSellingPermissions.Orders.Create,
                ProductSellingPermissions.Orders.Edit,
                ProductSellingPermissions.Carts.Default,
                ProductSellingPermissions.Carts.Delete,
                ProductSellingPermissions.Comments.Create,
                ProductSellingPermissions.Comments.Delete,
            });
        }

        private async Task<Volo.Abp.Identity.IdentityRole> CreateRoleIfNotExistsAsync(
            string roleName,
            bool isDefault = false,
            bool isPublic = false)
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

        private async Task<Volo.Abp.Identity.IdentityUser> CreateUserIfNotExistsAsync(
            string username,
            string email,
            string password,
            string roleNameToAssign = null,
            Guid? storeId = null)
        {
            var normalizedUserName = _lookupNormalizer.NormalizeName(username);
            var existingUser = await _identityUserRepository.FindByNormalizedUserNameAsync(normalizedUserName);
            if (existingUser != null)
            {
                return existingUser;
            }

            var user = new AppUser(_guidGenerator.Create(), username, email)
            {
                Name = username,
            };

            if (storeId.HasValue)
            {
                user.AssignToStore(storeId.Value);
            }

            var creationResult = await _identityUserManager.CreateAsync(user, password);
            if (!creationResult.Succeeded)
            {
                var errorDetails = string.Join(", ", creationResult.Errors.Select(e => e.Description));
                throw new Exception($"Cannot create user '{username}'. Reason: {errorDetails}");
            }

            if (!string.IsNullOrWhiteSpace(roleNameToAssign))
            {
                var roleResult = await _identityUserManager.AddToRoleAsync(user, roleNameToAssign);
                if (!roleResult.Succeeded)
                {
                    var errorDetails = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Cannot assign role '{roleNameToAssign}' to user '{username}'. Reason: {errorDetails}");
                }
            }

            return user;
        }

        private async Task GrantPermissionsToRoleAsync(string roleName, string[] permissions)
        {
            await _permissionDataSeeder.SeedAsync(
                RolePermissionValueProvider.ProviderName,
                roleName,
                permissions
            );
        }
    }
}

