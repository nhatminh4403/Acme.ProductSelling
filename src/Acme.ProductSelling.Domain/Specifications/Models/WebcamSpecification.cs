using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class WebcamSpecification : SpecificationBase
    {
        public string Resolution { get; set; } // "1920x1080", "3840x2160"
        public int FrameRate { get; set; } // 30, 60, etc.
        public FocusType FocusType { get; set; } // ENUM
        public int FieldOfView { get; set; } // degrees
        public ConnectivityType Connectivity { get; set; }
        public string Connection { get; set; } // USB-A, USB-C
        public bool HasMicrophone { get; set; }
        public bool HasPrivacyShutter { get; set; }
        public string MountType { get; set; } // Monitor clip, Tripod
        public string Color { get; set; }
    }
}