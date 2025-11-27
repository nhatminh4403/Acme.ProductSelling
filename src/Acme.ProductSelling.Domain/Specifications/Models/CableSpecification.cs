using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class CableSpecification : SpecificationBase
    {
        public CableType CableType { get; set; } // ENUM
        public float Length { get; set; } // meters
        public string MaxPower { get; set; } // e.g., "100W"
        public string DataTransferSpeed { get; set; } // e.g., "10 Gbps"
        public string Connector1 { get; set; }
        public string Connector2 { get; set; }
        public bool IsBraided { get; set; }
        public string Color { get; set; }
        public string Warranty { get; set; }


    }
}
