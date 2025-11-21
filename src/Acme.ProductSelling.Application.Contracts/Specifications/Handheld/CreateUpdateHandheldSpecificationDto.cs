using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications
{
    public class CreateUpdateHandheldSpecificationDto
    {
        public string Processor { get; set; }
        public string Graphics { get; set; }
        public string RAM { get; set; }
        public string Storage { get; set; }
        public string Display { get; set; }
        public string BatteryLife { get; set; }
        public string Weight { get; set; }
        public string OperatingSystem { get; set; }
        public ConnectivityType Connectivity { get; set; }
        public string WifiVersion { get; set; }
        public string BluetoothVersion { get; set; }
    }
}
