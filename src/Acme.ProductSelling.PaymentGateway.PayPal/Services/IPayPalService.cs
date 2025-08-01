using PayPal.Api;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.PaymentGateway.PayPal
{
    public interface IPayPalService : ITransientDependency
    {
        string CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl);
        Payment ExecutePayment(string paymentId, string payerId);
    }
}
