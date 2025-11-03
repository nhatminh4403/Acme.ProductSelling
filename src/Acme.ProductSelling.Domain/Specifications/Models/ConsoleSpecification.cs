using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class ConsoleSpecification : SpecificationBase
    {
        public string Processor { get; set; }
        public string Graphics { get; set; }
        public string RAM { get; set; }
        public string Storage { get; set; }
        public OpticalDriveType OpticalDrive { get; set; }
        public string MaxResolution { get; set; } // 4K, 8K
        public string MaxFrameRate { get; set; } // 60fps, 120fps
        public bool HDRSupport { get; set; }
        public ConnectivityType Connectivity { get; set; } // CHANGED: WiredWirelessAndBluetooth
        public bool HasEthernet { get; set; }
        public string WifiVersion { get; set; } // e.g., "WiFi 6"
        public string BluetoothVersion { get; set; } // e.g., "5.1"
    }
}