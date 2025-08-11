namespace Acme.ProductSelling.PaymentGateway.MoMo.Configurations
{
    public class MoMoOption
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string PartnerCode { get; set; }
        public string Language { get; set; } = "vi";
        public string Endpoint
        {
            get
            {
                return "https://test-payment.momo.vn/v2/gateway/api/create/";
            }
            set { }
        }
        public string RequestType { get; set; } = "payWithMethod";
    }
}
