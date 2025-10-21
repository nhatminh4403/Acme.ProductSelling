namespace Acme.ProductSelling.Orders.Dtos
{
    public class CreateOrderResultDto
    {
        public OrderDto Order { get; set; }
        public string RedirectUrl { get; set; }
    }
}
