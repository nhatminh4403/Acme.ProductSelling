using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Payments;
using Microsoft.Extensions.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;

namespace Acme.ProductSelling.Orders
{
    public class OnlineOrder : Order
    {
        [Required]
        public string ShippingAddress { get; private set; }
        protected OnlineOrder() { }

        public OnlineOrder(Guid id,
            string orderNumber,
            DateTime orderDate,
            Guid? customerId,
            string customerName,
            string customerPhone,
            string shippingAddress,
            string paymentMethod)
            : base(id, orderNumber, orderDate, customerId, customerName, customerPhone, paymentMethod)
        {
            ShippingAddress = Check.NotNullOrWhiteSpace(shippingAddress, nameof(shippingAddress));
            OrderType = OrderType.Online;
            OrderStatus = OrderStatus.Placed;
        }
        public void SetInitialStatus(OrderStatus initialOrderStatus, PaymentStatus initialPaymentStatus)
        {
            OrderStatus = initialOrderStatus;
            PaymentStatus = initialPaymentStatus;
        }
        public void UpdateShippingInfo(string customerName, string customerPhone, string shippingAddress)
        {
            CustomerName = Check.NotNullOrWhiteSpace(customerName, nameof(customerName));
            CustomerPhone = customerPhone;
            ShippingAddress = Check.NotNullOrWhiteSpace(shippingAddress, nameof(shippingAddress));
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

            PaymentStatus = PaymentStatus.Paid;
            SetStatus(OrderStatus.Delivered);
        }

        public void CancelByUser()
        {
            if (OrderStatus != OrderStatus.Placed && OrderStatus != OrderStatus.Pending)
            {
                throw new UserFriendlyException("Order:OrderCanOnlyBeCancelledWhenPlacedOrPending");
            }

            // Still prevent cancelling if already PAID (for online orders)
            if (PaymentStatus == PaymentStatus.Paid)
            {
                throw new UserFriendlyException("Order:CannotCancelPaidOrder");
            }

            SetStatus(OrderStatus.Cancelled);
            PaymentStatus = PaymentStatus.Unpaid;
        }
    }
}
