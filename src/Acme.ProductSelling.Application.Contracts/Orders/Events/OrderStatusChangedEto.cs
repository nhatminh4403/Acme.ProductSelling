using System;
using Volo.Abp.EventBus;

namespace Acme.ProductSelling.Orders
{
    [EventName("Acme.ProductSelling.OrderStatusChanged")]
    [Serializable]
    public class OrderStatusChangedEto
    {
        public Guid OrderId { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
