using System.ComponentModel.DataAnnotations;
namespace Acme.ProductSelling.Categories.Dtos
{
    public class CreateUpdateCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public SpecificationType SpecificationType { get; set; } = SpecificationType.None;
        public CategoryGroup CategoryGroup { get; set; } = CategoryGroup.Individual;
        public int DisplayOrder { get; set; }
        public string IconCssClass { get; set; }
        public string UrlSlug { get; set; }
    }
}
