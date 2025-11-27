namespace Acme.ProductSelling.Products.BackgroundJobs.RecentlyViewed
{
    public class RecentlyViewedCleanupArgs
    {
        public int DaysToKeep { get; set; } = RecentlyViewedConsts.CleanupDaysToKeep;

    }
}
