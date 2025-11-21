using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Products.BackgroundJobs.RecentlyViewed
{
    public class RecentlyViewedCleanupArgs
    {
        public int DaysToKeep { get; set; } = RecentlyViewedConsts.CleanupDaysToKeep;

    }
}
