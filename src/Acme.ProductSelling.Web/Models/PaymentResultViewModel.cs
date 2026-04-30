namespace Acme.ProductSelling.Web.Models
{
    public class PaymentResultViewModel
    {
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
        public bool IsSuccess { get; set; }
    }
}
