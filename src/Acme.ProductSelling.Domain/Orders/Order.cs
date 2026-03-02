using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Payments;
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
    public abstract class Order : FullAuditedAggregateRoot<Guid>
    {
        [Required, StringLength(OrderConsts.MaxOrderNumberLength)]
        public string OrderNumber { get; protected set; }
        public DateTime OrderDate { get; protected set; }
        public Guid? CustomerId { get; protected set; }
        [Required, StringLength(100)]
        public string CustomerName { get; protected set; }
        [StringLength(OrderConsts.MaxCustomerPhoneLentgth)]
        public string CustomerPhone { get; protected set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; protected set; }
        [Required]
        public string PaymentMethod { get; protected set; }
        public OrderStatus OrderStatus { get; protected set; }
        public PaymentStatus PaymentStatus { get; protected set; }
        public OrderType OrderType { get; protected set; }
        public Guid? StoreId { get; protected set; }
        public virtual ICollection<OrderItem> OrderItems { get; protected set; } = new HashSet<OrderItem>();
        public virtual ICollection<OrderHistory> OrderHistories { get; protected set; } = new HashSet<OrderHistory>();

        protected Order() { }

        protected Order(Guid id, string orderNumber, DateTime orderDate,
            Guid? customerId, string customerName, string customerPhone, string paymentMethod)
            : base(id)
        {
            OrderNumber = Check.NotNullOrWhiteSpace(orderNumber, nameof(orderNumber));
            OrderDate = orderDate;
            CustomerId = customerId;
            CustomerName = Check.NotNullOrWhiteSpace(customerName, nameof(customerName));
            CustomerPhone = customerPhone;
            PaymentMethod = Check.NotNullOrWhiteSpace(paymentMethod, nameof(paymentMethod));
            PaymentStatus = PaymentStatus.Unpaid;
        }
        // PUBLIC METHODS


        public void UpdatePaymentMethod(string paymentMethod)
        {
            PaymentMethod = Check.NotNullOrWhiteSpace(paymentMethod, nameof(paymentMethod));
        }

        public virtual void AddOrderItem(
            Guid productId,
            string productName,
            decimal productPrice,
            int quantity)
        {
            var existingItem = OrderItems.FirstOrDefault(oi => oi.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.SetQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                OrderItems.Add(new OrderItem(Id, productId, productName, productPrice, quantity));
            }
            CalculateTotals();
        }

        public void CalculateTotals()
        {
            TotalAmount = OrderItems.Any() ? OrderItems.Sum(oi => oi.LineTotalAmount) : 0;
        }

        public void SetStatus(OrderStatus newStatus)
        {
            if (!IsNextStatusValid(newStatus))
            {
                throw new UserFriendlyException("OrderStatusChangeNotAllowed",
                    $"Không thể chuyển trạng thái từ '{OrderStatus}' sang '{newStatus}'.");
            }
            OrderStatus = newStatus;
        }

        public void SetPaymentStatus(PaymentStatus newPaymentStatus)
        {
            //var validTransitions = new Dictionary<PaymentStatus, PaymentStatus[]>
            //{
            //    { PaymentStatus.Unpaid, new[] { PaymentStatus.Pending, PaymentStatus.PendingOnDelivery, PaymentStatus.Paid, PaymentStatus.Cancelled } },
            //    { PaymentStatus.Pending, new[] { PaymentStatus.Paid, PaymentStatus.Failed, PaymentStatus.Cancelled } },
            //    { PaymentStatus.PendingOnDelivery, new[] { PaymentStatus.Paid, PaymentStatus.Cancelled } },
            //    { PaymentStatus.Paid, new[] { PaymentStatus.Refunded } },
            //    { PaymentStatus.Failed, new[] { PaymentStatus.Pending } },
            //    { PaymentStatus.Cancelled, new PaymentStatus[] { } },
            //    { PaymentStatus.Refunded, new PaymentStatus[] { } }
            //};

            //if (!validTransitions.ContainsKey(PaymentStatus) ||
            //    !validTransitions[PaymentStatus].Contains(newPaymentStatus))
            //{
            //    throw new UserFriendlyException("InvalidPaymentStatusChange",
            //        $"Không thể chuyển trạng thái thanh toán từ '{PaymentStatus}' sang '{newPaymentStatus}'.");
            //}

            PaymentStatus = newPaymentStatus;
        }
        private bool IsNextStatusValid(OrderStatus newStatus)
        {
            if (newStatus == OrderStatus.Cancelled &&
                (OrderStatus == OrderStatus.Placed || OrderStatus == OrderStatus.Pending || OrderStatus == OrderStatus.Confirmed))
            {
                return true;
            }

            return (int)newStatus >= (int)OrderStatus;
        }
    }
}
