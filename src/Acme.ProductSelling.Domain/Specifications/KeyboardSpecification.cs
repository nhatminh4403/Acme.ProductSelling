using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Products.Specs;
using System;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Specifications
{
    public class KeyboardSpecification : SpecificationBase
    {
        public string KeyboardType { get; set; } // "Mechanical", "Membrane"

        public Guid SwitchTypeId { get; set; }
        public virtual SwitchType SwitchType { get; set; }
        public KeyboardLayout Layout { get; set; } // Dùng Enum
        public string Connectivity { get; set; } // "Wired", "Wireless", "Wired/Wireless"
        public string Backlight { get; set; } // "None", "Single Color", "RGB"
    }
}
