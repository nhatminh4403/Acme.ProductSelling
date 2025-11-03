using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class ChargerSpecification : SpecificationBase
    {
        public ChargerType ChargerType { get; set; } // ENUM
        public int TotalWattage { get; set; }
        public int PortCount { get; set; }
        public int UsbCPorts { get; set; }
        public int UsbAPorts { get; set; }
        public string MaxOutputPerPort { get; set; }
        public string FastChargingProtocols { get; set; } // Can be multiple
        public bool CableIncluded { get; set; }
        public bool HasFoldablePlug { get; set; }
        public string Technology { get; set; } // GaN, etc.
        public string Color { get; set; }
    }
}