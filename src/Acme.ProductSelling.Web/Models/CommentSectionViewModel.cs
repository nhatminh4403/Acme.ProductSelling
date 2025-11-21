using Acme.ProductSelling.Comments;
using System;
using System.Collections.Generic;

namespace Acme.ProductSelling.Web.Models
{
    public class CommentSectionViewModel
    {
        public string EntityType { get; set; }
        public Guid EntityId { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
