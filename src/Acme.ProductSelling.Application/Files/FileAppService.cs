using AngleSharp.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Files
{
    public class FileAppService : ApplicationService, IFileAppService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileAppService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<FileUploadResultDto> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new UserFriendlyException("No file uploaded");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new UserFriendlyException("Invalid file type. Only images are allowed.");

            var maxSizeBytes = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxSizeBytes)
                throw new UserFriendlyException("File size cannot exceed 5MB");

            var fileId = Guid.NewGuid();
            var fileName = $"{fileId}{fileExtension}";
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "images");

            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var fileUrl = $"/uploads/images/{fileName}";

            return new FileUploadResultDto
            {
                Url = fileUrl,
                FileName = fileName,
                Size = file.Length,
                FileId = fileId
            };
        }
    }
}
