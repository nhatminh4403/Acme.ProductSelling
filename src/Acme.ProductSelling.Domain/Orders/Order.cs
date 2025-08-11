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

        // --- Các trạng thái ---
        public OrderStatus Status { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }

        public virtual ICollection<OrderItem> OrderItems { get; protected set; } = new HashSet<OrderItem>();

        private Order() { /* Required by EF Core */ }

        // --- CONSTRUCTOR ĐƯỢC RÚT GỌN ---
        public Order(
            Guid id, string orderNumber, DateTime orderDate, Guid? customerId,
            string customerName, string customerPhone, string shippingAddress, string paymentMethod)
            : base(id)
        {
            OrderNumber = Check.NotNullOrWhiteSpace(orderNumber, nameof(orderNumber));
            OrderDate = orderDate;
            CustomerId = customerId;
            CustomerName = Check.NotNullOrWhiteSpace(customerName, nameof(customerName));
            CustomerPhone = customerPhone;
            ShippingAddress = Check.NotNullOrWhiteSpace(shippingAddress, nameof(shippingAddress));
            PaymentMethod = Check.NotNullOrWhiteSpace(paymentMethod, nameof(paymentMethod));

            // AppService sẽ chịu trách nhiệm set các trạng thái ban đầu
            // Mặc định, một đơn hàng vừa tạo luôn có PaymentStatus.Unpaid
            PaymentStatus = PaymentStatus.Unpaid;
        }

        // --- CÁC PHƯƠNG THỨC NGHIỆP VỤ (PUBLIC METHODS) ---

        public void SetInitialStatus(OrderStatus initialStatus, PaymentStatus initialPaymentStatus)
        {
            Status = initialStatus;
            PaymentStatus = initialPaymentStatus;
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

        public void SetStatus(OrderStatus newStatus)
        {
            if (!IsNextStatusValid(newStatus))
            {
                // Thông điệp lỗi chung chung, AppService sẽ xử lý việc dịch
                throw new UserFriendlyException("OrderStatusChangeNotAllowed",
                    $"Không thể chuyển trạng thái từ '{Status}' sang '{newStatus}'.");
            }
            Status = newStatus;
        }

        /// <summary>
        /// Dành cho các cổng thanh toán online sau khi giao dịch được xác nhận.
        /// </summary>
        public void MarkAsPaidOnline()
        {
            if (PaymentStatus == PaymentStatus.Pending)
            {
                PaymentStatus = PaymentStatus.Paid;
                // Sau khi thanh toán online thành công, đơn hàng nên được tự động xác nhận
                SetStatus(OrderStatus.Confirmed);
            }
        }
        public void SetPaymentStatus(PaymentStatus newPaymentStatus)
        {
            // Chỉ cho phép chuyển sang trạng thái thanh toán đã hoàn thành hoặc thất bại
            if (newPaymentStatus != PaymentStatus.Paid && newPaymentStatus != PaymentStatus.Failed)
            {
                throw new UserFriendlyException("InvalidPaymentStatusChange",
                    $"Chỉ có thể chuyển sang '{PaymentStatus.Paid}' hoặc '{PaymentStatus.Failed}'.");
            }
            PaymentStatus = newPaymentStatus;
            // Nếu thanh toán đã hoàn thành, chuyển trạng thái đơn hàng sang 'Confirmed'
        }
        /// <summary>
        /// Dành cho admin xác nhận đã thu tiền đơn COD.
        /// </summary>
        public void MarkAsCodPaidAndCompleted()
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

            PaymentStatus = PaymentStatus.Paid; // Đã đổi thành Paid
            SetStatus(OrderStatus.Delivered);   // Đã hoàn thành
        }

        public void CancelByUser(IStringLocalizer<ProductSellingResource> localizer = null)
        {
            // Người dùng chỉ có thể hủy khi đơn hàng ở trạng thái 'Placed'
            // và thanh toán chưa được thực hiện (cho chắc chắn).
            if (Status != OrderStatus.Placed)
            {
                throw new UserFriendlyException("OrderCanOnlyBeCancelledWhenPlaced");
            }
            if (PaymentStatus == PaymentStatus.Paid)
            {
                throw new UserFriendlyException("CannotCancelPaidOrder");
            }

            SetStatus(OrderStatus.Cancelled);
            PaymentStatus = PaymentStatus.Unpaid; // Hoặc một trạng thái hủy khác
        }

        // --- CÁC PHƯƠNG THỨC RIÊNG TƯ (PRIVATE METHODS) ---

        private bool IsNextStatusValid(OrderStatus newStatus)
        {
            // Cho phép hủy từ các trạng thái nhất định
            if (newStatus == OrderStatus.Cancelled &&
                (Status == OrderStatus.Placed || Status == OrderStatus.Pending || Status == OrderStatus.Confirmed))
            {
                return true;
            }

            // Quy tắc chung: Trạng thái mới phải lớn hơn hoặc bằng trạng thái cũ
            return (int)newStatus >= (int)Status;
        }
    }
}