using PayPal.Api;

namespace Acme.ProductSelling.PaymentGateway.PayPal
{
    public interface IPayPalService
    {
        string CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl);
        Payment ExecutePayment(string paymentId, string payerId);
    }
}
