using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Middleware
{
    public class CultureRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _defaultCulture = "vi";

        public CultureRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // Nếu URL không có culture và không phải static files
            if (!string.IsNullOrEmpty(path) &&
                path != "/" &&
                !path.StartsWith("/css") &&
                !path.StartsWith("/js") &&
                !path.StartsWith("/images") &&
                !HasCultureInPath(path))
            {
                var acceptLanguage = context.Request.Headers["Accept-Language"].ToString();
                var preferredCulture = GetPreferredCulture(acceptLanguage);

                if (preferredCulture != _defaultCulture)
                {
                    var newPath = $"/{preferredCulture}{path}";
                    context.Response.Redirect(newPath + context.Request.QueryString);
                    return;
                }
            }

            await _next(context);
        }

        private bool HasCultureInPath(string path)
        {
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0) return false;

            var supportedCultures = new[] { "en", "vi" };
            return supportedCultures.Contains(segments[0]);
        }

        private string GetPreferredCulture(string acceptLanguage)
        {
            if (string.IsNullOrEmpty(acceptLanguage)) return _defaultCulture;

            if (acceptLanguage.Contains("vi")) return "vi";
            return _defaultCulture;
        }
    }
}
