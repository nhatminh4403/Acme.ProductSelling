using System.Threading.Tasks;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.MultiTenancy;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;

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
                l["Menu:Home"],
                "/admin",
                icon: "fa fa-home",
                order: 1
            )
        );
        context.Menu.AddItem(new ApplicationMenuItem(
            "Categories",
            l["Categories"],
            icon: "fa-solid fa-list",
            url: "/admin/categories"
        ));
        context.Menu.AddItem(new ApplicationMenuItem(
            "Products",
            l["Products"],
            icon: "fa-solid fa-list",
            url: "/admin/products"
        ));
        context.Menu.AddItem(new ApplicationMenuItem(
            "Manufacturers",
            l["Manufacturers"],
            icon: "fa-solid fa-list",
            url: "/admin/manufacturers"
        ));
        context.Menu.AddItem(new ApplicationMenuItem(
            "Orders",
            l["Orders"],
            icon: "fa-solid fa-list",
            url: "/admin/orders"
        ));
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;

        
        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 7);

        return Task.CompletedTask;
    }
}
