using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Payments;
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
        // Existing properties
        [Required]
        [StringLength(OrderConsts.MaxOrderNumberLength)]
        public string OrderNumber { get; protected set; }

        public DateTime OrderDate { get; protected set; }

        public Guid? CustomerId { get; protected set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; protected set; }

        [CanBeNull]
        [StringLength(OrderConsts.MaxCustomerPhoneLentgth)]
        public string CustomerPhone { get; protected set; }

        [Required]
        public string ShippingAddress { get; protected set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; protected set; }

        [Required]
        public string PaymentMethod { get; protected set; }

        public OrderStatus OrderStatus { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }

        // NEW: Multi-location store properties
        public Guid? StoreId { get; protected set; } // Nullable for online orders

        public OrderType OrderType { get; protected set; } // Online or InStore

        // Staff tracking for in-store orders
        public Guid? SellerId { get; protected set; }
        public string SellerName { get; protected set; }

        public Guid? CashierId { get; protected set; }
        public string CashierName { get; protected set; }

        public Guid? FulfillerId { get; protected set; }
        public string FulfillerName { get; protected set; }

        // Timestamps for in-store workflow
        public DateTime? CompletedAt { get; protected set; } // When cashier processed payment
        public DateTime? FulfilledAt { get; protected set; } // When warehouse gave items

        // Existing collections
        public virtual ICollection<OrderItem> OrderItems { get; protected set; } = new HashSet<OrderItem>();
        public virtual ICollection<OrderHistory> OrderHistories { get; protected set; } = new HashSet<OrderHistory>();

        protected Order() { /* Required by EF Core */ }

        // Updated constructor for online orders (existing behavior)
        public Order(
            Guid id,
            string orderNumber,
            DateTime orderDate,
            Guid? customerId,
            string customerName,
            string customerPhone,
            string shippingAddress,
            string paymentMethod)
            : base(id)
        {
            OrderNumber = Check.NotNullOrWhiteSpace(orderNumber, nameof(orderNumber));
            OrderDate = orderDate;
            CustomerId = customerId;
            CustomerName = Check.NotNullOrWhiteSpace(customerName, nameof(customerName));
            CustomerPhone = customerPhone;
            ShippingAddress = Check.NotNullOrWhiteSpace(shippingAddress, nameof(shippingAddress));
            PaymentMethod = Check.NotNullOrWhiteSpace(paymentMethod, nameof(paymentMethod));

            PaymentStatus = PaymentStatus.Unpaid;
            OrderType = OrderType.Online; // Default to online
        }

        // NEW: Constructor for in-store orders
        public Order(
            Guid id,
            string orderNumber,
            DateTime orderDate,
            Guid storeId,
            Guid? sellerId,
            string sellerName,
            string customerName,
            string customerPhone,
            string paymentMethod)
            : base(id)
        {
            OrderNumber = Check.NotNullOrWhiteSpace(orderNumber, nameof(orderNumber));
            OrderDate = orderDate;
            StoreId = storeId;
            SellerId = sellerId;
            SellerName = sellerName;
            CustomerName = Check.NotNullOrWhiteSpace(customerName, nameof(customerName));
            CustomerPhone = customerPhone;
            ShippingAddress = "In-Store Pickup"; // No shipping for in-store
            PaymentMethod = Check.NotNullOrWhiteSpace(paymentMethod, nameof(paymentMethod));

            PaymentStatus = PaymentStatus.Unpaid;
            OrderStatus = OrderStatus.Placed;
            OrderType = OrderType.InStore;
        }

        // PUBLIC METHODS

        public void SetInitialStatus(OrderStatus initialOrderStatus, PaymentStatus initialPaymentStatus)
        {
            OrderStatus = initialOrderStatus;
            PaymentStatus = initialPaymentStatus;
        }

        public void SetStoreInfo(Guid storeId, OrderType orderType)
        {
            StoreId = storeId;
            OrderType = orderType;
        }

        public void UpdateShippingInfo(string customerName, string customerPhone, string shippingAddress)
        {
            CustomerName = Check.NotNullOrWhiteSpace(customerName, nameof(customerName));
            CustomerPhone = customerPhone;
            ShippingAddress = Check.NotNullOrWhiteSpace(shippingAddress, nameof(shippingAddress));
        }

        public void UpdatePaymentMethod(string paymentMethod)
        {
            PaymentMethod = Check.NotNullOrWhiteSpace(paymentMethod, nameof(paymentMethod));
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
                OrderItems.Add(new OrderItem(Id, productId, productName, productPrice, quantity));
            }
            CalculateTotals();
        }

        public void CalculateTotals()
        {
            TotalAmount = OrderItems.Any() ? OrderItems.Sum(oi => oi.LineTotalAmount) : 0;
        }

        public void SetStatus(OrderStatus newStatus, IStringLocalizer<ProductSellingResource> localizer = null)
        {
            if (!IsNextStatusValid(newStatus))
            {
                throw new UserFriendlyException("OrderStatusChangeNotAllowed",
                    $"Không thể chuyển trạng thái từ '{OrderStatus}' sang '{newStatus}'.");
            }
            OrderStatus = newStatus;
        }

        public void SetPaymentStatus(PaymentStatus newPaymentStatus, IStringLocalizer<ProductSellingResource> localizer = null)
        {
            var validTransitions = new Dictionary<PaymentStatus, PaymentStatus[]>
            {
                { PaymentStatus.Unpaid, new[] { PaymentStatus.Pending, PaymentStatus.PendingOnDelivery, PaymentStatus.Paid, PaymentStatus.Cancelled } },
                { PaymentStatus.Pending, new[] { PaymentStatus.Paid, PaymentStatus.Failed, PaymentStatus.Cancelled } },
                { PaymentStatus.PendingOnDelivery, new[] { PaymentStatus.Paid, PaymentStatus.Cancelled } },
                { PaymentStatus.Paid, new[] { PaymentStatus.Refunded } },
                { PaymentStatus.Failed, new[] { PaymentStatus.Pending } },
                { PaymentStatus.Cancelled, new PaymentStatus[] { } },
                { PaymentStatus.Refunded, new PaymentStatus[] { } }
            };

            if (!validTransitions.ContainsKey(PaymentStatus) ||
                !validTransitions[PaymentStatus].Contains(newPaymentStatus))
            {
                throw new UserFriendlyException("InvalidPaymentStatusChange",
                    $"Không thể chuyển trạng thái thanh toán từ '{PaymentStatus}' sang '{newPaymentStatus}'.");
            }

            PaymentStatus = newPaymentStatus;
        }

        public void SetPendingOnDelivery()
        {
            if (PaymentMethod != PaymentMethods.COD)
            {
                throw new UserFriendlyException("OnlyForCODOrders",
                    "Trạng thái PendingOnDelivery chỉ dành cho đơn COD.");
            }
            SetPaymentStatus(PaymentStatus.PendingOnDelivery);
        }

        public void MarkAsPaidOnline()
        {
            if (PaymentStatus == PaymentStatus.Pending)
            {
                PaymentStatus = PaymentStatus.Paid;
                SetStatus(OrderStatus.Confirmed);
            }
        }

        public void MarkAsCodPaidAndCompleted(IStringLocalizer<ProductSellingResource> localizer = null)
        {
            if (PaymentMethod != PaymentMethods.COD)
            {
                throw new UserFriendlyException("ActionNotAllowedForNonCodOrder");
            }
            if (PaymentStatus != PaymentStatus.PendingOnDelivery)
            {
                throw new UserFriendlyException("CannotConfirmPaymentForThisOrder",
                    $"Trạng thái thanh toán phải là '{PaymentStatus.PendingOnDelivery}'.");
            }

            PaymentStatus = PaymentStatus.Paid;
            SetStatus(OrderStatus.Delivered);
        }

        // NEW: In-store workflow methods

        /// <summary>
        /// Cashier completes payment for in-store order
        /// </summary>
        public void CompletePaymentInStore(Guid cashierId, string cashierName, decimal paidAmount)
        {
            if (OrderType != OrderType.InStore)
            {
                throw new UserFriendlyException("OnlyForInStoreOrders",
                    "Phương thức này chỉ dành cho đơn hàng tại cửa hàng.");
            }

            if (OrderStatus != OrderStatus.Placed && OrderStatus != OrderStatus.Pending)
            {
                throw new UserFriendlyException("CannotCompletePayment",
                    $"Chỉ có thể thanh toán cho đơn hàng ở trạng thái Placed hoặc Pending. Trạng thái hiện tại: {OrderStatus}");
            }

            CashierId = cashierId;
            CashierName = cashierName;
            CompletedAt = DateTime.UtcNow;

            PaymentStatus = PaymentStatus.Paid;
            OrderStatus = OrderStatus.Confirmed; // Move to confirmed after payment
        }

        /// <summary>
        /// Warehouse staff fulfills in-store order (gives items to customer)
        /// </summary>
        public void FulfillInStore(Guid fulfillerId, string fulfillerName)
        {
            if (OrderType != OrderType.InStore)
            {
                throw new UserFriendlyException("OnlyForInStoreOrders",
                    "Phương thức này chỉ dành cho đơn hàng tại cửa hàng.");
            }

            if (OrderStatus != OrderStatus.Confirmed && OrderStatus != OrderStatus.Processing)
            {
                throw new UserFriendlyException("CannotFulfillOrder",
                    $"Chỉ có thể giao hàng cho đơn đã thanh toán. Trạng thái hiện tại: {OrderStatus}");
            }

            if (PaymentStatus != PaymentStatus.Paid)
            {
                throw new UserFriendlyException("PaymentNotCompleted",
                    "Đơn hàng chưa được thanh toán.");
            }

            FulfillerId = fulfillerId;
            FulfillerName = fulfillerName;
            FulfilledAt = DateTime.UtcNow;

            OrderStatus = OrderStatus.Delivered; // Final status for in-store orders
        }

        public void CancelByUser(IStringLocalizer<ProductSellingResource> localizer = null)
        {
            if (OrderStatus != OrderStatus.Placed)
            {
                throw new UserFriendlyException(localizer["Order:OrderCanOnlyBeCancelledWhenPlaced"]);
            }
            if (PaymentStatus == PaymentStatus.Paid)
            {
                throw new UserFriendlyException(localizer["Order:CannotCancelPaidOrder"]);
            }

            SetStatus(OrderStatus.Cancelled);
            PaymentStatus = PaymentStatus.Unpaid;
        }

        // PRIVATE METHODS
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
