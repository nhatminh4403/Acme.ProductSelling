using Acme.ProductSelling.Payments;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Orders
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public OrderStatus NewStatus { get; set; }
        [Required]
        public PaymentStatus NewPaymentStatus { get; set; }
    }
}
