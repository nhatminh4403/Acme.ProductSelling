namespace Acme.ProductSelling.PaymentGateway.PayPal
{
    public class PayPalOption
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
        public string Environment { get; set; } = "sandbox";
    }
}
