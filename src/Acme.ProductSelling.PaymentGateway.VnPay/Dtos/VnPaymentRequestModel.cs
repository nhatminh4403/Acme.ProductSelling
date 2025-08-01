using System;

namespace Acme.ProductSelling.PaymentGateway.VnPay.Dtos
{
    public class VnPaymentRequestModel
    {
        public string OrderId { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
