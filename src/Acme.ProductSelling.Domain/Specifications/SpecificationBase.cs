using Acme.ProductSelling.Products;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications
{
    public abstract class SpecificationBase : Entity<Guid>
    {
        [ForeignKey(nameof(Product))]
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        protected SpecificationBase()
        {
        }
        protected SpecificationBase(Guid id, Guid productId) : base(id)
        {
            ProductId = productId;
        }
    }
}
