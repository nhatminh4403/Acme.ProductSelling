

using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class HeadsetSpecification : SpecificationBase
    {
        public ConnectivityType Connectivity { get; set; }
        public bool HasMicrophone { get; set; }
        public bool IsSurroundSound { get; set; }
        public bool IsNoiseCancelling { get; set; } // Có thể tách biệt cho mic và tai nghe
        public int DriverSize { get; set; } // mm, có thể là string nếu không chắc chắn là số
        public string Frequency { get; set; } // "20Hz - 20kHz"
        public string MicrophoneType { get; set; } // "Omnidirectional", "Unidirectional"
        public int Impedance { get; set; } // Ohm
        public int Sensitivity { get; set; } // dB
        public string Color { get; set; } // "Black", "White", "RGB"


    }
}
