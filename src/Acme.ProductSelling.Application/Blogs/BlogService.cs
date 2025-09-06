using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;


namespace Acme.ProductSelling.Blogs
{
    public class BlogService : ApplicationService, IBlog
    {
        private readonly IRepository<Blog, Guid> _repo;
        private readonly IIdentityUserRepository _identityUserRepository;
        public BlogService(IRepository<Blog, Guid> repo, IIdentityUserRepository identityUserRepository)
        {
            _repo = repo;
            _identityUserRepository = identityUserRepository;
        }

        public async Task<BlogDto> CreateAsync(CreateAndUpdateBlogDto input)
        {
            var slug = !string.IsNullOrWhiteSpace(input.UrlSlug)
                                ? input.UrlSlug
                                : GenerateSlug(input.Title);

            var authorId = CurrentUser.Id ?? throw new UserFriendlyException("User must be logged in");
            var authorName = !string.IsNullOrWhiteSpace(input.Author)
                ? input.Author
                : CurrentUser.Name ?? CurrentUser.UserName ?? "Unknown";

            var blog = new Blog(
               GuidGenerator.Create(),
               input.Title,
               input.Content,
               input.PublishedDate,
               authorName,
               authorId,
               slug,
               input.MainImageUrl,
               input.MainImageId
           );

            blog = await _repo.InsertAsync(blog, autoSave: true);

            return ObjectMapper.Map<Blog, BlogDto>(blog);
        }
        private string GenerateSlug(string title)
        {
            return title?.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("!", "")
                .Replace("?", "")
                ?? string.Empty;
        }
    }
}
