using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Blogs
{
    public class UpdateBlogDto : CreateBlogDto
    {
        public Guid Id { get; set; }
        public string Author { get; set; }

    }
}
