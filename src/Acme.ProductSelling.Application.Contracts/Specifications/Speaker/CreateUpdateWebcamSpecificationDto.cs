using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications
{

    public class CreateUpdateSpeakerSpecificationDto
    {
        public SpeakerType SpeakerType { get; set; }
        public int TotalWattage { get; set; }
        public string Frequency { get; set; }
        public ConnectivityType Connectivity { get; set; }
        public string InputPorts { get; set; }
        public string Color { get; set; }
    }
}