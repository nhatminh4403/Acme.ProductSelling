using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Categories.Dtos
{
    public class CategoryDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlSlug { get; set; } // Đường dẫn URL thân thiện
        public SpecificationType SpecificationType { get; set; } = SpecificationType.None;
        public CategoryGroup CategoryGroup { get; set; } = CategoryGroup.Individual;
        public int DisplayOrder { get; set; }
        public string IconCssClass { get; set; }
    }
}
