using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Orders
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public OrderStatus NewStatus { get; set; }
    }
}
