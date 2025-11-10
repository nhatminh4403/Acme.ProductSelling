using Acme.ProductSelling.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
namespace Acme.ProductSelling.Permissions;
public class ProductSellingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ProductSellingPermissions.GroupName);

        #region Product Permissions
        var productsPermission = myGroup.AddPermission(ProductSellingPermissions.Products.Default, L("Permission:Products"));
        productsPermission.AddChild(ProductSellingPermissions.Products.Create, L("Permission:Products.Create"));
        productsPermission.AddChild(ProductSellingPermissions.Products.Edit, L("Permission:Products.Edit"));
        productsPermission.AddChild(ProductSellingPermissions.Products.Delete, L("Permission:Products.Delete"));
        #endregion
        #region Category Permissions
        var categoriesPermission = myGroup.AddPermission(ProductSellingPermissions.Categories.Default, L("Permission:Categories"));
        categoriesPermission.AddChild(ProductSellingPermissions.Categories.Create, L("Permission:Categories.Create"));
        categoriesPermission.AddChild(ProductSellingPermissions.Categories.Edit, L("Permission:Categories.Edit"));
        categoriesPermission.AddChild(ProductSellingPermissions.Categories.Delete, L("Permission:Categories.Delete"));
        #endregion

        #region Order Permissions
        var orderPermission = myGroup.AddPermission(ProductSellingPermissions.Orders.Default, L("Permission:Orders"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.Create, L("Permission:Orders.Create"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.Edit, L("Permission:Orders.Edit"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.Delete, L("Permission:Orders.Delete"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.ChangeStatus, L("Permission:Orders.ChangeStatus"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.ConfirmCodPayment, L("Permission:Orders.ConfirmCodPayment"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.ShipOrder, L("Permission:Orders.ShipOrder"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.ViewProfitReport, L("Permission:Orders.ViewProfitReport"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.Complete, L("Permission:Orders.Complete"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.Fulfill, L("Permission:Orders.Fulfill"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.ViewCompleted, L("Permission:Orders.ViewCompleted"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.ViewAll, L("Permission:Orders.ViewAll"));
        #endregion

        #region Cart Permissions
        var cartPermission = myGroup.AddPermission(ProductSellingPermissions.Carts.Default, L("Permission:Carts"));
        cartPermission.AddChild(ProductSellingPermissions.Carts.Create, L("Permission:Carts.Create"));
        cartPermission.AddChild(ProductSellingPermissions.Carts.Edit, L("Permission:Carts.Edit"));
        cartPermission.AddChild(ProductSellingPermissions.Carts.Delete, L("Permission:Carts.Delete"));
        #endregion


        #region Manufacturer Permissions
        var manufacturersPermission = myGroup.AddPermission(ProductSellingPermissions.Manufacturers.Default, L("Permission:Manufacturers"));
        manufacturersPermission.AddChild(ProductSellingPermissions.Manufacturers.Create, L("Permission:Manufacturers.Create"));
        manufacturersPermission.AddChild(ProductSellingPermissions.Manufacturers.Edit, L("Permission:Manufacturers.Edit"));
        manufacturersPermission.AddChild(ProductSellingPermissions.Manufacturers.Delete, L("Permission:Manufacturers.Delete"));
        #endregion

        #region Blog Permissions
        var blogsPermission = myGroup.AddPermission(ProductSellingPermissions.Blogs.Default, L("Permission:Blogs"));
        blogsPermission.AddChild(ProductSellingPermissions.Blogs.Create, L("Permission:Blogs.Create"));
        blogsPermission.AddChild(ProductSellingPermissions.Blogs.Edit, L("Permission:Blogs.Edit"));
        blogsPermission.AddChild(ProductSellingPermissions.Blogs.Delete, L("Permission:Blogs.Delete"));
        //blogsPermission.AddChild(ProductSellingPermissions.Blogs.Publish, L("Permission:Blogs.Publish"));

        #endregion
        #region Comment Permissions

        var commentsPermission = myGroup.AddPermission(ProductSellingPermissions.Comments.Default, L("Permission:Comments"));
        commentsPermission.AddChild(ProductSellingPermissions.Comments.Create, L("Permission:Comments.Create"));
        commentsPermission.AddChild(ProductSellingPermissions.Comments.Edit, L("Permission:Comments.Edit"));
        commentsPermission.AddChild(ProductSellingPermissions.Comments.Delete, L("Permission:Comments.Delete"));
        commentsPermission.AddChild(ProductSellingPermissions.Comments.Approve, L("Permission:Comments.Approve"));
        #endregion
        #region Chatbot
        //chatbot
        var chatbotPermission = myGroup.AddPermission(ProductSellingPermissions.Chatbots.Default, L("Permission:Chatbot"));
        var chatbotAdminPermission = myGroup.AddPermission(ProductSellingPermissions.Chatbots.Admin, L("Permission:Chatbot:Admin"));
        var chatbotHistoryPermission = myGroup.AddPermission(ProductSellingPermissions.Chatbots.ViewHistory, L("Permission:Chatbot:ViewHistory"));
        #endregion
        //hangfire
        myGroup.AddPermission(ProductSellingPermissions.Hangfire.Dashboard, L("Permission:HangfireDashboard"));

        #region Stores
        //store
        var storesPermission = myGroup.AddPermission(ProductSellingPermissions.Stores.Default, L("Permission:Stores"));
        storesPermission.AddChild(ProductSellingPermissions.Stores.Create, L("Permission:Stores.Create"));
        storesPermission.AddChild(ProductSellingPermissions.Stores.Edit, L("Permission:Stores.Edit"));
        storesPermission.AddChild(ProductSellingPermissions.Stores.Delete, L("Permission:Stores.Delete"));
        #endregion

        #region Users 
        var usersPermission = myGroup.AddPermission(ProductSellingPermissions.Users.Default, L("Permission:Users"));
        usersPermission.AddChild(ProductSellingPermissions.Users.ManageStoreAssignment, L("Permission:Users.ManageStoreAssignment"))
        #endregion
    }
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ProductSellingResource>(name);
    }
}
