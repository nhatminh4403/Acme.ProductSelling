using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Routing
{
    public class UrlCultureProvider : IRequestCultureProvider
    {
        
        public Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var path = httpContext.Request.Path.Value;

            if (string.IsNullOrEmpty(path) || path == "/")
            {
                return Task.FromResult((ProviderCultureResult)null);
            }

            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
            {
                return Task.FromResult((ProviderCultureResult)null);
            }

            var cultureSegment = segments[0];
            var supportedCultures = new[] { "en", "vi" }; // Danh sách culture được hỗ trợ

            if (supportedCultures.Contains(cultureSegment))
            {
                return Task.FromResult(new ProviderCultureResult(cultureSegment));
            }

            return Task.FromResult((ProviderCultureResult)null);
        }
    }
}
