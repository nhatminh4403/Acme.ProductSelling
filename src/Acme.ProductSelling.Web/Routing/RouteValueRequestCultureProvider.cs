using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Routing
{
    public class RouteValueRequestCultureProvider : RequestCultureProvider
    {
        private readonly string[] _supportedCultures = { "en", "vi" }; public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var path = httpContext.Request.Path.Value;
            if (string.IsNullOrEmpty(path) || path == "/")
                return Task.FromResult<ProviderCultureResult?>(null);

            // Lấy segments
            var segments = path.Split('/', System.StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
                return Task.FromResult<ProviderCultureResult?>(null);

            // Lấy segment đầu tiên
            var firstSegment = segments[0];
            if (string.IsNullOrEmpty(firstSegment) || !IsSupportedCulture(firstSegment))
                return Task.FromResult<ProviderCultureResult?>(null);

            // Kiểm tra nếu có duplicate culture (ví dụ: /vi/en/page)
            if (segments.Length >= 2 && IsSupportedCulture(segments[1]))
            {
                // Có duplicate, chỉ lấy culture đầu tiên
                var culture = firstSegment;
                SetCultureCookie(httpContext, culture);
                return Task.FromResult(new ProviderCultureResult(culture, culture))!;
            }

            // Normal case: chỉ có 1 culture
            SetCultureCookie(httpContext, firstSegment);
            return Task.FromResult(new ProviderCultureResult(firstSegment, firstSegment))!;
        }

        private void SetCultureCookie(HttpContext httpContext, string culture)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = System.DateTimeOffset.UtcNow.AddDays(30),
                Path = "/",
                HttpOnly = false, // Cho phép JavaScript access
                SameSite = SameSiteMode.Lax
            };

            httpContext.Response.Cookies.Append("culture", culture, cookieOptions);
        }

        private bool IsSupportedCulture(string culture)
        {
            foreach (var supportedCulture in _supportedCultures)
            {
                if (string.Equals(culture, supportedCulture, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}

