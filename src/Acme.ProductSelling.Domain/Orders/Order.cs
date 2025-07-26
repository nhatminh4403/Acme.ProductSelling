using Acme.ProductSelling.Localization;
using JetBrains.Annotations;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; }
        [CanBeNull]
        [StringLength(OrderConsts.MaxCustomerPhoneLentgth)]
        public string CustomerPhone { get; set; }
        [Required]
        public string ShippingAddress { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        [Required]
        public string PaymentMethod { get; set; } // Thêm phương thức thanh toán

        public OrderStatus Status { get; private set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();

        private Order()
        {
        }

        public Order(Guid id, string orderNumber, DateTime orderDate, Guid? customerId,
            string customerName, string customerPhone,
            string shippingAddress, string payingMethod)
            : base(id)
        {
            OrderNumber = Check.NotNullOrWhiteSpace(orderNumber, nameof(orderNumber), OrderConsts.MaxOrderNumberLength);
            OrderDate = orderDate;
            CustomerId = customerId;

            CustomerName = Check.NotNullOrWhiteSpace(customerName, nameof(customerName), 100);
            CustomerPhone = Check.Length(customerPhone,
                                            nameof(customerPhone),
                                            OrderConsts.MaxCustomerPhoneLentgth);

            ShippingAddress = Check.NotNullOrWhiteSpace(shippingAddress,
                                                        nameof(shippingAddress),
                                                        500);
            Status = OrderStatus.Placed;
            PaymentMethod = Check.NotNullOrWhiteSpace(payingMethod, nameof(payingMethod), 100);
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
                var newItem = new OrderItem(Id,
                                            productId,
                                            productName,
                                            productPrice,
                                            quantity);
                OrderItems.Add(newItem);
            }
            CalculateTotals();
        }
        public void CalculateTotals()
        {
            TotalAmount = OrderItems.Sum(oi => oi.LineTotalAmount);
            if (TotalAmount < 0)
                TotalAmount = 0;
        }
        public void SetStatus(OrderStatus newStatus)
        {

            if (!IsNextStatusValid(newStatus))
            {
                throw new UserFriendlyException
                    ($"Cannot change status from {Status} to {newStatus}.");
            }
            Status = newStatus;
        }
        public void SetPendingStatus()
        {
            if (Status == OrderStatus.Placed)
            {
                Status = OrderStatus.Pending;
            }
        }

        public void MarkAsPaid()
        {
            if (Status == OrderStatus.PendingPayment)
            {
                SetStatus(OrderStatus.Placed); // Hoặc Confirmed tùy quy trình của bạn
            }
        }

        public void CancelByUser(IStringLocalizer<ProductSellingResource> localizer = null)
        {
            if (Status != OrderStatus.Placed)
            {
                throw new UserFriendlyException(localizer["OrderCancelOnlyWhenPlaced"]);
            }
            SetStatus(OrderStatus.Cancelled);
        }
        private bool IsNextStatusValid(OrderStatus newStatus)
        {
            if (newStatus == OrderStatus.Cancelled &&
                (Status == OrderStatus.Placed || Status == OrderStatus.Pending || Status == OrderStatus.Confirmed))
            {
                return true;
            }
            return (int)newStatus >= (int)Status;
        }
    }
}
