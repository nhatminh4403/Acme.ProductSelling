using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Routing.Providers
{
    public class RouteCultureProvider : IRequestCultureProvider
    {
        private readonly string[] _supportedCultures = { "en", "vi" };

        public Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var path = httpContext.Request.Path.Value;

            if (string.IsNullOrEmpty(path) || path == "/")
            {
                // Trả về culture mặc định
                return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult("en"));
            }

            var segments = path.Split('/', System.StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length > 0)
            {
                var potentialCulture = segments[0].ToLowerInvariant();

                if (System.Array.Exists(_supportedCultures, c => c == potentialCulture))
                {
                    return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(potentialCulture));
                }
            }

            // Fallback về culture mặc định
            return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult("en"));
        }
    }
}
