using System;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Products
{
    public class RecentlyViewedProduct : Entity<Guid>
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public DateTime ViewedAt { get; set; }

        // Navigation property
        public virtual Product Product { get; set; }

        protected RecentlyViewedProduct()
        {
        }

        public RecentlyViewedProduct(
            Guid id,
            Guid userId,
            Guid productId,
            DateTime viewedAt) : base(id)
        {
            UserId = userId;
            ProductId = productId;
            ViewedAt = viewedAt;
        }

        public void UpdateViewedTime()
        {
            ViewedAt = DateTime.UtcNow;
        }
    }
}
