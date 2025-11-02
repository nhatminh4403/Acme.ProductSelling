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
            _supportedCultures = options.Value.SupportedCultures?.Select(c => c.TwoLetterISOLanguageName).ToArray() ?? new[] { "vi", "en" };
            _defaultCulture = options.Value.DefaultRequestCulture.Culture.TwoLetterISOLanguageName;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            Console.WriteLine($"[CultureRedirectMiddleware] Processing path: {path}");

            // Bỏ qua các static files, API, v.v.
            if (IsStaticResource(path))
            {
                Console.WriteLine($"[CultureRedirectMiddleware] Static resource detected, skipping culture redirect path {path}.");
                await _next(context);
                return;
            }

            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            // Kiểm tra duplicate culture (ví dụ: /vi/en/page)
            if (segments.Length >= 2 &&
                _supportedCultures.Contains(segments[0]) &&
                _supportedCultures.Contains(segments[1]))
            {
                Console.WriteLine($"[CultureRedirectMiddleware] Duplicate culture detected: {segments[0]}, {segments[1]}");
                // Có duplicate culture, chỉ giữ lại culture đầu tiên
                var culture = segments[0];
                var remainingPath = "/" + string.Join("/", segments.Skip(2));
                var newPath = $"/{culture}{remainingPath}{context.Request.QueryString}";

                Console.WriteLine($"[CultureRedirectMiddleware] Redirecting to: {newPath}");
                context.Response.Redirect(newPath, permanent: false);
                return;
            }

            var firstSegment = segments.FirstOrDefault();

            if (firstSegment != null && _supportedCultures.Contains(firstSegment))
            {
                Console.WriteLine($"[CultureRedirectMiddleware] Culture prefix found: {firstSegment}");

                // Đảm bảo culture trong URL được lưu vào HttpContext
                context.Items["RequestedCulture"] = firstSegment;

                // Đồng bộ cookie với URL culture
                SyncCultureCookie(context, firstSegment);

                await _next(context);
                return;
            }

            // Nếu thiếu culture prefix, thêm vào
            var targetCulture = GetCultureFromRequest(context) ?? _defaultCulture;
            var redirectPath = $"/{targetCulture}{path}{context.Request.QueryString}";

            Console.WriteLine($"[CultureRedirectMiddleware] Redirecting to add culture prefix: {redirectPath}");
            context.Response.Redirect(redirectPath, permanent: false);
        }

        private void SyncCultureCookie(HttpContext context, string culture)
        {
            // Chỉ set cookie nếu khác với cookie hiện tại
            var currentCookie = context.Request.Cookies["culture"];
            if (currentCookie != culture)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30),
                    Path = "/",
                    HttpOnly = false,
                    SameSite = SameSiteMode.Lax
                };

                context.Response.Cookies.Append("culture", culture, cookieOptions);
                context.Response.Cookies.Append("Abp.Localization.CultureName", culture, cookieOptions);
                Console.WriteLine($"[CultureRedirectMiddleware] Synced culture cookie to: {culture}");
            }
        }
        private string GetCultureFromRequest(HttpContext context)
        {
            // Ưu tiên culture từ URL nếu có
            if (context.Items.TryGetValue("RequestedCulture", out var requestedCulture) &&
                requestedCulture is string culture &&
                _supportedCultures.Contains(culture))
            {
                Console.WriteLine($"[CultureRedirectMiddleware] Culture from URL: {culture}");
                return culture;
            }

            if (context.Request.Cookies.TryGetValue("culture", out var cookieCulture) &&
                _supportedCultures.Contains(cookieCulture))
            {
                Console.WriteLine($"[CultureRedirectMiddleware] Culture from cookie: {cookieCulture}");
                return cookieCulture;
            }

            if (context.Request.Cookies.TryGetValue("Abp.Localization.CultureName", out var abpCookieCulture) &&
                _supportedCultures.Contains(abpCookieCulture))
            {
                Console.WriteLine($"[CultureRedirectMiddleware] Culture from ABP cookie: {abpCookieCulture}");
                return abpCookieCulture;
            }

            var acceptLanguage = context.Request.Headers["Accept-Language"].FirstOrDefault();
            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                var preferredCultures = acceptLanguage
                    .Split(',')
                    .Select(x => x.Split(';')[0].Trim())
                    .Where(x => x.Length >= 2)
                    .Select(x => x.Substring(0, 2).ToLower());

                foreach (var preferredCulture in preferredCultures)
                {
                    if (_supportedCultures.Contains(preferredCulture))
                    {
                        Console.WriteLine($"[CultureRedirectMiddleware] Culture from Accept-Language: {preferredCulture}");
                        return preferredCulture;
                    }
                }
            }

            return null;
        }
        private bool IsStaticResource(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            var normalizedPath = path.TrimEnd('/');

            var prefixList = new[]
            {
                "/api", "/_framework", "/css", "/js", "/lib", "/images",
                "/swagger", "/signalr", "/favicon.ico", "/_vs/","/health-status",
                "/signalr-hubs","/admin", "/identity", "/account/manage","/account", "/TenantManagement",
                "/SettingManagement","/abp","/abp-web-resources", "/swagger-ui", "/swagger/v1/swagger.json",
                "/Abp","/libs","/.well-known", "/connect","/hangfire"
            };
            foreach (var prefix in prefixList)
            {
                if (normalizedPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            if (path.Contains("."))
            {
                var staticExtensions = new[]
                {
                    ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".ico",
                    ".svg", ".woff", ".woff2", ".ttf", ".map", ".json"
                };

                if (staticExtensions.Any(e => path.EndsWith(e, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
