using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications
{
    public class CreateUpdateSoftwareSpecificationDto
    {
        public SoftwareType SoftwareType { get; set; }
        public LicenseType LicenseType { get; set; }
        public Platform Platform { get; set; }
        public string Version { get; set; }
        public string Language { get; set; }
        public string DeliveryMethod { get; set; }
        public string SystemRequirements { get; set; }
        public bool IsSubscription { get; set; }
    }
}