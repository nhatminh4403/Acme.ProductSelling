using Acme.ProductSelling.Blogs;
using Acme.ProductSelling.Folder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Users;
namespace Acme.ProductSelling.Web.Pages.Admin.Blogs
{
    public class CreateBlogModel : AdminPageModelBase
    {
        private readonly IBlogAppService _blogAppService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICurrentUser _currentUser;
        private const string UploadsFolder = FolderConsts.ImageFolder + "blog-cover-images";
        [BindProperty]
        public CreateAndUpdateBlogDto Blog { get; set; }
        [BindProperty]
        public IFormFile? CoverImageFile { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }
        public CreateBlogModel(IBlogAppService blogAppService, IWebHostEnvironment webHostEnvironment, ICurrentUser currentUser)
        {
            _webHostEnvironment = webHostEnvironment;
            _blogAppService = blogAppService;
            _currentUser = currentUser;
        }
        public void OnGet()
        {
            if (Prefix != RoleBasedPrefix)
            {
                Response.Redirect($"/{RoleBasedPrefix}/blogs/create");
            }

            Blog = new CreateAndUpdateBlogDto
            {
                PublishedDate = DateTime.Now,

            };
        }

        public async Task<IActionResult> OnPostAsync()
        {




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
            if (CoverImageFile != null && CoverImageFile.Length > 0)
            {
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
            }


            try
            {

                await _blogAppService.CreateAsync(Blog);
                return Redirect("/admin/blogs/");
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
