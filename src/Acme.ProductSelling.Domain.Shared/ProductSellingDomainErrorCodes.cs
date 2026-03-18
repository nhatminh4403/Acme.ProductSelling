namespace Acme.ProductSelling;

public static class ProductSellingDomainErrorCodes
{
    public const string UserNotAssignedToStore = "ProductSelling:UserNotAssignedToStore";
    public const string NoStoreAccess = "ProductSelling:NoStoreAccess";
    public const string ProductNotActive = "ProductSelling:ProductNotActive";
    public const string InvalidOrderStatus = "ProductSelling:InvalidOrderStatus";

    // New error codes
    public const string OrderNotFound = "ProductSelling:OrderNotFound";
    public const string OrderCannotShip = "ProductSelling:OrderCannotShip";
    public const string OrderCannotDeliver = "ProductSelling:OrderCannotDeliver";
    public const string OrderOnlyForInStoreOrders = "ProductSelling:OnlyForInStoreOrders";
    public const string OrderInvalidPaymentStatusChange = "ProductSelling:InvalidPaymentStatusChange";
    public const string CartIsEmpty = "ProductSelling:CartIsEmpty";
    public const string AccountUnauthorized = "ProductSelling:Unauthorized";
    public const string AccountUserNotAuthenticated = "ProductSelling:UserNotAuthenticated";
    public const string UserCannotAssignStoreToCustomer = "ProductSelling:UserCannotAssignStoreToCustomer";
    public const string InventoryRecordAlreadyExists = "ProductSelling:InventoryRecordAlreadyExists";
    public const string InventoryCannotTransferToSameStore = "ProductSelling:InventoryCannotTransferToSameStore";
    public const string InventorySourceNotFound = "ProductSelling:InventorySourceNotFound";
    public const string InventoryInsufficientStock = "ProductSelling:InventoryInsufficientStock";
    public const string InventoryNotFoundForStoreAndProduct = "ProductSelling:InventoryNotFoundForStoreAndProduct";
    public const string InventoryNoAccessToStore = "ProductSelling:InventoryNoAccessToStore";
    public const string InventoryNoPermissionViewAll = "ProductSelling:InventoryNoPermissionViewAll";
    public const string PaymentInitializationFailed = "ProductSelling:PaymentInitializationFailed";
    public const string PaymentVnPayLinkCreationFailed = "ProductSelling:VnPayLinkCreationFailed";
    public const string PaymentInvalidOrderAmount = "ProductSelling:PaymentInvalidOrderAmount";
    public const string PaymentOrderAlreadyPaid = "ProductSelling:PaymentOrderAlreadyPaid";
    public const string PaymentCannotPayForCancelledOrder = "ProductSelling:PaymentCannotPayForCancelledOrder";
    public const string PaymentPayPalSmallAmount = "ProductSelling:PaymentPayPalSmallAmount";
    public const string PaymentPayPalLargeAmount = "ProductSelling:PaymentPayPalLargeAmount";
    public const string PaymentPayPalLinkCreationFailed = "ProductSelling:PaymentPayPalLinkCreationFailed";
    public const string PaymentPayPalConnectionError = "ProductSelling:PaymentPayPalConnectionError";
    public const string PaymentInvalidIpnData = "ProductSelling:PaymentInvalidIpnData";
    public const string PaymentInvalidReferenceCode = "ProductSelling:PaymentInvalidReferenceCode";
    public const string PaymentAmountMismatch = "ProductSelling:PaymentAmountMismatch";

    // Products
    public const string ProductPriceCannotBeNegative = "ProductSelling:ProductPriceCannotBeNegative";
    public const string ProductDiscountPercentInvalid = "ProductSelling:ProductDiscountPercentInvalid";
    public const string ProductStockCountCannotBeNegative = "ProductSelling:ProductStockCountCannotBeNegative";
    public const string ProductNameAlreadyExists = "ProductSelling:ProductNameAlreadyExists";
    public const string ProductNotFound = "ProductSelling:ProductNotFound";
    public const string ProductNotYetReleased = "ProductSelling:ProductNotYetReleased";

    // Categories
    public const string CategoryNameAlreadyExists = "ProductSelling:CategoryNameAlreadyExists";

    // Orders
    public const string OrderCannotCompletePayment = "ProductSelling:OrderCannotCompletePayment";
    public const string OrderCannotFulfillOrder = "ProductSelling:OrderCannotFulfillOrder";
    public const string OrderPaymentNotCompleted = "ProductSelling:OrderPaymentNotCompleted";
    public const string OrderOnlyForCODOrders = "ProductSelling:OrderOnlyForCODOrders";
    public const string OrderActionNotAllowedForNonCodOrder = "ProductSelling:OrderActionNotAllowedForNonCodOrder";
    public const string OrderCannotConfirmPaymentForThisOrder = "ProductSelling:OrderCannotConfirmPaymentForThisOrder";
    public const string OrderCanOnlyBeCancelledWhenPlacedOrPending = "ProductSelling:OrderCanOnlyBeCancelledWhenPlacedOrPending";
    public const string OrderCannotCancelPaidOrder = "ProductSelling:OrderCannotCancelPaidOrder";
    public const string OrderStatusChangeNotAllowed = "ProductSelling:OrderStatusChangeNotAllowed";

    // Users
    public const string StaffUsersCannotHaveCustomerProfiles = "ProductSelling:StaffUsersCannotHaveCustomerProfiles";

    // OpenIddict
    public const string NoClientSecretCanBeSetForPublicApplications = "ProductSelling:NoClientSecretCanBeSetForPublicApplications";
    public const string TheClientSecretIsRequiredForConfidentialApplications = "ProductSelling:TheClientSecretIsRequiredForConfidentialApplications";
    public const string InvalidRedirectUri = "ProductSelling:InvalidRedirectUri";
    public const string InvalidPostLogoutRedirectUri = "ProductSelling:InvalidPostLogoutRedirectUri";

    // Store Inventory Additional
    public const string StoreInventoryInsufficientStock = "ProductSelling:StoreInventoryInsufficientStock";
    public const string StoreInventoryQuantityMustBePositive = "ProductSelling:QuantityMustBePositive";
    public const string StoreInventoryQuantityCannotBeNegative = "ProductSelling:QuantityCannotBeNegative";
    public const string StoreInventoryReorderValuesCannotBeNegative = "ProductSelling:ReorderValuesCannotBeNegative";

    // Gemini
    public const string GeminiSearchError = "ProductSelling:GeminiSearchError";
    public const string GeminiRetrieveModelsFailed = "ProductSelling:GeminiRetrieveModelsFailed";
    public const string GeminiCountTokensFailed = "ProductSelling:GeminiCountTokensFailed";

    // Data Seeding
    public const string IdentityDataSeedingFailed = "ProductSelling:IdentityDataSeedingFailed";
    public const string OpenIddictDataSeedingFailed = "ProductSelling:OpenIddictDataSeedingFailed";
    public const string DatabaseMigrationFailed = "ProductSelling:DatabaseMigrationFailed";

    // Stores
    public const string StoreNotFound = "ProductSelling:StoreNotFound";
}