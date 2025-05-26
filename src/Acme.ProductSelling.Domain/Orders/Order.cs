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
        public OrderStatus Status { get; private set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } =new HashSet<OrderItem>();
        
        private Order()
        {
        }

        public Order(Guid id, string orderNumber, DateTime orderDate, Guid? customerId, string customerName, string customerPhone, string shippingAddress)
            : base(id)
        {
            OrderNumber = Check.NotNullOrWhiteSpace(orderNumber, nameof(orderNumber), OrderConsts.MaxOrderNumberLength);
            OrderDate = orderDate;
            CustomerId = customerId;

            CustomerName = Check.NotNullOrWhiteSpace(customerName, nameof(customerName), 100);
            CustomerPhone = Check.Length(customerPhone, nameof(customerPhone), OrderConsts.MaxCustomerPhoneLentgth);

            ShippingAddress = Check.NotNullOrWhiteSpace(shippingAddress, nameof(shippingAddress), 500);
            Status = OrderStatus.Placed;
        }
        public void ChangeStatus(OrderStatus newStatus)
        {
            // (Tuỳ chọn) Kiểm tra tính hợp lệ của chuyển trạng thái
            if (!IsValidTransition(Status, newStatus))
            {
                throw new BusinessException("Order:InvalidStatusTransition",
                    $"Không thể đổi từ {Status} sang {newStatus}");
            }
            Status = newStatus;
        }
        private bool IsValidTransition(OrderStatus from, OrderStatus to)
        {
            // Ví dụ: chỉ cho Confirmed khi đang Placed
            return (from, to) switch
            {
                (OrderStatus.Placed, OrderStatus.Pending) => true,
                (OrderStatus.Pending, OrderStatus.Confirmed) => true,
                (OrderStatus.Confirmed, OrderStatus.Processing) => true,
                (OrderStatus.Processing, OrderStatus.Shipped) => true,
                (OrderStatus.Placed, OrderStatus.Cancelled) => true,
                (OrderStatus.Pending, OrderStatus.Cancelled) => true,
                _ => false
            };
        }
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
