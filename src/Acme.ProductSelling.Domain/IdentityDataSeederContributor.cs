using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
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
        private readonly IPermissionDataSeeder _permissionDataSeeder;

        public IdentityDataSeederContributor(
            IdentityUserManager identityUserManager,
            ILookupNormalizer lookupNormalizer,
            IIdentityRoleRepository roleRepository,
            IIdentityUserRepository userRepository,
            IGuidGenerator guidGenerator,
            IPermissionDataSeeder permissionDataSeeder)
        {
            _identityUserManager = identityUserManager;
            _lookupNormalizer = lookupNormalizer;
            _identityRoleRepository = roleRepository;
            _identityUserRepository = userRepository;
            _guidGenerator = guidGenerator;
            _permissionDataSeeder = permissionDataSeeder;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            var adminRole = await CreateRoleIfNotExistsAsync(Acme.ProductSelling.Identity.IdentityRoleConsts.Admin);
            var managerRole = await CreateRoleIfNotExistsAsync(Acme.ProductSelling.Identity.IdentityRoleConsts.Manager);
            var editorRole = await CreateRoleIfNotExistsAsync(Acme.ProductSelling.Identity.IdentityRoleConsts.Blogger);
            var sellerRole = await CreateRoleIfNotExistsAsync(Acme.ProductSelling.Identity.IdentityRoleConsts.Seller);
            var cashierRole = await CreateRoleIfNotExistsAsync(Acme.ProductSelling.Identity.IdentityRoleConsts.Cashier);
            var warehouseRole = await CreateRoleIfNotExistsAsync(Acme.ProductSelling.Identity.IdentityRoleConsts.WarehouseStaff);

            var customerRole = await CreateRoleIfNotExistsAsync(Acme.ProductSelling.Identity.IdentityRoleConsts.Customer, isDefault: true);



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

            await CreateUserIfNotExistsAsync(
                username: "seller",
                email: "seller@abp.io",
                password: "123456",
                roleNameToAssign: sellerRole.Name
            );

            await CreateUserIfNotExistsAsync(
                username: "cashier",
                email: "cashier@abp.io",
                password: "123456",
                roleNameToAssign: cashierRole.Name
            );

            await CreateUserIfNotExistsAsync(
                username: "warehouse",
                email: "warehouse@abp.io",
                password: "123456",
                roleNameToAssign: warehouseRole.Name
            );

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
                ProductSellingPermissions.Orders.Default,   // View orders
                ProductSellingPermissions.Orders.Complete,  // Complete orders after payment
                ProductSellingPermissions.Orders.Edit,      // Update order status
            });
            await GrantPermissionsToRoleAsync(warehouseRole.Name, new[]
{
                ProductSellingPermissions.Orders.Default,       // View orders
                ProductSellingPermissions.Orders.ViewCompleted, // View completed orders
                ProductSellingPermissions.Orders.Fulfill,       // Mark orders as fulfilled/delivered
                ProductSellingPermissions.Products.Default,     // View products to pick items
            });
            await GrantPermissionsToRoleAsync(sellerRole.Name, new[]
            {
                ProductSellingPermissions.Products.Default, // View products
                ProductSellingPermissions.Orders.Default,   // View orders
                ProductSellingPermissions.Orders.Create,    // Create orders for customers
                ProductSellingPermissions.Orders.Edit,      // Edit pending orders
            });
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
