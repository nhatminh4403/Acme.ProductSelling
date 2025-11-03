using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class DeskSpecification : SpecificationBase
    {
        public int Width { get; set; } // cm
        public int Depth { get; set; } // cm
        public float Height { get; set; } // cm (can be range if adjustable)
        public DeskMaterial Material { get; set; } // ENUM
        public int MaxWeight { get; set; } // kg
        public bool IsHeightAdjustable { get; set; }
        public bool HasCableManagement { get; set; }
        public bool HasCupHolder { get; set; }
        public bool HasHeadphoneHook { get; set; }
        public string SurfaceType { get; set; }
        public string Color { get; set; }
    }
}