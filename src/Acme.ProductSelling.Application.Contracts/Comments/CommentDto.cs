using System;
using Volo.Abp.Application.Dtos;
namespace Acme.ProductSelling.Comments
{
    public class CommentDto : AuditedEntityDto<Guid>
    {
        public string Commenter { get; set; }
        public DateTime CommentedOn { get; set; }
        public string Content { get; set; }
        public string EntityType { get; private set; }
        public Guid EntityId { get; private set; }
        public Guid? ParentId { get; set; }
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }
}
