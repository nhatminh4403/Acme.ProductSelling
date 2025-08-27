using System;
using System.ComponentModel.DataAnnotations;

namespace Acme.ProductSelling.Comments
{
    public class CreateCommentDto
    {
        [Required]
        public string EntityType { get; set; }
        [Required]
        public Guid EntityId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        public Guid? ParentId { get; set; }
    }
}
