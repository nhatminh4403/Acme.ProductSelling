namespace Acme.ProductSelling.VNPay
{
    public class VnPayOptions
    {
        public const string SectionName = "VnPay";
        public string Version { get; set; }
        public string Command { get; set; }
        public string TmnCode { get; set; }
        public string HashSecret { get; set; }
        public string BaseUrl { get; set; }
        public string CurrCode { get; set; }
        public string Locale { get; set; }
        public string PaymentBackReturnUrl { get; set; }
    }
}
