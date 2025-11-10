using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Cable
{
    public class CreateUpdateCableSpecificationDto
    {
        public CableType CableType { get; set; }
        public float Length { get; set; }
        public string MaxPower { get; set; }
        public string DataTransferSpeed { get; set; }
        public string Connector1 { get; set; }
        public string Connector2 { get; set; }
        public bool IsBraided { get; set; }
        public string Color { get; set; }
        public string Warranty { get; set; }
    }
}
