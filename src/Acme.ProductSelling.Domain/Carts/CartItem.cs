using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using System.Xml.Linq;

namespace Acme.ProductSelling.Carts
{
    public class CartItem : Entity<Guid>
    {
        public Guid ProductId { get;  set; }
        public Guid CartId { get;  set; }
        public int Quantity { get;  set; }
        public string ProductName { get; set; } // Lấy từ thông tin Product liên kết
        public decimal ProductPrice { get; set; } // Lấy từ thông tin Product liên kết
        protected CartItem() { }
        public CartItem(Guid id, Guid cartId, Guid productId, int quantity, string productName,decimal productPrice) : base(id)
        {
            Check.NotNullOrWhiteSpace(productName, nameof(productName));

            CartId = cartId;
            ProductId = productId;
            SetQuantity(quantity);
            ProductName = productName;
            ProductPrice = productPrice;
        }

        public void SetQuantity(int quantity)
        {
            Check.Range(quantity, nameof(quantity), 1, int.MaxValue);
            Quantity = quantity;
        }
    }
}
