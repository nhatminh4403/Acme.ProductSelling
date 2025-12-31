using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Acme.ProductSelling.Web.Middleware
{
    public class RequestCultureMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _supportedCultures;
        private readonly string _defaultCulture;

        // 1. OPTIMIZATION: Move static arrays to class level so they aren't allocated on every request
        private static readonly string[] _staticFileExtensions = new[] {
            ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".ico",
            ".svg", ".woff", ".woff2", ".ttf", ".map", ".json"
        };

        private static readonly string[] _pathPrefixesToIgnore = new[] {
            "/api", "/_framework", "/css", "/js", "/lib", "/images",
            "/swagger", "/signalr", "/favicon.ico", "/_vs/","/health-status",
            "/signalr-hubs","/admin", "/identity", "/Account", "/TenantManagement",
            "/SettingManagement","/abp","/abp-web-resources", "/swagger-ui", "/swagger/v1/swagger.json","/thanh-toan",
            "/Abp","/libs","/.well-known", "/connect","/hangfire","/blogger","/manager","/seller","/cashier","/warehouse",
        };

        public RequestCultureMiddleware(RequestDelegate next, IOptions<RequestLocalizationOptions> options)
        {
            _next = next;
            var localizationOptions = options.Value;
            _supportedCultures = localizationOptions.SupportedCultures?.Select(c => c.TwoLetterISOLanguageName).ToArray() ?? new[] { "vi", "en" };
            _defaultCulture = localizationOptions.DefaultRequestCulture.Culture.TwoLetterISOLanguageName;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (IsStaticResource(path))
            {
                await _next(context);
                return;
            }

            var urlCulture = GetCultureFromUrl(path);
            var cookieCulture = GetCultureFromCookies(context);
            var targetCulture = cookieCulture ?? urlCulture ?? _defaultCulture;

            if (urlCulture != targetCulture)
            {
                var newPath = UpdatePathWithCulture(path, targetCulture, urlCulture);
                var redirectUrl = $"{newPath}{context.Request.QueryString}";
                context.Response.Redirect(redirectUrl, permanent: false);
                return;
            }

            SyncCultureCookies(context, targetCulture);

            await _next(context);
        }

        private bool IsStaticResource(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            if (path.Contains("Logout", StringComparison.OrdinalIgnoreCase) || path.Contains("/Account/Logout", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            // Check Extensions
            if (_staticFileExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            // Check Prefixes
            bool isIgnoredPrefix = _pathPrefixesToIgnore.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

            // 2. FIX: Calculate result first, then log using the variable. No Recursion.
            if (path.Contains("Logout", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[DEBUG] Logout path detected: {path}, IsStatic: {isIgnoredPrefix}");
            }

            return isIgnoredPrefix;
        }

        private string GetCultureFromUrl(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var firstSegment = segments.FirstOrDefault();

            return firstSegment != null && _supportedCultures.Contains(firstSegment, StringComparer.OrdinalIgnoreCase)
                ? firstSegment
                : null;
        }

        private string GetCultureFromCookies(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue(".AspNetCore.Culture", out var aspNetCoreCultureValue))
            {
                var decodedValue = HttpUtility.UrlDecode(aspNetCoreCultureValue);
                var culturePart = decodedValue.Split('|')
                    .FirstOrDefault(p => p.StartsWith("c="))?
                    .Substring(2);

                if (culturePart != null && _supportedCultures.Contains(culturePart))
                {
                    return culturePart;
                }
            }

            string[] cookieNames = { "culture", "Abp.Localization.CultureName" };
            foreach (var name in cookieNames)
            {
                if (context.Request.Cookies.TryGetValue(name, out var culture) && _supportedCultures.Contains(culture))
                {
                    return culture;
                }
            }

            return null;
        }

        private string UpdatePathWithCulture(string originalPath, string newCulture, string currentUrlCulture)
        {
            if (string.IsNullOrEmpty(originalPath) || originalPath == "/")
            {
                return $"/{newCulture}";
            }

            var segments = originalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (currentUrlCulture != null && segments.Length > 0 && segments[0].Equals(currentUrlCulture, StringComparison.OrdinalIgnoreCase))
            {
                segments[0] = newCulture;
                return "/" + string.Join("/", segments);
            }

            return $"/{newCulture}{originalPath}";
        }

        private void SyncCultureCookies(HttpContext context, string culture)
        {
            if (GetCultureFromCookies(context) != culture)
            {
                var feature = context.Features.Get<IRequestCultureFeature>();
                var actualCulture = feature?.RequestCulture ?? new RequestCulture(culture);

                context.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(actualCulture),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    Path = "/",
                    HttpOnly = false,
                    SameSite = SameSiteMode.Lax
                };
                context.Response.Cookies.Append("culture", culture, cookieOptions);
                context.Response.Cookies.Append("Abp.Localization.CultureName", culture, cookieOptions);
            }
        }
    }
}