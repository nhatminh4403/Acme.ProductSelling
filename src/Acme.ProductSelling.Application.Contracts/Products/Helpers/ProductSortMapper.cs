namespace Acme.ProductSelling.Products.Helpers
{
    /// <summary>
    /// Maps frontend sort values to ABP Sorting format
    /// </summary>
    public static class ProductSortMapper
    {
        /// <summary>
        /// Convert frontend sort value to ABP Sorting string format
        /// </summary>
        /// <param name="sortBy">Frontend sort value (e.g., "price-asc", "name-desc")</param>
        /// <returns>ABP Sorting format or special marker for complex sorts</returns>
        public static string MapToAbpSorting(string sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return "ProductName ASC"; // Default
            }

            return sortBy.ToLower() switch
            {
                "featured" => "ProductName ASC",
                "name-asc" => "ProductName ASC",
                "name-desc" => "ProductName DESC",

                // Price sorting needs special handling for DiscountedPrice ?? OriginalPrice
                // We use special markers that the service layer will handle
                "price-asc" => "price-asc",
                "price-desc" => "price-desc",

                "newest" => "CreationTime DESC",
                "oldest" => "CreationTime ASC",

                _ => "ProductName ASC" // Fallback to default
            };
        }

        /// <summary>
        /// Check if sort value requires special handling
        /// </summary>
        public static bool RequiresSpecialHandling(string sorting)
        {
            return sorting == "price-asc" || sorting == "price-desc";
        }

        /// <summary>
        /// Get user-friendly display name for sort option
        /// </summary>
        public static string GetDisplayName(string sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "featured" => "Featured",
                "name-asc" => "Name: A-Z",
                "name-desc" => "Name: Z-A",
                "price-asc" => "Price: Low to High",
                "price-desc" => "Price: High to Low",
                "newest" => "Newest First",
                "oldest" => "Oldest First",
                _ => "Featured"
            };
        }
    }
}