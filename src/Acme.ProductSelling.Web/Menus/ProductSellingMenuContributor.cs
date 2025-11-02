using Acme.ProductSelling.Localization;
using Acme.ProductSelling.MultiTenancy;
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

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<ProductSellingResource>();

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
        context.Menu.AddItem(new ApplicationMenuItem(
            ProductSellingMenus.Categories,
            l["Admin:Menu:Categories"],
            icon: "fa-solid fa-list",
            url: "/admin/categories"
        ));
        context.Menu.AddItem(new ApplicationMenuItem(
            ProductSellingMenus.Products,
            l["Admin:Menu:Products"],
            icon: "fa-solid fa-list",
            url: "/admin/products"
        ));
        context.Menu.AddItem(new ApplicationMenuItem(
            ProductSellingMenus.Manufacturers,
            l["Admin:Menu:Manufacturers"],
            icon: "fa-solid fa-list",
            url: "/admin/manufacturers"
        ));
        var ordersMenu = new ApplicationMenuItem(
            ProductSellingMenus.Orders,
            l["Admin:Menu:Orders"],
            icon: "fa-solid fa-list",
            url: "#"
        );

        ordersMenu.AddItem(new ApplicationMenuItem(
            name: "Admin.Orders.List",
            displayName: l["Admin:Menu:Orders:OrderList"],
            url: "/admin/orders",
            icon: "fa-solid fa-list-ul"
        ));

        ordersMenu.AddItem(new ApplicationMenuItem(
            ProductSellingMenus.DeletedOrders,
            l["Admin:Menu:Orders:DeletedOrders"],
            icon: "fa-solid fa-dumpster-fire",
            url: "/admin/orders/deleted"
        ));

        // 5. Add the fully constructed menu item to the main menu
        context.Menu.AddItem(ordersMenu);

        context.Menu.AddItem(new ApplicationMenuItem(
            ProductSellingMenus.Blogs,
            l["Admin:Menu:Blogs"],
            icon: "fa-solid fa-list",
            url: "/admin/blogs"
        ));

        void UpdateMenuItemUrls(ApplicationMenuItem menuItem)
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

        var administration = context.Menu.GetAdministration();
        administration.Order = 6;
        var identityMenu = administration.GetMenuItem(IdentityMenuNames.GroupName);
        if (identityMenu != null)
        {
            UpdateMenuItemUrls(identityMenu);

        }

        // Update Tenant Management menu items
        if (MultiTenancyConsts.IsEnabled)
        {
            var tenantMenu = administration.GetMenuItem(TenantManagementMenuNames.GroupName);
            if (tenantMenu != null)
            {
                UpdateMenuItemUrls(tenantMenu);

            }
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        var settingsMenu = administration.GetMenuItem(SettingManagementMenuNames.GroupName);
        if (settingsMenu != null)
        {
            UpdateMenuItemUrls(settingsMenu);

        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);

        return Task.CompletedTask;
    }


}
