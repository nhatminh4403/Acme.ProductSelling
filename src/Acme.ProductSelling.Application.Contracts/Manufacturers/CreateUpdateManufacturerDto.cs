namespace Acme.ProductSelling.Manufacturers
{
    public class CreateUpdateManufacturerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public string ContactInfo { get; set; }

        public string ManufacturerImage { get; set; }
    }
}
