using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Guids;

namespace Acme.ProductSelling.Carts
{
    public class Cart : AuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public ICollection<CartItem> Items { get; set; }
        protected Cart()
        {
            Items = new HashSet<CartItem>();
        }
        public Cart(Guid id, Guid userId) : base(id)
        {
            UserId = userId;
            Items = new HashSet<CartItem>();
        }
        public virtual void AddOrUpdateItem(Guid productId, int quantity, IGuidGenerator guidGenerator, string productName, decimal productPrice)
        {
            Check.Range(quantity, nameof(quantity), 1, int.MaxValue);

            var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem == null)
            {
                var newItem = new CartItem(guidGenerator.Create(), Id, productId, quantity, productName, productPrice);
                Items.Add(newItem);
            }
            else
            {
                existingItem.SetQuantity(existingItem.Quantity + quantity);
            }
        }
        public virtual void RemoveItem(Guid cartItemId)
        {
            var itemToRemove = Items.FirstOrDefault(i => i.Id == cartItemId);
            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
            }
        }

        // Phương thức xóa sạch giỏ hàng
        public virtual void ClearItems()
        {
            Items.Clear();
        }
    }
}
