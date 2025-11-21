using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications
{
    public class CreateUpdateChargerSpecificationDto
    {
        public ChargerType ChargerType { get; set; }
        public int TotalWattage { get; set; }
        public int PortCount { get; set; }
        public int UsbCPorts { get; set; }
        public int UsbAPorts { get; set; }
        public string MaxOutputPerPort { get; set; }
        public string FastChargingProtocols { get; set; }
        public bool CableIncluded { get; set; }
        public bool HasFoldablePlug { get; set; }
        public string Technology { get; set; }
        public string Color { get; set; }
    }
}
