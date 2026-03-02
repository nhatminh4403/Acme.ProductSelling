using Ganss.Xss;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Products.Helpers
{
    public interface IProductDescriptionHelper : ITransientDependency
    {
        string SanitizeDescription(string html);
        void DeleteImageFile(string imageUrl);
    }

    public class ProductDescriptionHelper : IProductDescriptionHelper
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly ILogger<ProductDescriptionHelper> _logger;

        public ProductDescriptionHelper(
            IWebHostEnvironment webHostEnvironment,
            IHtmlSanitizer htmlSanitizer,
            ILogger<ProductDescriptionHelper> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _htmlSanitizer = htmlSanitizer;
            _logger = logger;

            // Configure the sanitizer once during initialization
            ConfigureHtmlSanitizer();
        }

        public string SanitizeDescription(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            return _htmlSanitizer.Sanitize(html);
        }

        public void DeleteImageFile(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;

            try
            {
                // Ensures leading slashes are handled and path is localized to the OS
                var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("Deleted product image: {FilePath}", filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete product image: {ImageUrl}", imageUrl);
            }
        }

        private void ConfigureHtmlSanitizer()
        {
            _htmlSanitizer.AllowedTags.Clear();
            _htmlSanitizer.AllowedAttributes.Clear();

            var allowedTags = new[] {
                "img", "figure", "figcaption", "p", "h1", "h2", "h3", "h4", "h5", "h6",
                "strong", "em", "u", "ul", "ol", "li", "blockquote", "a", "br", "div",
                "span", "table", "tbody", "tr", "td", "th", "thead"
            };
            foreach (var tag in allowedTags) _htmlSanitizer.AllowedTags.Add(tag);

            var allowedAttributes = new[] {
                "src", "alt", "title", "width", "height", "style", "class", "href",
                "target", "id"
            };
            foreach (var attr in allowedAttributes) _htmlSanitizer.AllowedAttributes.Add(attr);

            _htmlSanitizer.AllowDataAttributes = true;
        }
    }
}