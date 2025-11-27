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
        public CategoryGroup CategoryGroup { get; set; } = CategoryGroup.Individual;
        public int DisplayOrder { get; set; }
        public string IconCssClass { get; set; }
        public string UrlSlug { get; set; }
        public ICollection<Product> Products { get; set; }
        protected Category()
        {
        }
        public Category(Guid id,
                        string name,
                        string description,
                        string urlSlug,
                        SpecificationType specType = SpecificationType.None,
                        CategoryGroup categoryGroup = CategoryGroup.Individual,
                        int displayOrder = 0,
                        string iconCssClass = "fas fa-box") : base(id)
        {
            Name = name;
            Description = description;
            SpecificationType = specType; // Gán loại spec khi tạo
            Products = new HashSet<Product>();
            UrlSlug = urlSlug;
            CategoryGroup = categoryGroup;
            DisplayOrder = displayOrder;
            IconCssClass = iconCssClass;
        }
    }
}
