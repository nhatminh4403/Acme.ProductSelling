using Acme.ProductSelling.Products;
using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Manufacturers
{
    public class Manufacturer : FullAuditedEntity<Guid>
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; }
        public string UrlSlug { get; set; }

        public string ManufacturerImage { get; set; }
        public ICollection<Product> Products { get; set; }

        public Manufacturer() { }

        public Manufacturer(Guid id,
                            string name,
                            string urlSlug,
                            string description = null,
                            string contactInfo = null,
                            string manufacturerImage = null) : base(id)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name));
            UrlSlug = Check.NotNullOrWhiteSpace(urlSlug, nameof(urlSlug));
            Description = description;
            ContactInfo = contactInfo;
            ManufacturerImage = manufacturerImage;
            Products = new HashSet<Product>();
        }

    }
}
