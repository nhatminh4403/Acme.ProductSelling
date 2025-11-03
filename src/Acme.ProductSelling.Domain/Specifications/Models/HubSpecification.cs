using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class HubSpecification : SpecificationBase
    {
        public HubType HubType { get; set; } // ENUM
        public int PortCount { get; set; }
        public string UsbAPorts { get; set; } // e.g., "3x USB-A 3.0"
        public string UsbCPorts { get; set; } // e.g., "2x USB-C"
        public int HdmiPorts { get; set; }
        public int DisplayPorts { get; set; }
        public bool EthernetPort { get; set; }
        public bool SdCardReader { get; set; }
        public bool AudioJack { get; set; }
        public int MaxDisplays { get; set; }
        public string MaxResolution { get; set; }
        public string PowerDelivery { get; set; } // e.g., "85W"
        public string Color { get; set; }
    }
}