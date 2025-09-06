using Acme.ProductSelling.Blogs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Controllers
{
    [Route("api/blogs")]
    public class BlogController : AbpController
    {
        private readonly IBlog _blogRepository;

        public BlogController(IBlog blogRepository)
        {
            _blogRepository = blogRepository;
        }

        [HttpPost]
        public async Task<BlogDto> CreateAsync(CreateAndUpdateBlogDto input)
        {
            return await _blogRepository.CreateAsync(input);
        }
    }
}
