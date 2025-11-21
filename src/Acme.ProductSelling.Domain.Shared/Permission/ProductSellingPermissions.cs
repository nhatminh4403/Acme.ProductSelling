namespace Acme.ProductSelling.Permissions;

public static class ProductSellingPermissions
{
    public const string GroupName = "ProductSelling";

    public static class Products
    {
        public const string Default = GroupName + ".Products";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
    public static class Categories
    {
        public const string Default = GroupName + ".Categories";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
    public static class Manufacturers
    {
        public const string Default = GroupName + ".Manufacturers";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
    public static class Carts
    {
        public const string Default = GroupName + ".Carts";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
    public static class Orders
    {
        public const string Default = GroupName + ".Orders";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ChangeStatus = Default + ".ChangeStatus";
        public const string ConfirmCodPayment = Default + ".ConfirmCodPayment";
        public const string ShipOrder = Default + ".ShipOrder";
        public const string ViewProfitReport = Default + ".ViewProfitReport";

        public const string ViewCompleted = Default + ".ViewCompleted";
        public const string Fulfill = Default + ".Fulfill";
        public const string Complete = Default + ".Complete";
        public const string ViewAll = Default + ".ViewAll";
    }

    public static class Stores
    {
        public const string Default = GroupName + ".Stores";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }// You can add specific inventory permissions if needed
    public static class Inventory
    {
        public const string Default = GroupName + ".Inventory";
        public const string Manage = Default + ".Manage";
        public const string Transfer = Default + ".Transfer";
        public const string ViewAllStores = Default + ".ViewAllStores";
    }
    public static class Users
    {
        public const string Default = GroupName + ".Users";
        public const string ManageStoreAssignment = Default + ".ManageStoreAssignment";
        public const string AssignToStore = Default + ".AssignToStore";
        public const string Manage = Default + ".Manage";

    }
    public static class Blogs
    {
        public const string Default = GroupName + ".Blogs";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
    public static class Comments
    {
        public const string Default = GroupName + ".Comments";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Approve = Default + ".Approve";
    }

    public static class Chatbots
    {
        public const string Default = GroupName + ".Chatbot";
        public const string Admin = Default + ".Admin";
        public const string ViewHistory = Default + ".ViewHistory";
    }
    public static class Hangfire
    {
        public const string Dashboard = GroupName + ".HangfireDashboard";
    }
}
