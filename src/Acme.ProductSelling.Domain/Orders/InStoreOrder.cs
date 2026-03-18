using Acme.ProductSelling.Payments;
using JetBrains.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;

namespace Acme.ProductSelling.Orders
{
    public class InStoreOrder : Order
    {
        // Nullable for online orders
        public Guid? SellerId { get; protected set; }
        [CanBeNull]
        [StringLength(128)]
        public string SellerName { get; protected set; }

        public Guid? CashierId { get; protected set; }
        [CanBeNull]
        [StringLength(128)]
        public string CashierName { get; protected set; }

        public Guid? FulfillerId { get; protected set; }
        [CanBeNull]
        [StringLength(128)]
        public string FulfillerName { get; protected set; }
        public DateTime? CompletedAt { get; protected set; } // When cashier processed payment
        public DateTime? FulfilledAt { get; protected set; } // When warehouse gave items


        protected InStoreOrder() { }

        public InStoreOrder(Guid id, string orderNumber, DateTime orderDate,
            Guid storeId, Guid? sellerId, string sellerName,
            string customerName, string customerPhone, string paymentMethod)
            : base(id, orderNumber, orderDate, null, customerName, customerPhone, paymentMethod)
        {
            StoreId = storeId;
            SellerId = sellerId;
            SellerName = sellerName;
            OrderType = OrderType.InStore;
            OrderStatus = OrderStatus.Placed;
        }


        public void CompletePaymentInStore(Guid cashierId, string cashierName, DateTime completedAt)
        {
            if (OrderType != OrderType.InStore)
            {
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.OrderOnlyForInStoreOrders);
            }

            if (OrderStatus != OrderStatus.Placed && OrderStatus != OrderStatus.Pending)
            {
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.OrderCannotCompletePayment)
                    .WithData("Status", OrderStatus);
            }

            CashierId = cashierId;
            CashierName = cashierName;
            CompletedAt = completedAt;

            PaymentStatus = PaymentStatus.Paid;
            OrderStatus = OrderStatus.Confirmed;
        }

        public void FulfillInStore(Guid fulfillerId, string fulfillerName, DateTime fulfilledAt)
        {
            if (OrderType != OrderType.InStore)
            {
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.OrderOnlyForInStoreOrders);
            }

            if (OrderStatus != OrderStatus.Confirmed && OrderStatus != OrderStatus.Processing)
            {
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.OrderCannotFulfillOrder)
                    .WithData("Status", OrderStatus);
            }

            if (PaymentStatus != PaymentStatus.Paid)
            {
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.OrderPaymentNotCompleted);
            }

            FulfillerId = fulfillerId;
            FulfillerName = fulfillerName;
            FulfilledAt = fulfilledAt;

            OrderStatus = OrderStatus.Delivered; // Final status for in-store orders
        }
    }
}
