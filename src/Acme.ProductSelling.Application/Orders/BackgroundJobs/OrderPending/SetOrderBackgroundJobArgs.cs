using System;

namespace Acme.ProductSelling.Orders.BackgroundJobs.OrderPending
{
    [Serializable]
    public class SetOrderBackgroundJobArgs
    {
        public Guid OrderId { get; set; }
    }
}
