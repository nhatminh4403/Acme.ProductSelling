using System.ComponentModel.DataAnnotations;
namespace Acme.ProductSelling.Categories
{
    public class CreateUpdateCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public string UrlSlug { get; set; }
    }
}
