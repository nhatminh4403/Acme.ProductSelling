using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Models
{
    public class CaseFanSpecification : SpecificationBase
    {
        public int FanSize { get; set; } // 120mm, 140mm, etc.
        public int MaxRpm { get; set; }
        public float NoiseLevel { get; set; } // dB(A)
        public float Airflow { get; set; } // CFM
        public float StaticPressure { get; set; } // mm H2O
        public string Connector { get; set; } // 3-pin, 4-pin PWM
        public BearingType BearingType { get; set; } // ENUM
        public bool HasRgb { get; set; }
        public string Color { get; set; }
    }
}
