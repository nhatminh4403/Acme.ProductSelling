namespace Acme.ProductSelling;

public static class ProductSellingDomainErrorCodes
{
    public const string UserNotAssignedToStore = "ProductSelling:UserNotAssignedToStore";
    public const string NoStoreAccess = "ProductSelling:NoStoreAccess";
    public const string ProductNotActive = "ProductSelling:ProductNotActive";
    public const string InvalidOrderStatus = "ProductSelling:InvalidOrderStatus";
}