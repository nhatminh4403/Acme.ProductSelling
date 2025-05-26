using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Carts
{
    public class UpdateCartItemInput
    {
        public Guid CartItemId { get; set; }


        [Range(1, 100)] // Số lượng mới phải lớn hơn 0
        public int Quantity { get; set; }
    }
}
