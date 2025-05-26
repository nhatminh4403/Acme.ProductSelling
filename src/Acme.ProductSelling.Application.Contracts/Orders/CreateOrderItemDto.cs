using System;
using System.ComponentModel.DataAnnotations;
namespace Acme.ProductSelling.Orders
{
    public class CreateOrderItemDto
    {
        [Required]
        public Guid ProductId { get; set; }
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
