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
namespace Acme.ProductSelling.Web.Pages.Admin.Blogs
{
    public class CreateBlogModel : ProductSellingPageModel
    {
        private readonly IBlogAppService _blogAppService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string UploadsFolder = "blog-cover-images";
        [BindProperty]
        public CreateAndUpdateBlogDto BlogPost { get; set; }
        [BindProperty]
        public IFormFile CoverImageFile { get; set; }
        public CreateBlogModel(IBlogAppService blogAppService, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _blogAppService = blogAppService;
        }
        public void OnGet()
        {
            BlogPost = new CreateAndUpdateBlogDto();
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

                if(!Directory.Exists(uploadsRootFolder))
                {
                    Directory.CreateDirectory(uploadsRootFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + $"{Path.GetExtension(CoverImageFile.FileName)}";
                var filePath = Path.Combine(uploadsRootFolder, CoverImageFile.FileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await CoverImageFile.CopyToAsync(stream);
                }
                BlogPost.ImageUrl = $"/{UploadsFolder}/{uniqueFileName}";

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while processing the form.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return Page();
            }

            try
            {
                await _blogAppService.CreateAsync(BlogPost);
                return RedirectToPage("/Admin/Blogs/Index");
            }
            catch (UserFriendlyException ex)
            {
                Logger.LogWarning(ex, "User-friendly error creating product.");
                // Add error to a specific field if possible, or general model error
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
