using System;
using System.ComponentModel.DataAnnotations;
namespace Acme.ProductSelling.Comments
{
    public class CommentListDto
    {
        [Required]
        public string EntityType { get; set; }
        [Required]
        public Guid EntityId { get; set; }
    }
}
