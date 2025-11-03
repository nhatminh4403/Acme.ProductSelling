using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class MousePadSpecification : SpecificationBase
    {
        public int Width { get; set; } // mm
        public int Height { get; set; } // mm
        public float Thickness { get; set; } // mm
        public MousePadMaterial Material { get; set; } // ENUM
        public SurfaceType SurfaceType { get; set; } // ENUM
        public string BaseType { get; set; } // Non-slip rubber, etc.
        public bool HasRgb { get; set; }
        public bool IsWashable { get; set; }
        public string Color { get; set; }
    }
}