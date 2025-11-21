using Acme.ProductSelling.Localization;
using Acme.ProductSelling.MultiTenancy;
using Acme.ProductSelling.Permissions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Web.Menus
{
    public class ProductSellingMenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if (context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
        }

        private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            var l = context.GetLocalizer<ProductSellingResource>();
            var permissionChecker = context.ServiceProvider.GetRequiredService<IPermissionChecker>();
            var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

            var prefix = await GetRoleBasedPrefixAsync(context.ServiceProvider, currentUser);

            // Home - Visible to all authenticated users
            context.Menu.AddItem(
                new ApplicationMenuItem(
                    ProductSellingMenus.Home,
                    l["Admin:Menu:Home"],
                    $"/{prefix}",
                    icon: "fa fa-home",
                    order: 1
                )
            );

            // Stores - NEW
            if (await permissionChecker.IsGrantedAsync(ProductSellingPermissions.Stores.Default))
            {
                context.Menu.AddItem(new ApplicationMenuItem(
                    ProductSellingMenus.Stores,
                    l["Admin:Menu:Stores"],
                    icon: "fa-solid fa-store",
                    url: $"/{prefix}/stores",
                    order: 2
                ));
            }

            // Categories
            if (await permissionChecker.IsGrantedAsync(ProductSellingPermissions.Categories.Default))
            {
                context.Menu.AddItem(new ApplicationMenuItem(
                    ProductSellingMenus.Categories,
                    l["Admin:Menu:Categories"],
                    icon: "fa-solid fa-list",
                    url: $"/{prefix}/categories",
                    order: 3
                ));
            }

            // Manufacturers
            if (await permissionChecker.IsGrantedAsync(ProductSellingPermissions.Manufacturers.Default))
            {
                context.Menu.AddItem(new ApplicationMenuItem(
                    ProductSellingMenus.Manufacturers,
                    l["Admin:Menu:Manufacturers"],
                    icon: "fa-solid fa-industry",
                    url: $"/{prefix}/manufacturers",
                    order: 4
                ));
            }

            // Products
            if (await permissionChecker.IsGrantedAsync(ProductSellingPermissions.Products.Default))
            {
                var productsMenu = new ApplicationMenuItem(
                    ProductSellingMenus.Products,
                    l["Admin:Menu:Products"],
                    icon: "fa-solid fa-box",
                    order: 5
                );

                productsMenu.AddItem(new ApplicationMenuItem(
                    "Admin.Products.List",
                    l["Admin:Menu:Products:ProductList"],
                    url: $"/{prefix}/products",
                    icon: "fa-solid fa-list-ul"
                ));

                // Store Inventory submenu
                if (await permissionChecker.IsGrantedAsync(ProductSellingPermissions.Products.Default))
                {
                    productsMenu.AddItem(new ApplicationMenuItem(
                        "Admin.Products.StoreInventory",
                        l["Admin:Menu:Products:StoreInventory"],
                        url: $"/{prefix}/store-inventory",
                        icon: "fa-solid fa-warehouse"
                    ));
                }
                productsMenu.AddItem(new ApplicationMenuItem(
                    "Admin.Products.LowStock",
                    l["Admin:Menu:Products:LowStock"],
                    url: $"/{prefix}/store-inventory/low-stock",
                    icon: "fa-solid fa-exclamation-triangle",
                    order: 3
                ));
                context.Menu.AddItem(productsMenu);
            }

            // Orders
            if (await permissionChecker.IsGrantedAsync(ProductSellingPermissions.Orders.Default))
            {
                var ordersMenu = new ApplicationMenuItem(
                    ProductSellingMenus.Orders,
                    l["Admin:Menu:Orders"],
                    icon: "fa-solid fa-shopping-cart",
                    order: 6
                );

                // All Orders
                ordersMenu.AddItem(new ApplicationMenuItem(
                    "Admin.Orders.List",
                    l["Admin:Menu:Orders:OrderList"],
                    url: $"/{prefix}/orders",
                    icon: "fa-solid fa-list-ul"
                ));

                // In-Store Orders (for store staff)
                if (await permissionChecker.IsGrantedAsync(ProductSellingPermissions.Orders.Create))
                {
                    ordersMenu.AddItem(new ApplicationMenuItem(
                        "Admin.Orders.InStore",
                        l["Admin:Menu:Orders:InStoreOrders"],
                        url: $"/{prefix}/orders/in-store",
                        icon: "fa-solid fa-store"
                    ));
                    ordersMenu.AddItem(new ApplicationMenuItem(
                       "Admin.Orders.CreateInStore",
                       l["Menu:CreateInStoreOrder"],
                       url: $"/{prefix}/orders/in-store/create",
                       icon: "fas fa-plus-circle"
                   ));

                    ordersMenu.AddItem(new ApplicationMenuItem(
                        "Admin.Orders.MyOrders",
                        l["Menu:MyOrders"],
                        url: $"/{prefix}/orders/in-store",
                        icon: "fas fa-receipt"
                    ));
                }

                // Pending Payment (for cashiers)
                if (await permissionChecker.IsGrantedAsync(ProductSellingPermissions.Orders.Complete))
                {
                    ordersMenu.AddItem(new ApplicationMenuItem(
                        "Admin.Orders.PendingPayment",
                        l["Admin:Menu:Orders:PendingPayment"],
                        url: $"/{prefix}/orders/pending-payment",
                        icon: "fa-solid fa-cash-register"
                    ));
                }

                // Ready for Fulfillment (for warehouse staff)
                if (await permissionChecker.IsGrantedAsync(ProductSellingPermissions.Orders.Fulfill))
                {
                    ordersMenu.AddItem(new ApplicationMenuItem(
                        "Admin.Orders.Fulfillment",
                        l["Admin:Menu:Orders:Fulfillment"],
                        url: $"/{prefix}/orders/fulfillment",
                        icon: "fa-solid fa-box-open"
                    ));
                }

                // Deleted Orders (admin only)
                if (await permissionChecker.IsGrantedAsync(ProductSellingPermissions.Orders.ViewAll))
                {
                    ordersMenu.AddItem(new ApplicationMenuItem(
                        ProductSellingMenus.DeletedOrders,
                        l["Admin:Menu:Orders:DeletedOrders"],
                        icon: "fa-solid fa-trash-restore",
                        url: $"/{prefix}/orders/deleted"
                    ));
                }

                context.Menu.AddItem(ordersMenu);
            }

            // Blogs
            if (await permissionChecker.IsGrantedAsync(ProductSellingPermissions.Blogs.Default))
            {
                context.Menu.AddItem(new ApplicationMenuItem(
                    ProductSellingMenus.Blogs,
                    l["Admin:Menu:Blogs"],
                    icon: "fa-solid fa-blog",
                    url: $"/{prefix}/blogs",
                    order: 7
                ));
            }
            // User Management (Admin/Manager only)
            if (await permissionChecker.IsGrantedAsync(ProductSellingPermissions.Users.Default))
            {
                context.Menu.AddItem(new ApplicationMenuItem(
                    ProductSellingMenus.Users,
                    l["Menu:Users"],
                    icon: "fas fa-users",
                    url: $"/{prefix}/users",
                    order: 5
                ));
            }
            // Reports (admin/manager only)
            if (await permissionChecker.IsGrantedAsync(ProductSellingPermissions.Orders.ViewAll))
            {
                var reportsMenu = new ApplicationMenuItem(
                    "Admin.Reports",
                    l["Admin:Menu:Reports"],
                    icon: "fa-solid fa-chart-line",
                    order: 8
                );

                reportsMenu.AddItem(new ApplicationMenuItem(
                    "Admin.Reports.Sales",
                    l["Admin:Menu:Reports:Sales"],
                    url: $"/{prefix}/reports/sales",
                    icon: "fa-solid fa-dollar-sign"
                ));

                reportsMenu.AddItem(new ApplicationMenuItem(
                    "Admin.Reports.Inventory",
                    l["Admin:Menu:Reports:Inventory"],
                    url: $"/{prefix}/reports/inventory",
                    icon: "fa-solid fa-boxes"
                ));

                context.Menu.AddItem(reportsMenu);
            }

            // Administration Menu - Use permissions instead of roles
            await ConfigureAdministrationMenuAsync(context, prefix);
        }
        private static async Task ConfigureAdministrationMenuAsync(MenuConfigurationContext context, string prefix)
        {
            var administration = context.Menu.GetAdministration();
            var permissionChecker = context.ServiceProvider.GetRequiredService<IPermissionChecker>();

            administration.Order = 100; // Show at bottom

            // Check if user has any admin permissions
            var hasIdentityPermission = await permissionChecker.IsGrantedAsync("AbpIdentity.Users");
            var hasSettingsPermission = await permissionChecker.IsGrantedAsync("SettingManagement.Emailing");

            if (!hasIdentityPermission && !hasSettingsPermission)
            {
                context.Menu.TryRemoveMenuItem(administration.Name);
            }
            else
            {
                UpdateMenuItemUrls(administration, prefix);

                if (!MultiTenancyConsts.IsEnabled)
                {
                    administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
                }

                administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);
                administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);
            }
        }
        private static async Task<string> GetRoleBasedPrefixAsync(IServiceProvider serviceProvider, ICurrentUser currentUser)
        {
            if (!currentUser.IsAuthenticated || !currentUser.Id.HasValue)
                return "admin";

            var userRepository = serviceProvider.GetRequiredService<IIdentityUserRepository>();
            var user = await userRepository.GetAsync(currentUser.Id.Value);
            var roles = await userRepository.GetRolesAsync(user.Id);
            if (roles.Any(r => string.Equals(r.Name, Acme.ProductSelling.Identity.IdentityRoleConsts.Blogger, System.StringComparison.OrdinalIgnoreCase)))
                return "blogger";
            if (roles.Any(r => string.Equals(r.Name, Acme.ProductSelling.Identity.IdentityRoleConsts.Manager, System.StringComparison.OrdinalIgnoreCase)))
                return "manager";
            if (roles.Any(r => string.Equals(r.Name, Acme.ProductSelling.Identity.IdentityRoleConsts.Seller, System.StringComparison.OrdinalIgnoreCase)))
                return "seller";
            if (roles.Any(r => string.Equals(r.Name, Acme.ProductSelling.Identity.IdentityRoleConsts.Cashier, System.StringComparison.OrdinalIgnoreCase)))
                return "cashier";
            if (roles.Any(r => string.Equals(r.Name, Acme.ProductSelling.Identity.IdentityRoleConsts.WarehouseStaff, System.StringComparison.OrdinalIgnoreCase)))
                return "warehouse";
            return "admin";
        }
        private static void UpdateMenuItemUrls(ApplicationMenuItem menuItem, string prefix)
        {
            if (!string.IsNullOrEmpty(menuItem.Url))
            {
                var cleanUrl = menuItem.Url.TrimStart('~');
                menuItem.Url = $"/{prefix}" + cleanUrl;
            }

            foreach (var item in menuItem.Items)
            {
                UpdateMenuItemUrls(item, prefix);
            }
        }
    }
}