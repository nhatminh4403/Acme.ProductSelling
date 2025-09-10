using Acme.ProductSelling.Folder;
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
        private const string ContentImageUpload = FolderConsts.ImageFolder + "uploads";
        public FileAppService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<FileUploadResultDto> UploadImageAsync(IFormFile file, bool replaceIfExists = true)
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

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, ContentImageUpload);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Use original filename (sanitized)
            var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
            var sanitizedFileName = SanitizeFileName(originalFileName);
            var fileName = $"{sanitizedFileName}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            Guid fileId;

            // Check if file already exists
            if (File.Exists(filePath))
            {
                if (replaceIfExists)
                {
                    // Replace the existing file
                    fileId = Guid.NewGuid(); // Generate new ID for the replacement

                    // Delete the existing file first
                    File.Delete(filePath);
                }
                else
                {
                    // Create a unique filename by appending a number
                    var counter = 1;
                    string uniqueFileName;
                    string uniqueFilePath;

                    do
                    {
                        uniqueFileName = $"{sanitizedFileName}({counter}){fileExtension}";
                        uniqueFilePath = Path.Combine(uploadsFolder, uniqueFileName);
                        counter++;
                    } while (File.Exists(uniqueFilePath));

                    fileName = uniqueFileName;
                    filePath = uniqueFilePath;
                    fileId = Guid.NewGuid();
                }
            }
            else
            {
                // File doesn't exist, create new
                fileId = Guid.NewGuid();
            }

            // Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var fileUrl = $"/images/uploads/{fileName}";

            return new FileUploadResultDto
            {
                Url = fileUrl,
                FileName = fileName,
                Size = file.Length,
                FileId = fileId
            };
        }
        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "unnamed";

            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new StringBuilder();

            foreach (char c in fileName)
            {
                if (!invalidChars.Contains(c) && c != ' ')
                    sanitized.Append(c);
                else if (c == ' ')
                    sanitized.Append('_');
                else
                    sanitized.Append('-');
            }

            var result = sanitized.ToString().Trim('-', '_');
            return string.IsNullOrEmpty(result) ? "unnamed" : result;
        }
    }
}
