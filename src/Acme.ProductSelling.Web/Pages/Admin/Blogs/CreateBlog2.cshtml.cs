using Acme.ProductSelling.Blogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Admin.Blogs
{
    public class CreateBlog2Model : ProductSellingPageModel
    {
        private readonly IBlog _blogAppService;
        [BindProperty]
        public CreateBlogVM Blog { get; set; }
        public CreateBlog2Model(IBlog blogAppService)
        {
            _blogAppService = blogAppService;
        }

        public void OnGet()
        {
            Blog = new CreateBlogVM
            {
                PublishedDate = DateTime.Now,
                Author = CurrentUser.Name ?? CurrentUser.UserName ?? ""
            };
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var createDto = new CreateAndUpdateBlogDto
            {
                Title = Blog.Title,
                Content = Blog.Content,
                PublishedDate = Blog.PublishedDate,
                Author = Blog.Author,
                UrlSlug = Blog.UrlSlug,
                MainImageUrl = Blog.MainImageUrl,
                MainImageId = Blog.MainImageId
            };  
            var result = _blogAppService.CreateAsync(createDto);
            Alerts.Success("Blog post created successfully!");
            return RedirectToPage("./Admin/Blogs/Index");

        }
    }
    public class CreateBlogVM
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string UrlSlug { get; set; }
        public string? MainImageUrl { get; set; }
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public string Author { get; set; }

        public Guid? MainImageId { get; set; }
    }

}
