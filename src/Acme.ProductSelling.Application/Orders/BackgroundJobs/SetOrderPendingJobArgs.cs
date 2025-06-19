using System;

namespace Acme.ProductSelling.Orders.BackgroundJobs
{
    [Serializable]
    public class SetOrderPendingJobArgs
    {
        public Guid OrderId { get; set; }
    }
}
