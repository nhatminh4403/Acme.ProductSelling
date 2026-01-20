namespace Acme.ProductSelling.PaymentGateway.MoMo.Configurations.Models
{
    public class MoMoPaymentRequest
    {
        public string PartnerCode { get; set; }
        public string RequestId { get; set; }
        public long Amount { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public string RedirectUrl { get; set; }
        public string IpnUrl { get; set; }
        public string RequestType { get; set; }
        public bool AutoCapture { get; set; } = true;
        public string Lang { get; set; }
        public string ExtraData { get; set; }
        public string AccessKey { get; set; }
        public string Signature { get; set; }
    }
}
