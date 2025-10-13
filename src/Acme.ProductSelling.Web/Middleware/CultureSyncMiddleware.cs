using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Acme.ProductSelling.Web.Middleware
{
    public class CultureSyncMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _supportedCultures;
        private readonly string _defaultCulture;

        public CultureSyncMiddleware(RequestDelegate next, IOptions<RequestLocalizationOptions> options)
        {
            _next = next;
            _supportedCultures = options.Value.SupportedCultures?.Select(c => c.TwoLetterISOLanguageName).ToArray() ?? new[] { "vi", "en" };
            _defaultCulture = options.Value.DefaultRequestCulture.Culture.TwoLetterISOLanguageName;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            Console.WriteLine($"[CultureSyncMiddleware] Processing path: {path}");

            if (IsStaticResource(path))
            {
                await _next(context);
                return;
            }

            // Lấy culture từ các nguồn khác nhau
            var urlCulture = GetCultureFromUrl(path);
            var cookieCulture = GetCultureFromCookie(context);
            var aspNetCoreCulture = GetCultureFromAspNetCoreCookie(context);

            Console.WriteLine($"[CultureSyncMiddleware] URL Culture: {urlCulture}");
            Console.WriteLine($"[CultureSyncMiddleware] Cookie Culture: {cookieCulture}");
            Console.WriteLine($"[CultureSyncMiddleware] AspNetCore Cookie Culture: {aspNetCoreCulture}");

            // Ưu tiên culture theo thứ tự: AspNetCore Cookie > Custom Cookie > URL > Default
            var targetCulture = aspNetCoreCulture ?? cookieCulture ?? urlCulture ?? _defaultCulture;

            Console.WriteLine($"[CultureSyncMiddleware] Target Culture: {targetCulture}");

            // Nếu URL culture khác với target culture, redirect
            if (urlCulture != targetCulture)
            {
                var newPath = UpdatePathWithCulture(path, targetCulture, urlCulture);
                if (newPath != path)
                {
                    var redirectUrl = $"{newPath}{context.Request.QueryString}";
                    Console.WriteLine($"[CultureSyncMiddleware] Redirecting from {path} to {redirectUrl}");

                    context.Response.Redirect(redirectUrl, permanent: false);
                    return;
                }
            }

            // Đồng bộ cookies nếu cần
            SyncCultureCookies(context, targetCulture);

            await _next(context);
        }

        private string GetCultureFromUrl(string path)
        {
            if (string.IsNullOrEmpty(path) || path == "/")
                return null;

            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
                return null;

            var firstSegment = segments[0].ToLower();
            return _supportedCultures.Contains(firstSegment) ? firstSegment : null;
        }

        private string GetCultureFromCookie(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("culture", out var culture) &&
                _supportedCultures.Contains(culture))
            {
                return culture;
            }

            if (context.Request.Cookies.TryGetValue("Abp.Localization.CultureName", out var abpCulture) &&
                _supportedCultures.Contains(abpCulture))
            {
                return abpCulture;
            }

            return null;
        }

        private string GetCultureFromAspNetCoreCookie(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue(".AspNetCore.Culture", out var cookieValue))
            {
                try
                {
                    // Parse AspNetCore culture cookie format: c=en|uic=en
                    var decodedValue = HttpUtility.UrlDecode(cookieValue);
                    Console.WriteLine($"[CultureSyncMiddleware] AspNetCore Cookie Value: {decodedValue}");

                    var parts = decodedValue.Split('|');
                    foreach (var part in parts)
                    {
                        if (part.StartsWith("c="))
                        {
                            var culture = part.Substring(2).ToLower();
                            if (_supportedCultures.Contains(culture))
                            {
                                return culture;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CultureSyncMiddleware] Error parsing AspNetCore culture cookie: {ex.Message}");
                }
            }

            return null;
        }

        private string UpdatePathWithCulture(string originalPath, string newCulture, string currentCulture)
        {
            if (string.IsNullOrEmpty(originalPath) || originalPath == "/")
            {
                return $"/{newCulture}";
            }

            var segments = originalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
            {
                return $"/{newCulture}";
            }

            // Nếu segment đầu tiên là culture hiện tại, thay thế
            if (currentCulture != null && segments[0].Equals(currentCulture, StringComparison.OrdinalIgnoreCase))
            {
                segments[0] = newCulture;
                return "/" + string.Join("/", segments);
            }

            // Nếu không có culture prefix, thêm vào
            if (!_supportedCultures.Contains(segments[0].ToLower()))
            {
                return $"/{newCulture}{originalPath}";
            }

            // Nếu có culture prefix khác, thay thế
            segments[0] = newCulture;
            return "/" + string.Join("/", segments);
        }

        private void SyncCultureCookies(HttpContext context, string culture)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                Path = "/",
                HttpOnly = false,
                SameSite = SameSiteMode.Lax
            };

            // Sync custom culture cookies
            var currentCustomCookie = context.Request.Cookies["culture"];
            if (currentCustomCookie != culture)
            {
                context.Response.Cookies.Append("culture", culture, cookieOptions);
                context.Response.Cookies.Append("Abp.Localization.CultureName", culture, cookieOptions);
                Console.WriteLine($"[CultureSyncMiddleware] Synced custom culture cookies to: {culture}");
            }
        }

        private bool IsStaticResource(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            var prefixList = new[]
            {
                "/api", "/_framework", "/css", "/js", "/lib", "/images",
                "/swagger", "/signalr", "/favicon.ico", "/_vs/", "/health-status",
                "/signalr-hubs", "/admin", "/identity", "/account", "/tenantmanagement",
                "/SettingManagement", "/abp", "/abp-web-resources", "/swagger-ui",
                "/swagger/v1/swagger.json", "/Abp", "/libs", "/.well-known", "/connect",
            };

            foreach (var prefix in prefixList)
            {
                if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
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
