using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Comments
{
    public class Comment : FullAuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public DateTime CommentedOn { get; set; }
        public string Content { get; set; }
        public string EntityType { get; protected set; }
        public Guid EntityId { get; protected set; }
        public Guid? ParentId { get; set; }


        // Constructor
        protected Comment() { }
        public Comment(Guid id, Guid userId, DateTime commentedOn,
            string content, string entityType, Guid entityId, Guid? parentId = null) : base(id)
        {
            UserId = userId;
            CommentedOn = commentedOn;
            Content = content;
            EntityType = entityType;
            EntityId = entityId;
            ParentId = parentId;
        }



    }
}
