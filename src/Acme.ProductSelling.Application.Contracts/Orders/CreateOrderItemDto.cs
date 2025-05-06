using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
