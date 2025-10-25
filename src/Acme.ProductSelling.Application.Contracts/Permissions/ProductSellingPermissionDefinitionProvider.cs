using Acme.ProductSelling.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
namespace Acme.ProductSelling.Permissions;
public class ProductSellingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ProductSellingPermissions.GroupName);
        //product
        var productsPermission = myGroup.AddPermission(ProductSellingPermissions.Products.Default, L("Permission:Products"));
        productsPermission.AddChild(ProductSellingPermissions.Products.Create, L("Permission:Products.Create"));
        productsPermission.AddChild(ProductSellingPermissions.Products.Edit, L("Permission:Products.Edit"));
        productsPermission.AddChild(ProductSellingPermissions.Products.Delete, L("Permission:Products.Delete"));

        //category
        var categoriesPermission = myGroup.AddPermission(ProductSellingPermissions.Categories.Default, L("Permission:Categories"));
        categoriesPermission.AddChild(ProductSellingPermissions.Categories.Create, L("Permission:Categories.Create"));
        categoriesPermission.AddChild(ProductSellingPermissions.Categories.Edit, L("Permission:Categories.Edit"));
        categoriesPermission.AddChild(ProductSellingPermissions.Categories.Delete, L("Permission:Categories.Delete"));
        //order
        var orderPermission = myGroup.AddPermission(ProductSellingPermissions.Orders.Default, L("Permission:Orders"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.Create, L("Permission:Orders.Create"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.Edit, L("Permission:Orders.Edit"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.Delete, L("Permission:Orders.Delete"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.ChangeStatus, L("Permission:Orders.ChangeStatus"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.ConfirmCodPayment, L("Permission:Orders.ConfirmCodPayment"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.ShipOrder, L("Permission:Orders.ShipOrder"));
        orderPermission.AddChild(ProductSellingPermissions.Orders.ViewProfitReport, L("Permission:Orders.ViewProfitReport"));
        //cart
        var cartPermission = myGroup.AddPermission(ProductSellingPermissions.Carts.Default, L("Permission:Carts"));
        cartPermission.AddChild(ProductSellingPermissions.Carts.Create, L("Permission:Carts.Create"));
        cartPermission.AddChild(ProductSellingPermissions.Carts.Edit, L("Permission:Carts.Edit"));
        cartPermission.AddChild(ProductSellingPermissions.Carts.Delete, L("Permission:Carts.Delete"));
        //manufacturer
        var manufacturersPermission = myGroup.AddPermission(ProductSellingPermissions.Manufacturers.Default, L("Permission:Manufacturers"));
        manufacturersPermission.AddChild(ProductSellingPermissions.Manufacturers.Create, L("Permission:Manufacturers.Create"));
        manufacturersPermission.AddChild(ProductSellingPermissions.Manufacturers.Edit, L("Permission:Manufacturers.Edit"));
        manufacturersPermission.AddChild(ProductSellingPermissions.Manufacturers.Delete, L("Permission:Manufacturers.Delete"));
        //blog
        var blogsPermission = myGroup.AddPermission(ProductSellingPermissions.Blogs.Default, L("Permission:Blogs"));
        blogsPermission.AddChild(ProductSellingPermissions.Blogs.Create, L("Permission:Blogs.Create"));
        blogsPermission.AddChild(ProductSellingPermissions.Blogs.Edit, L("Permission:Blogs.Edit"));
        blogsPermission.AddChild(ProductSellingPermissions.Blogs.Delete, L("Permission:Blogs.Delete"));
        //blogsPermission.AddChild(ProductSellingPermissions.Blogs.Publish, L("Permission:Blogs.Publish"));
        //comment
        var commentsPermission = myGroup.AddPermission(ProductSellingPermissions.Comments.Default, L("Permission:Comments"));
        commentsPermission.AddChild(ProductSellingPermissions.Comments.Create, L("Permission:Comments.Create"));
        commentsPermission.AddChild(ProductSellingPermissions.Comments.Edit, L("Permission:Comments.Edit"));
        commentsPermission.AddChild(ProductSellingPermissions.Comments.Delete, L("Permission:Comments.Delete"));
        commentsPermission.AddChild(ProductSellingPermissions.Comments.Approve, L("Permission:Comments.Approve"));
        //chatbot
        var chatbotPermission = myGroup.AddPermission(ProductSellingPermissions.Chatbots.Default, L("Permission:Chatbot"));
        var chatbotAdminPermission = myGroup.AddPermission(ProductSellingPermissions.Chatbots.Admin, L("Permission:Chatbot:Admin"));
        var chatbotHistoryPermission = myGroup.AddPermission(ProductSellingPermissions.Chatbots.ViewHistory, L("Permission:Chatbot:ViewHistory"));

        //hangfire
        myGroup.AddPermission(ProductSellingPermissions.Hangfire.Dashboard, L("Permission:HangfireDashboard"));

    }
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ProductSellingResource>(name);
    }
}
