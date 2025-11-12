using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Orders.Dtos
{
    public class CreateInStoreOrderDto
    {
        [Required]
        public string CustomerName { get; set; }

        public string CustomerPhone { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        [Required]
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }
}
