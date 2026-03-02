using Acme.ProductSelling.Payments;
using System;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Orders.Services
{
    public class DefaultPaymentStatusPolicy : IPaymentStatusPolicy, ISingletonDependency
    {
        private static readonly Dictionary<PaymentStatus, PaymentStatus[]> Transitions = new()
    {
        { PaymentStatus.Unpaid,            [PaymentStatus.Pending, PaymentStatus.PendingOnDelivery, PaymentStatus.Paid, PaymentStatus.Cancelled] },
        { PaymentStatus.Pending,           [PaymentStatus.Paid, PaymentStatus.Failed, PaymentStatus.Cancelled] },
        { PaymentStatus.PendingOnDelivery, [PaymentStatus.Paid, PaymentStatus.Cancelled] },
        { PaymentStatus.Paid,              [PaymentStatus.Refunded] },
        { PaymentStatus.Failed,            [PaymentStatus.Pending] },
        { PaymentStatus.Cancelled,         [] },
        { PaymentStatus.Refunded,          [] },
    };

        public bool CanTransition(PaymentStatus from, PaymentStatus to)
            => Transitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
    }
}
