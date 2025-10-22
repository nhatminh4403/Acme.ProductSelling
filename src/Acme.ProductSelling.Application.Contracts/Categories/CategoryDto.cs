using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Categories
{
    public class CategoryDto : AuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlSlug { get; set; } // Đường dẫn URL thân thiện
    }
}
