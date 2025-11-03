using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class SpeakerSpecification : SpecificationBase
    {
        public SpeakerType SpeakerType { get; set; } // ENUM
        public int TotalWattage { get; set; }
        public string Frequency { get; set; } // e.g., "20Hz - 20kHz"
        public ConnectivityType Connectivity { get; set; }
        public string InputPorts { get; set; } // 3.5mm, RCA, Optical, etc.
        public bool HasBluetooth { get; set; }
        public bool HasRemote { get; set; }
        public string Color { get; set; }
    }
}