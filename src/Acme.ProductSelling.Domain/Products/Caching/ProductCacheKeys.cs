namespace Acme.ProductSelling.Products.Caching
{
    public static class ProductCacheKeys
    {
        // Single product detail (used by GetAsync / GetProductBySlug)
        // Key  : product:detail:{productId}
        public const string DetailById = "product:detail:";
        public const string DetailBySlug = "product:slug:";

        // Paged list results (used by GetListAsync)
        // Key  : product:list:{sorting}:{skipCount}:{maxResultCount}
        public const string List = "product:list:";

        // Featured carousels — one entry for the whole payload
        public const string FeaturedCarousels = "product:featured-carousels";

        // Per-category list (ProductLookupAppService)
        // Key  : product:category:{categoryId}:{sorting}:{skip}:{max}
        public const string ByCategory = "product:category:";

        // TTLs (minutes)
        public const int DetailTtlMinutes = 15;
        public const int ListTtlMinutes = 5;
        public const int FeaturedCarouselMinutes = 10;
        public const int LookupListMinutes = 5;
    }
}
