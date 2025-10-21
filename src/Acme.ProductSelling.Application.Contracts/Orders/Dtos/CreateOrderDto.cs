using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Acme.ProductSelling.Orders.Dtos
{
    public class CreateOrderDto
    {
        [Required]
        public string CustomerName { get; set; }
        [Phone] // Giữ validation SĐT
        public string CustomerPhone { get; set; }
        [Required]
        public string ShippingAddress { get; set; }
        [Required]
        public string PaymentMethod { get; set; } // Thêm phương thức thanh toán
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }
}
