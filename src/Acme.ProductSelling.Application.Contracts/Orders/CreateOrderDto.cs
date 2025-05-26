using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Acme.ProductSelling.Orders
{
    public class CreateOrderDto
    {
        [Required]
        public string CustomerName { get; set; }
        [Phone] // Giữ validation SĐT
        public string CustomerPhone { get; set; }
        [Required]
        public string ShippingAddress { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }
}
