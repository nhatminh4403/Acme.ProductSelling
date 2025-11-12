using Acme.ProductSelling.Payments;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Orders
{
    public class OrderHistory : CreationAuditedEntity<Guid>
    {
        public Guid OrderId { get; set; }
        public OrderStatus OldStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public PaymentStatus OldPaymentStatus { get; set; }
        public PaymentStatus NewPaymentStatus { get; set; }
        public string ChangeDescription { get; set; }
        public string ChangedBy { get; set; }

        protected OrderHistory() { }

        public OrderHistory(
            Guid id,
            Guid orderId,
            OrderStatus oldStatus,
            OrderStatus newStatus,
            PaymentStatus oldPaymentStatus,
            PaymentStatus newPaymentStatus,
            string changeDescription,
            string changedBy) : base(id)
        {
            OrderId = orderId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            OldPaymentStatus = oldPaymentStatus;
            NewPaymentStatus = newPaymentStatus;
            ChangeDescription = changeDescription;
            ChangedBy = changedBy;
        }
    }
}
