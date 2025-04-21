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

        //Define your own permissions here. Example:
        //myGroup.AddPermission(ProductSellingPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ProductSellingResource>(name);
    }
}
