using Acme.ProductSelling.Categories;
namespace Acme.ProductSelling.Specifications
{
    public class CreateUpdateMicrophoneSpecificationDto
    {
        public MicrophoneType MicrophoneType { get; set; }
        public string PolarPattern { get; set; }
        public string Frequency { get; set; }
        public string SampleRate { get; set; }
        public string Sensitivity { get; set; }
        public ConnectivityType Connectivity { get; set; }
        public string Connection { get; set; }
        public bool HasShockMount { get; set; }
        public bool HasPopFilter { get; set; }
        public bool HasRgb { get; set; }
        public string Color { get; set; }
    }
}