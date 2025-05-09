﻿using Acme.ProductSelling.Products;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Manufacturers
{
    public class Manufacturer : AuditedEntity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; }

        public string ManufacturerImage { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}
