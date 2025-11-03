using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class SoftwareSpecification : SpecificationBase
    {
        public SoftwareType SoftwareType { get; set; } // ENUM
        public LicenseType LicenseType { get; set; } // ENUM
        public Platform Platform { get; set; } // ENUM
        public string Version { get; set; }
        public string Language { get; set; } // Can be multi-select
        public string DeliveryMethod { get; set; } // Digital, Physical, Both
        public string SystemRequirements { get; set; }
        public bool IsSubscription { get; set; }
    }
}