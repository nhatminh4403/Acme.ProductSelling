using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Carts
{
    public class AddToCartInput
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
