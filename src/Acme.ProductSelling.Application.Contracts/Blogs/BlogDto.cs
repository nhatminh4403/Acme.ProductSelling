using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Blogs
{
    public class BlogDto : AuditedEntityDto<Guid>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public string UrlSlug { get; set; }
        public string? MainImageUrl { get; set; }
        public Guid? MainImageId { get; set; } // Thêm thuộc tính MainImageId
    }
}
