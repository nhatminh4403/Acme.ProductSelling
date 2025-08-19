using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Middleware
{
    public class CultureRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _supportedCultures;
        private readonly string _defaultCulture;
        public CultureRedirectMiddleware(RequestDelegate next, IOptions<RequestLocalizationOptions> options)
        {
            _next = next;
            _supportedCultures = options.Value.SupportedCultures?.Select(c => c.TwoLetterISOLanguageName).ToArray() ?? new[] { "vi" };
            _defaultCulture = options.Value.DefaultRequestCulture.Culture.TwoLetterISOLanguageName;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            // Bỏ qua các static files, API, v.v.
            if (path.StartsWith("/api") || path.StartsWith("/_framework") || path.StartsWith("/css") || path.StartsWith("/js"))
            {
                await _next(context);
                return;
            }

            var parts = path.Split('/', System.StringSplitOptions.RemoveEmptyEntries);
            var firstSegment = parts.FirstOrDefault();

            // Nếu thiếu culture prefix thì redirect sang default culture
            if (firstSegment == null || !_supportedCultures.Contains(firstSegment))
            {
                var culture = _defaultCulture;
                var newPath = $"/{culture}{path}{context.Request.QueryString}";
                context.Response.Redirect(newPath, permanent: false);
                return;
            }

            await _next(context);
        }

    }
}
