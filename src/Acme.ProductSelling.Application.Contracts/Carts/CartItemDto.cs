using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Carts
{
    public class CartItemDto : EntityDto<Guid>
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } // Lấy từ thông tin Product liên kết
        public decimal ProductPrice { get; set; } // Lấy từ thông tin Product liên kết
        public int Quantity { get; set; }
        public decimal LineTotal => ProductPrice * Quantity;
    }
}
