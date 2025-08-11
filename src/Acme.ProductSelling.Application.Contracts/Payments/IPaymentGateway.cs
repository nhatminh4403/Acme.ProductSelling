using Acme.ProductSelling.Orders;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Payments
{
    public interface IPaymentGateway : ITransientDependency
    {
        string Name { get; }
        Task<PaymentGatewayResult> ProcessAsync(Order order);
    }
    public class PaymentGatewayResult
    {
        public string RedirectUrl { get; set; }
    }
}
