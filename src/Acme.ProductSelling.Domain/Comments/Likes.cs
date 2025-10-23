using System;
using Volo.Abp.Domain.Entities;

namespace Acme.ProductSelling.Comments
{
    public class Likes : Entity
    {
        public Guid CommentId { get; protected set; }
        public Guid UserId { get; protected set; }

        public Likes(Guid commentId, Guid userId)
        {
            CommentId = commentId;
            UserId = userId;
        }

        protected Likes()
        {
        }

        public override object[] GetKeys()
        {
            return new object[] { CommentId, UserId };
        }
    }
}
