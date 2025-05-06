using Acme.ProductSelling.Payments;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
namespace Acme.ProductSelling.Orders
{
    public class Order : FullAuditedAggregateRoot<Guid>
    {
        [Required]
        [StringLength(OrderConsts.MaxOrderNumberLength)]
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid? CustomerId { get; set; }
        // Thông tin khách hàng cơ bản
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; }
        [CanBeNull]
        [StringLength(OrderConsts.MaxCustomerPhoneLentgth)]
        public string CustomerPhone { get; set; }
        [Required]
        public string ShippingAddress { get; set; }
        // Tổng tiền đơn hàng (vẫn cần thiết)
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } =new HashSet<OrderItem>();
        
       
        public virtual void AddOrderItem(Guid productId, string productName, decimal productPrice, int quantity)
        {
            var existingItem = OrderItems.FirstOrDefault(oi => oi.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.SetQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var newItem = new OrderItem(Id, productId, productName, productPrice, quantity);
                OrderItems.Add(newItem);
            }
            CalculateTotals();
        }
        public  void CalculateTotals()
        {
            TotalAmount = OrderItems.Sum(oi => oi.LineTotalAmount);
            if (TotalAmount < 0)
                TotalAmount = 0;
        }
    }
}
