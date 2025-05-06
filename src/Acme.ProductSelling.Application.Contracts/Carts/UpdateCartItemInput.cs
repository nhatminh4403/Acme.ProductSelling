using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Carts
{
    public class UpdateCartItemInput
    {
        public Guid CartItemId { get; set; }


        [Range(1, 100)] // Số lượng mới phải lớn hơn 0
        public int Quantity { get; set; }
    }
}
