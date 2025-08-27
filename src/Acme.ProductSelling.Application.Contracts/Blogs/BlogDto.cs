using System;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Blogs
{
    public class BlogDto : AuditedEntityDto<Guid>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishedDate { get; set; }
        public string AuthorName { get; set; }
    }
}
