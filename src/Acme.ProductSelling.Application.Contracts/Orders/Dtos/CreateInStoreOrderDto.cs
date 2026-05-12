using System;
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
        
        public Guid? CurrentUserStoreId { get; set; } = Guid.Empty; // Optional, can be set by the system based on current user's store assignment

        [Required]
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }
}
