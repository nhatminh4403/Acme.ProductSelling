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

            // 1. Skip processing for static resources and API calls.
            if (IsStaticResource(path))
            {
                await _next(context);
                return;
            }

            // 2. Determine the culture from different sources.
            var urlCulture = GetCultureFromUrl(path);
            var cookieCulture = GetCultureFromCookies(context);

            // 3. Establish the final "target" culture based on priority: Cookie > URL > Default
            var targetCulture = cookieCulture ?? urlCulture ?? _defaultCulture;

            // 4. Decide if a redirect is necessary.
            // A redirect is needed if the URL is missing a culture, or has the wrong one.
            if (urlCulture != targetCulture)
            {
                var newPath = UpdatePathWithCulture(path, targetCulture, urlCulture);
                var redirectUrl = $"{newPath}{context.Request.QueryString}";

                Console.WriteLine($"[RequestCultureMiddleware] Redirecting from '{path}' to '{redirectUrl}' because URL culture ('{urlCulture}') mismatches target culture ('{targetCulture}').");

                context.Response.Redirect(redirectUrl, permanent: false);
                return;
            }

            // 5. If no redirect is needed, ensure cookies are in sync with the current URL culture.
            SyncCultureCookies(context, targetCulture);

            await _next(context);
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
            // The official ASP.NET Core cookie is the highest priority source.
            if (context.Request.Cookies.TryGetValue(".AspNetCore.Culture", out var aspNetCoreCultureValue))
            {
                var decodedValue = HttpUtility.UrlDecode(aspNetCoreCultureValue);
                // The cookie value is typically "c=vi|uic=vi". We just need the 'c' part.
                var culturePart = decodedValue.Split('|')
                    .FirstOrDefault(p => p.StartsWith("c="))?
                    .Substring(2);

                if (culturePart != null && _supportedCultures.Contains(culturePart))
                {
                    return culturePart;
                }
            }

            // Fallback to custom/ABP cookies
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
            // If path is root, just return the new culture
            if (string.IsNullOrEmpty(originalPath) || originalPath == "/")
            {
                return $"/{newCulture}";
            }

            var segments = originalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            // If the URL already has a culture, replace it
            if (currentUrlCulture != null && segments.Length > 0 && segments[0].Equals(currentUrlCulture, StringComparison.OrdinalIgnoreCase))
            {
                segments[0] = newCulture;
                return "/" + string.Join("/", segments);
            }

            // Otherwise, prepend the new culture
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

        private bool IsStaticResource(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            var staticFileExtensions = new[] {
                ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".ico",
                ".svg", ".woff", ".woff2", ".ttf", ".map", ".json"
            };

            if (staticFileExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            var pathPrefixesToIgnore = new[] {
            "/api", "/_framework", "/css", "/js", "/lib", "/images",
            "/swagger", "/signalr", "/favicon.ico", "/_vs/","/health-status",
            "/signalr-hubs","/admin", "/identity", "/account/manage","/account", "/TenantManagement",
            "/SettingManagement","/abp","/abp-web-resources", "/swagger-ui", "/swagger/v1/swagger.json",
            "/Abp","/libs","/.well-known", "/connect","/hangfire","/blogger","/manager","/seller","/cashier","/warehouse",

            };

            return pathPrefixesToIgnore.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }
    }
}
