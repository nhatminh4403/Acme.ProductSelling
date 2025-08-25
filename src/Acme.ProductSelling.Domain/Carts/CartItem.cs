using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Carts
{
    public class CartItem : Entity<Guid>
    {
        public Guid ProductId { get; set; }
        public Guid CartId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; } // Lấy từ thông tin Product liên kết
        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductPrice { get; set; }
        protected CartItem() { }
        public CartItem(Guid id, Guid cartId, Guid productId, int quantity, string productName, decimal productPrice) : base(id)
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
