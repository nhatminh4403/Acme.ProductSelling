using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.CaseFan
{
    public class CreateUpdateCaseFanSpecificationDto
    {
        public int FanSize { get; set; }
        public int MaxRpm { get; set; }
        public float NoiseLevel { get; set; }
        public float Airflow { get; set; }
        public float StaticPressure { get; set; }
        public string Connector { get; set; }
        public BearingType BearingType { get; set; }
        public bool HasRgb { get; set; }
        public string Color { get; set; }
    }
}
