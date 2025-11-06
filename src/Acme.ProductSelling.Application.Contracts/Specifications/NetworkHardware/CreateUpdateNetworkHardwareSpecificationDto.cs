
using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.NetworkHardware
{
    public class CreateUpdateNetworkHardwareSpecificationDto
    {
        public NetworkDeviceType DeviceType { get; set; }
        public WifiStandard WifiStandard { get; set; }
        public string MaxSpeed { get; set; }
        public string Frequency { get; set; }
        public string EthernetPorts { get; set; }
        public int AntennaCount { get; set; }
        public bool HasUsb { get; set; }
        public string SecurityProtocol { get; set; }
        public string Coverage { get; set; }
    }
}
