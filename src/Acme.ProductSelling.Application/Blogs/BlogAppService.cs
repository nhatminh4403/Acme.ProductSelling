using Acme.ProductSelling.Permissions;
using Ganss.Xss;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
namespace Acme.ProductSelling.Blogs
{
    public class BlogAppService :
        CrudAppService<Blog, BlogDto, Guid, PagedAndSortedResultRequestDto, CreateAndUpdateBlogDto>,
        IBlogAppService
    {
        //private readonly IRepository<Blog, Guid> _blogRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHtmlSanitizer _HtmlSanitizer;
        public BlogAppService(
            IRepository<Blog, Guid> repository,
            IGuidGenerator guidGenerator,
            IWebHostEnvironment webHostEnvironment,
            IHtmlSanitizer htmlSanitizer,
            IBlogRepository blogRepository) : base(repository)
        {
            _blogRepository = blogRepository;
            _guidGenerator = guidGenerator;
            _webHostEnvironment = webHostEnvironment;
            _HtmlSanitizer = htmlSanitizer;


            ConfigurePolicies();
            ConfigureHtmlSanitizer();
            _blogRepository = blogRepository;
        }

        private void ConfigurePolicies()
        {
            GetPolicyName = null;
            CreatePolicyName = ProductSellingPermissions.Blogs.Create;
            UpdatePolicyName = ProductSellingPermissions.Blogs.Edit;
            DeletePolicyName = ProductSellingPermissions.Blogs.Delete;
            GetListPolicyName = ProductSellingPermissions.Blogs.Default;
        }
        [AllowAnonymous]
        public override Task<PagedResultDto<BlogDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return base.GetListAsync(input);
        }

        [AllowAnonymous]
        public virtual async Task<BlogDto> GetBlogBySlug(string slug)
        {
            var blog = await _blogRepository.FirstOrDefaultAsync(b => b.UrlSlug == slug);
            if (blog == null)
            {
                throw new Exception("Blog not found");
            }
            return ObjectMapper.Map<Blog, BlogDto>(blog);
        }
        /*
                [Authorize(Roles = IdentityRoleConsts.Blogger, Policy = ProductSellingPermissions.Blogs.Create)]
                [Authorize(Roles = IdentityRoleConsts.Admin, Policy = ProductSellingPermissions.Blogs.Create)]*/
        public override async Task<BlogDto> CreateAsync(CreateAndUpdateBlogDto input)
        {

            var sanitizedContent = _HtmlSanitizer.Sanitize(input.Content);
            if (string.IsNullOrWhiteSpace(input.Title))
            {
                input.Title = ExtractTitleFromHtml(sanitizedContent);
                Logger.LogInformation($"Auto-extracted title: {input.Title}");
            }

            // Validate that we have a title (either provided or extracted)
            if (string.IsNullOrWhiteSpace(input.Title))
            {
                throw new UserFriendlyException("Blog title is required. Please provide a title or add a heading to your content.");
            }

            var authorId = CurrentUser.Id ?? throw new UserFriendlyException("User must be logged in");
            var authorName = !string.IsNullOrWhiteSpace(input.Author)
                ? input.Author
                : CurrentUser.Name ?? CurrentUser.UserName ?? "Unknown";

            var blog = new Blog(
                _guidGenerator.Create(),
                input.Title,
                sanitizedContent,
                //input.Content,
                input.PublishedDate,
                input.Author, authorId,

                input.UrlSlug,
                input.MainImageUrl,
                input.MainImageId
                );

            var newBlog = await _blogRepository.InsertAsync(blog, autoSave: true);
            var result = ObjectMapper.Map<Blog, BlogDto>(newBlog);
            return result;
        }

        public override async Task DeleteAsync(Guid id)
        {
            var blog = await _blogRepository.GetAsync(id);

            // Delete associated image file if exists
            if (!string.IsNullOrEmpty(blog.MainImageUrl))
            {
                DeleteImageFile(blog.MainImageUrl);
            }

            await Repository.DeleteAsync(id, autoSave: true);
        }

