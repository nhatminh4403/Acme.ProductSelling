using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class NetworkHardwareSpecification : SpecificationBase
    {
        public NetworkDeviceType DeviceType { get; set; } // ENUM
        public WifiStandard WifiStandard { get; set; } // ENUM
        public string MaxSpeed { get; set; } // e.g., "3000 Mbps"
        public string Frequency { get; set; } // 2.4GHz, 5GHz, 6GHz
        public string EthernetPorts { get; set; } // e.g., "4x Gigabit LAN"
        public int AntennaCount { get; set; }
        public bool HasUsb { get; set; }
        public string SecurityProtocol { get; set; } // WPA3, WPA2
        public string Coverage { get; set; } // sq ft or meters
    }
}