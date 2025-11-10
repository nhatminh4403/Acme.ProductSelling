using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Products.Lookups;
using System;

namespace Acme.ProductSelling.Specifications.Models
{
    public class KeyboardSpecification : SpecificationBase
    {
        public string KeyboardType { get; set; } // "Mechanical", "Membrane"

        public Guid SwitchTypeId { get; set; }
        public virtual SwitchType SwitchType { get; set; }
        public KeyboardLayout Layout { get; set; } // Dùng Enum
        public virtual ConnectivityType Connectivity { get; set; } // "Wired", "Wireless", "Wired/Wireless"
        public string Backlight { get; set; } // "None", "Single Color", "RGB"
    }
}
