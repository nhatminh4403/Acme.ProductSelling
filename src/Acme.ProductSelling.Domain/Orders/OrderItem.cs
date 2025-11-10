using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
namespace Acme.ProductSelling.Orders
{
    public class OrderItem : Entity<Guid>
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotalAmount => Price * Quantity;
        public Guid OrderId { get; set; }
        protected OrderItem()
        {
        }
        public OrderItem(Guid orderId, Guid productId, string productName, decimal price, int quantity)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductId = productId;
            ProductName = Check.NotNullOrWhiteSpace(productName, nameof(productName));
            Price = price;
            SetQuantity(quantity);
        }
        public void SetQuantity(int quantity)
        {
            Check.Range(quantity, nameof(quantity), 1, int.MaxValue); // Đảm bảo số lượng > 0
            Quantity = quantity;
        }
    }
}
