using Acme.ProductSelling.Products;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Categories
{
    public class Category : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Product> Products { get; set; }
        private Category()
        {
        }

        internal Category(Guid id, string name,string description) : base(id)
        {
            Name = name;
            Description = description;
            Products = new HashSet<Products.Product>(); // Khởi tạo collection
        }
    }
}
