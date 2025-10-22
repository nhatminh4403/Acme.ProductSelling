using System;

namespace Acme.ProductSelling.Orders.BackgroundJobs.OrderCleanup
{
    [Serializable]
    public class CleanupOldOrdersJobArgs
    {
        public int MonthsOld { get; set; } = 6;
    }
}
