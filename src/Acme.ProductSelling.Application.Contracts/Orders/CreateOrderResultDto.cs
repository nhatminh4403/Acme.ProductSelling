namespace Acme.ProductSelling.Orders
{
    public class CreateOrderResultDto
    {
        public OrderDto Order { get; set; }
        public string RedirectUrl { get; set; }
    }
}
