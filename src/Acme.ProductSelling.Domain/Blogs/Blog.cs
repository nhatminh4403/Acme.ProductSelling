using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.ProductSelling.Blogs
{
    public class Blog : FullAuditedAggregateRoot<Guid>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Author { get; set; }
        public string UrlSlug { get; set; }
        public Guid AuthorId { get; set; }
        // Constructor
        public string? MainImageUrl { get; set; }
        public Guid? MainImageId { get; set; } // Thêm thuộc tính MainImageId


        protected Blog() { }

        public Blog(Guid id, string title, string content, DateTime publishedDate,
            string author, Guid authorId, string slug, string? mainImageUrl, Guid? mainImageId) : base(id)
        {
            Title = title;
            Content = content;
            PublishedDate = publishedDate;
            Author = author;
            AuthorId = authorId;
            UrlSlug = slug;
            MainImageUrl = mainImageUrl;
            MainImageId = mainImageId;
        }
        public void UpdateMainImage(string? imageUrl, Guid? imageId)
        {
            MainImageUrl = imageUrl;
            MainImageId = imageId;
        }

        public void UpdateSlug(string slug)
        {
            UrlSlug = slug;
        }
    }
}
