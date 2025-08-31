using Acme.ProductSelling.Localization;
using Acme.ProductSelling.MultiTenancy;
using Acme.ProductSelling.Permissions;
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
        context.Menu.AddItem(new ApplicationMenuItem(
            ProductSellingMenus.Orders,
            l["Admin:Menu:Orders"],
            icon: "fa-solid fa-list",
            url: "/admin/orders"
        ));
        context.Menu.AddItem(new ApplicationMenuItem(
            ProductSellingMenus.Blogs,
            l["Admin:Menu:Blogs"],
            icon: "fa-solid fa-list",
            url: "/admin/blogs",
            order: 5,
            requiredPermissionName: ProductSellingPermissions.Blogs.Default
            
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
