using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications
{

    public class CreateUpdateWebcamSpecificationDto
    {
        public string Resolution { get; set; }
        public int FrameRate { get; set; }
        public FocusType FocusType { get; set; }
        public int FieldOfView { get; set; }
        public ConnectivityType Connectivity { get; set; }
        public string Connection { get; set; }
        public bool HasMicrophone { get; set; }
        public bool HasPrivacyShutter { get; set; }
        public string MountType { get; set; }
        public string Color { get; set; }
    }
}