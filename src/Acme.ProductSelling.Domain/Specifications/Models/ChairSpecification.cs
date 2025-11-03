using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class ChairSpecification : SpecificationBase
    {
        public string ChairType { get; set; } // Gaming, Office, Executive
        public ChairMaterial Material { get; set; } // ENUM
        public int MaxWeight { get; set; } // kg
        public ArmrestType ArmrestType { get; set; } // ENUM
        public string BackrestAdjustment { get; set; } // e.g., "90°-160°"
        public string SeatHeight { get; set; } // e.g., "42-52cm"
        public bool HasLumbarSupport { get; set; }
        public bool HasHeadrest { get; set; }
        public string BaseType { get; set; } // Nylon, Aluminum
        public string WheelType { get; set; } // PU caster, Rubber
        public string Color { get; set; }
    }
}