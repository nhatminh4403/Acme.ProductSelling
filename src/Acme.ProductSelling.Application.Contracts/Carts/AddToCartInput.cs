using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Carts
{
    public class AddToCartInput
    {
        [Required]
        public Guid ProductId { get; set; }

        /// <summary>
        /// Số lượng sản phẩm muốn thêm. Mặc định là 1.
        /// </summary>
        [Range(1, 100)] // Ví dụ: giới hạn số lượng thêm mỗi lần
        public int Quantity { get; set; } = 1;

        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
    }
}
