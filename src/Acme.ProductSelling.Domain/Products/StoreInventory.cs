using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Products
{
    public class StoreInventory : FullAuditedAggregateRoot<Guid>
    {
        public Guid StoreId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public int ReorderQuantity { get; set; }

        protected StoreInventory()
        {
        }

        public StoreInventory(
            Guid id,
            Guid storeId,
            Guid productId,
            int quantity) : base(id)
        {
            StoreId = storeId;
            ProductId = productId;
            Quantity = quantity;
            ReorderLevel = 10;
            ReorderQuantity = 50;
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
                throw new InvalidOperationException("Insufficient stock");

            Quantity -= quantity;
        }

        public bool NeedsReorder()
        {
            return Quantity <= ReorderLevel;
        }
    }
}
