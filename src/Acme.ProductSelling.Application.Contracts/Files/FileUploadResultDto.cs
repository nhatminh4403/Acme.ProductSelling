using System;
namespace Acme.ProductSelling.Files
{
    public class FileUploadResultDto
    {
        public string Url { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public Guid FileId { get; set; } // Add FileId to track uploaded files
    }
}
