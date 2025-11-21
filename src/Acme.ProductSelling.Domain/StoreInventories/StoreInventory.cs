using Acme.ProductSelling.Products;
using Acme.ProductSelling.Stores;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.StoreInventories
{
    public class StoreInventory : FullAuditedAggregateRoot<Guid>
    {
        public Guid StoreId { get; protected set; }
        public Guid ProductId { get; protected set; }
        public int Quantity { get; protected set; }
        public int ReorderLevel { get; protected set; }
        public int ReorderQuantity { get; protected set; }
        public bool IsAvailableForSale { get; protected set; }

        // Navigation properties
        public virtual Store Store { get; set; }
        public virtual Product Product { get; set; }

        protected StoreInventory()
        {
        }

        public StoreInventory(
            Guid id,
            Guid storeId,
            Guid productId,
            int quantity,
            int reorderLevel = 10,
            int reorderQuantity = 50) : base(id)
        {
            StoreId = storeId;
            ProductId = productId;
            Quantity = quantity;
            ReorderLevel = reorderLevel;
            ReorderQuantity = reorderQuantity;
            IsAvailableForSale = true;
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            Quantity += quantity;
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            if (Quantity < quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {Quantity}, Requested: {quantity}");

            Quantity -= quantity;
        }

        public void SetQuantity(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

            Quantity = quantity;
        }

        public bool NeedsReorder()
        {
            return Quantity <= ReorderLevel;
        }

        public void SetAvailability(bool isAvailable)
        {
            IsAvailableForSale = isAvailable;
        }

        public void UpdateReorderSettings(int reorderLevel, int reorderQuantity)
        {
            if (reorderLevel < 0 || reorderQuantity < 0)
                throw new ArgumentException("Reorder values cannot be negative");

            ReorderLevel = reorderLevel;
            ReorderQuantity = reorderQuantity;
        }
    }
}