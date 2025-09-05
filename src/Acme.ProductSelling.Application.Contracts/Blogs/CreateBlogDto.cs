using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Blogs
{
    public class CreateBlogDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Slug { get; set; }
        public string? MainImageUrl { get; set; }
        public DateTime PostedOn { get; set; }

        public Guid? ImageId { get; set; } // Thêm thuộc tính ImageId
    }
}
