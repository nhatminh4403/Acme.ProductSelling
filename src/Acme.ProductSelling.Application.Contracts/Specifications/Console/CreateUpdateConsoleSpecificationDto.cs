using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications
{

    public class CreateUpdateConsoleSpecificationDto
    {
        public string Processor { get; set; }
        public string Graphics { get; set; }
        public string RAM { get; set; }
        public string Storage { get; set; }
        public OpticalDriveType OpticalDrive { get; set; }
        public string MaxResolution { get; set; }
        public string MaxFrameRate { get; set; }
        public bool HDRSupport { get; set; }
        public ConnectivityType Connectivity { get; set; }
        public bool HasEthernet { get; set; }
        public string WifiVersion { get; set; }
        public string BluetoothVersion { get; set; }
    }
}
