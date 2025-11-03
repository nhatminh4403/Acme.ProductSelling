using Acme.ProductSelling.Categories;


namespace Acme.ProductSelling.Specifications.Models
{
    public class MicrophoneSpecification : SpecificationBase
    {
        public MicrophoneType MicrophoneType { get; set; } // ENUM
        public string PolarPattern { get; set; } // Can be multiple, keep as string or use flags
        public string Frequency { get; set; } // e.g., "20Hz - 20kHz"
        public string SampleRate { get; set; } // e.g., "48kHz/16-bit"
        public string Sensitivity { get; set; } // e.g., "-36dB"
        public ConnectivityType Connectivity { get; set; }
        public string Connection { get; set; } // USB, XLR, 3.5mm
        public bool HasShockMount { get; set; }
        public bool HasPopFilter { get; set; }
        public bool HasRgb { get; set; }
        public string Color { get; set; }
    }
}