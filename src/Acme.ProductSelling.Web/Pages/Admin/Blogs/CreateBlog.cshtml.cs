using Acme.ProductSelling.Blogs;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Users;
using YamlDotNet.Core.Tokens;
namespace Acme.ProductSelling.Web.Pages.Admin.Blogs
{
    public class CreateBlogModel : ProductSellingPageModel
    {
        private readonly IBlogAppService _blogAppService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICurrentUser _currentUser;
        private const string UploadsFolder = "blog-cover-images";
        [BindProperty]
        public CreateAndUpdateBlogDto Blog { get; set; }
        [BindProperty]
        public IFormFile? CoverImageFile { get; set; }
        public CreateBlogModel(IBlogAppService blogAppService, IWebHostEnvironment webHostEnvironment, CurrentUser currentUser)
        {
            _webHostEnvironment = webHostEnvironment;
            _blogAppService = blogAppService;
            _currentUser = currentUser;
        }
        public void OnGet()
        {
            Blog = new CreateAndUpdateBlogDto();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (CoverImageFile == null || string.IsNullOrEmpty(CoverImageFile.FileName.ToString()))
            {
                ModelState.AddModelError("CoverImageFile", "Cover image is required.");
                return Page();
            }


            if (!ModelState.IsValid)
            {
                Logger.LogWarning("ModelState is invalid");

                foreach (var error in ModelState)
                {
                    Logger.LogWarning($"ModelState Error - Key: {error.Key}, Errors: " +
                        $"{string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return Page();
            }

            try
            {
                var uploadsRootFolder = Path.Combine(_webHostEnvironment.WebRootPath, UploadsFolder);

                if (!Directory.Exists(uploadsRootFolder))
                {
                    Directory.CreateDirectory(uploadsRootFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(CoverImageFile.FileName);
                var filePath = Path.Combine(uploadsRootFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await CoverImageFile.CopyToAsync(stream);
                }
                Blog.MainImageUrl = $"/{UploadsFolder}/" + uniqueFileName;
                Console.WriteLine($"Image uploaded to: {Blog.MainImageUrl}");
                Blog.MainImageId = Guid.NewGuid(); // Assign a new GUID for the image

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while processing the form.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return Page();
            }

            try
            {
                Blog.Author = _currentUser.Name;
                Console.WriteLine($"Current User: {_currentUser.Name}, Author set to: {Blog.Author}");
                Blog.PublishedDate = DateTime.Now;
                await _blogAppService.CreateAsync(Blog);
                return RedirectToPage("/Admin/Blogs/Index");
            }
            catch (UserFriendlyException ex)
            {
                Logger.LogWarning(ex, "User-friendly error creating blog.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while creating the blog post.");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the blog post. Please try again.");
                return Page();
            }

        }
    }
}
