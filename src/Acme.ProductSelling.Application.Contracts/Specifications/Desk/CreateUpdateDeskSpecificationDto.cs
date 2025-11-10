using Acme.ProductSelling.Categories;

namespace Acme.ProductSelling.Specifications.Desk
{
    public class CreateUpdateDeskSpecificationDto
    {
        public int Width { get; set; }
        public int Depth { get; set; }
        public float Height { get; set; }
        public DeskMaterial Material { get; set; }
        public int MaxWeight { get; set; }
        public bool IsHeightAdjustable { get; set; }
        public bool HasCableManagement { get; set; }
        public bool HasCupHolder { get; set; }
        public bool HasHeadphoneHook { get; set; }
        public string SurfaceType { get; set; }
        public string Color { get; set; }
    }
}