        public override async Task<BlogDto> UpdateAsync(Guid id, CreateAndUpdateBlogDto input)
        {
            var blog = await _blogRepository.GetAsync(id);
            blog.Title = input.Title;
            blog.Content = _HtmlSanitizer.Sanitize(input.Content);
            blog.PublishedDate = input.PublishedDate;
            blog.UrlSlug = input.UrlSlug;
            //input.ImageUrl = input.ImageUrl;
            blog.Content = _HtmlSanitizer.Sanitize(input.Content);

            var updatedBlog = await Repository.UpdateAsync(blog, autoSave: true);

            var result = ObjectMapper.Map<Blog, BlogDto>(updatedBlog);
            return result;
        }
        public override async Task<BlogDto> GetAsync(Guid id)
        {
            var query = await Repository.GetQueryableAsync();
            var blog = await query.FirstOrDefaultAsync(x => x.Id == id);


            return blog == null ? throw new EntityNotFoundException(typeof(Blog), id) : ObjectMapper.Map<Blog, BlogDto>(blog);
        }

        private void ConfigureHtmlSanitizer()
        {
            // Clear existing configuration
            _HtmlSanitizer.AllowedTags.Clear();
            _HtmlSanitizer.AllowedAttributes.Clear();

            // Add allowed tags
            var allowedTags = new[]
            {
            "img", "figure", "figcaption", "p", "h1", "h2", "h3", "h4", "h5", "h6",
            "strong", "em", "u", "ul", "ol", "li", "blockquote", "a", "br", "div",
            "span", "table", "tbody", "tr", "td", "th", "thead"
        };

            foreach (var tag in allowedTags)
            {
                _HtmlSanitizer.AllowedTags.Add(tag);
            }

            // Add allowed attributes
            var allowedAttributes = new[]
            {
            "src", "alt", "title", "width", "height", "style", "class", "href",
            "target", "data-*", "id"
        };

            foreach (var attr in allowedAttributes)
            {
                _HtmlSanitizer.AllowedAttributes.Add(attr);
            }

            // Allow data attributes pattern
            _HtmlSanitizer.AllowDataAttributes = true;
        }
        public string ExtractTitleFromHtml(string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
                return string.Empty;

            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                // Priority order for heading extraction
                var headingTags = new[] { "h1", "h2", "h3", "h4", "h5", "h6" };

                foreach (var tag in headingTags)
                {
                    var heading = doc.DocumentNode.SelectSingleNode($"//{tag}");
                    if (heading != null && !string.IsNullOrWhiteSpace(heading.InnerText))
                    {
                        return CleanText(heading.InnerText);
                    }
                }

                // Fallback: first paragraph
                var firstParagraph = doc.DocumentNode.SelectSingleNode("//p");
                if (firstParagraph != null && !string.IsNullOrWhiteSpace(firstParagraph.InnerText))
                {
                    var text = CleanText(firstParagraph.InnerText);
                    return text.Length > 60 ? text.Substring(0, 60) + "..." : text;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GenerateUrlSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            return title
                .ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("--", "-")
                .Trim('-')
                .Substring(0, Math.Min(50, title.Length)); // Limit length
        }

        private string CleanText(string text)
        {
            return System.Net.WebUtility.HtmlDecode(text?.Trim() ?? string.Empty);
        }
        private void DeleteImageFile(string imageUrl)
        {
            try
            {
                var relativePath = imageUrl.TrimStart('/');
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Logger.LogInformation("Deleted blog image: {FilePath}", filePath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to delete blog image: {ImageUrl}", imageUrl);
            }
        }

        public async Task<PagedResultDto<BlogDto>> GetBlogByBloggerAsync(PagedAndSortedResultRequestDto input)
        {
            var query = await Repository.GetQueryableAsync();
            var authorId = CurrentUser.Id ?? throw new AbpAuthorizationException("User must be logged in");
            var queryable = query.Where(b => b.AuthorId == authorId);

            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var sortedQuery = queryable.OrderBy(input.Sorting.IsNullOrWhiteSpace() ? nameof(Blog.CreationTime) + " DESC" : input.Sorting).PageBy(input);

            var blogs = await AsyncExecuter.ToListAsync(sortedQuery.AsNoTracking());
            return new PagedResultDto<BlogDto>(
                totalCount,
                 ObjectMapper.Map<List<Blog>, List<BlogDto>>(blogs)
            );
        }
    }
}
