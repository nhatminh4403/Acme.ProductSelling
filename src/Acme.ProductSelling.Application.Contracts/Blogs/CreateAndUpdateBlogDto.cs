using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Blogs
{
    public class CreateAndUpdateBlogDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Slug { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime PostedOn { get; set; }
        public string Author { get; set; }

        public Guid? ImageId { get; set; } // Thêm thuộc tính ImageId
    }
}
