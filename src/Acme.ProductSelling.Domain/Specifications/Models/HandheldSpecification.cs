

using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class HandheldSpecification : SpecificationBase
    {

        public string Processor { get; set; }
        public string Graphics { get; set; }
        public string RAM { get; set; }
        public string Storage { get; set; }
        public string Display { get; set; }
        public string BatteryLife { get; set; }
        public string Weight { get; set; } // grams
        public string OperatingSystem { get; set; }
        public ConnectivityType Connectivity { get; set; } // CHANGED: WirelessAndBluetooth typically
        public string WifiVersion { get; set; } // e.g., "WiFi 5", "WiFi 6"
        public string BluetoothVersion { get; set; } // e.g., "5.0", "5.2"
    }
}