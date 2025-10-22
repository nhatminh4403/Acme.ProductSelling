using System;
namespace Acme.ProductSelling.Blogs
{
    public class CreateAndUpdateBlogDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string UrlSlug { get; set; }
        public string? MainImageUrl { get; set; }
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public string Author { get; set; }
        public Guid AuthorId { get; set; }
        public Guid? MainImageId { get; set; }
    }
}
