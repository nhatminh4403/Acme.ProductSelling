using Acme.ProductSelling.Identity;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.MultiTenancy;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace Acme.ProductSelling.Web.Menus;

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
        var authorizationService = context.ServiceProvider.GetRequiredService<IAuthorizationService>();

        //Home
        context.Menu.AddItem(
            new ApplicationMenuItem(
                ProductSellingMenus.Home,
                l["Admin:Menu:Home"],
                "/admin",
                icon: "fa fa-home",
                order: 1
            )
        );

        if (await authorizationService.IsGrantedAsync(ProductSellingPermissions.Categories.Default))
        {
            context.Menu.AddItem(new ApplicationMenuItem(
                ProductSellingMenus.Categories,
                l["Admin:Menu:Categories"],
                icon: "fa-solid fa-list",
                url: "/admin/categories"
            ));
        }
        if (await authorizationService.IsGrantedAsync(ProductSellingPermissions.Products.Default))
        {
            context.Menu.AddItem(new ApplicationMenuItem(
                ProductSellingMenus.Products,
                l["Admin:Menu:Products"],
                icon: "fa-solid fa-list",
                url: "/admin/products"
            ));
        }
        if (await authorizationService.IsGrantedAsync(ProductSellingPermissions.Manufacturers.Default))
        {
            context.Menu.AddItem(new ApplicationMenuItem(
                ProductSellingMenus.Manufacturers,
                l["Admin:Menu:Manufacturers"],
                icon: "fa-solid fa-list",
                url: "/admin/manufacturers"
            ));
        }

        if (await authorizationService.IsGrantedAsync(ProductSellingPermissions.Orders.Default))
        {
            var ordersMenu = new ApplicationMenuItem(
                ProductSellingMenus.Orders,
                l["Admin:Menu:Orders"],
                icon: "fa-solid fa-list" // No URL for a parent menu
            );

            if (await authorizationService.IsGrantedAsync(ProductSellingPermissions.Orders.Default))
            {
                ordersMenu.AddItem(new ApplicationMenuItem(
                    name: "Admin.Orders.List",
                    displayName: l["Admin:Menu:Orders:OrderList"],
                    url: "/admin/orders",
                    icon: "fa-solid fa-list-ul"
                ));
            }

            if (await authorizationService.IsGrantedAsync(ProductSellingPermissions.Orders.ViewAll) || await authorizationService.IsGrantedAsync(ProductSellingPermissions.Orders.ViewCompleted)) // ViewAll is a good permission for this
            {
                ordersMenu.AddItem(new ApplicationMenuItem(
                   ProductSellingMenus.DeletedOrders,
                   l["Admin:Menu:Orders:DeletedOrders"],
                   icon: "fa-solid fa-dumpster-fire",
                   url: "/admin/orders/deleted"
               ));
            }

            context.Menu.AddItem(ordersMenu);
        }

        if (await authorizationService.IsGrantedAsync(ProductSellingPermissions.Blogs.Default))
        {
            context.Menu.AddItem(new ApplicationMenuItem(
                ProductSellingMenus.Blogs,
                l["Admin:Menu:Blogs"],
                icon: "fa-solid fa-list",
                url: "/admin/blogs"
            ));
        }


        var administration = context.Menu.GetAdministration();
        if (await authorizationService.IsGrantedAsync(Acme.ProductSelling.Identity.IdentityRoleConsts.Admin) ||
            await authorizationService.IsGrantedAsync(Acme.ProductSelling.Identity.IdentityRoleConsts.Manager))
        {
            administration.Order = 6;

            var identityMenu = administration.GetMenuItem(IdentityMenuNames.GroupName);
            if (identityMenu != null) { UpdateMenuItemUrls(identityMenu); }

            if (MultiTenancyConsts.IsEnabled)
            {
                var tenantMenu = administration.GetMenuItem(TenantManagementMenuNames.GroupName);
                if (tenantMenu != null) { UpdateMenuItemUrls(tenantMenu); }
                administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
            }
            else
            {
                administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
            }

            var settingsMenu = administration.GetMenuItem(SettingManagementMenuNames.GroupName);
            if (settingsMenu != null) { UpdateMenuItemUrls(settingsMenu); }

            administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);
            administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);
        }
        else
        {
            // If the user is not an admin or manager, remove the entire Administration menu
            context.Menu.TryRemoveMenuItem(administration.Name);
        }
    }
    private static void UpdateMenuItemUrls(ApplicationMenuItem menuItem)
    {
        if (!string.IsNullOrEmpty(menuItem.Url))
        {
            var cleanUrl = menuItem.Url.TrimStart('~');
            menuItem.Url = "/admin" + cleanUrl;
        }
        foreach (var item in menuItem.Items)
        {
            UpdateMenuItemUrls(item);
        }
    }

}
