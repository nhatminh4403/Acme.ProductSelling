using Acme.ProductSelling.Payments;

namespace Acme.ProductSelling.Orders.Services
{
    public interface IPaymentStatusPolicy
    {
        bool CanTransition(PaymentStatus from, PaymentStatus to);
    }
}
