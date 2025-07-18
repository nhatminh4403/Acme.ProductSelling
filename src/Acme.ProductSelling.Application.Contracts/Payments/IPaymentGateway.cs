using Acme.ProductSelling.Orders;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Payments
{
    public interface IPaymentGateway
    {
        string Name { get; }
        Task<PaymentGatewayResult> ProcessAsync(Order order);
    }
    public class PaymentGatewayResult
    {
        public string RedirectUrl { get; set; }
        public OrderStatus NextOrderStatus { get; set; }
    }
}
