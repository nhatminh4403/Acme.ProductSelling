using PaypalServerSdk.Standard.Models;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.PaymentGateway.PayPal
{
    public interface IPayPalService : ITransientDependency
    {
        Task<string> CreatePaymentAsync(decimal amount, string currency, string returnUrl, string cancelUrl);
        Task<Order> ExecutePaymentAsync(string orderId);

        PayPalOption GetPayPalOption();
    }
}
