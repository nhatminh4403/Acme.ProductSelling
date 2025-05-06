using Acme.ProductSelling.Products;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
namespace Acme.ProductSelling.Categories
{
    public class Category : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public SpecificationType SpecificationType { get; set; } = SpecificationType.None;
        public ICollection<Product> Products { get; set; }
        private Category()
        {
        }
        internal Category(Guid id, string name, string description, SpecificationType specType = SpecificationType.None) : base(id)
        {
            Name = name;
            Description = description;
            SpecificationType = specType; // Gán loại spec khi tạo
            Products = new HashSet<Product>();
        }
    }
}
