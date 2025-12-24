using System;
using System.Collections.Generic;
using System.Linq;

namespace Acme.ProductSelling.Products.Dtos
{
    public class GetProductByNameWithPriceDto : GetProductByNameDto
    {
        public decimal MinPrice { get; set; }   // Min price
        public decimal MaxPrice { get; set; }
        public Guid? ManufacturerId { get; set; }
        public List<Guid> ManufacturerIds { get; set; } = new List<Guid>();

        // Helper property for empty check
        public bool HasManufacturerFilter => ManufacturerIds != null && ManufacturerIds.Any();
    }
}
