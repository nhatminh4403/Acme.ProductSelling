using Acme.ProductSelling.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Acme.ProductSelling.Permissions;

public class ProductSellingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ProductSellingPermissions.GroupName);

        var productsPermission = myGroup.AddPermission(ProductSellingPermissions.Products.Default, L("Permission:Products"));
        productsPermission.AddChild(ProductSellingPermissions.Products.Create, L("Permission:Products.Create"));
        productsPermission.AddChild(ProductSellingPermissions.Products.Edit, L("Permission:Products.Edit"));
        productsPermission.AddChild(ProductSellingPermissions.Products.Delete, L("Permission:Products.Delete"));


        var categoriesPermission = myGroup.AddPermission(ProductSellingPermissions.Categories.Default, L("Permission:Categories"));
        categoriesPermission.AddChild(ProductSellingPermissions.Categories.Create, L("Permission:Categories.Create"));
        categoriesPermission.AddChild(ProductSellingPermissions.Categories.Edit, L("Permission:Categories.Edit"));
        categoriesPermission.AddChild(ProductSellingPermissions.Categories.Delete, L("Permission:Categories.Delete"));

        var orderPermission = myGroup.AddPermission(ProductSellingPermissions.Orders.Default, L("Permission:Orders"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.Create, L("Permission:Orders.Create"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.Edit, L("Permission:Orders.Edit"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.Delete, L("Permission:Orders.Delete"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ProductSellingResource>(name);
    }
}
