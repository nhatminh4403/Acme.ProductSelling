using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Acme.ProductSelling.Web.Routing
{
    public class AspNetCoreCookieRequestCultureProvider : RequestCultureProvider
    {
        private readonly string[] _supportedCultures = { "en", "vi" };

        public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext.Request.Cookies.TryGetValue(".AspNetCore.Culture", out var cookieValue))
            {
                try
                {
                    // Parse AspNetCore culture cookie format: c=en|uic=en
                    var decodedValue = HttpUtility.UrlDecode(cookieValue);
                    Console.WriteLine($"[AspNetCoreCookieProvider] Cookie Value: {decodedValue}");

                    var parts = decodedValue.Split('|');
                    string culture = null;
                    string uiCulture = null;

                    foreach (var part in parts)
                    {
                        if (part.StartsWith("c="))
                        {
                            culture = part.Substring(2).ToLower();
                        }
                        else if (part.StartsWith("uic="))
                        {
                            uiCulture = part.Substring(4).ToLower();
                        }
                    }

                    if (!string.IsNullOrEmpty(culture) && IsSupportedCulture(culture))
                    {
                        var finalUiCulture = !string.IsNullOrEmpty(uiCulture) && IsSupportedCulture(uiCulture)
                            ? uiCulture
                            : culture;

                        Console.WriteLine($"[AspNetCoreCookieProvider] Determined culture: {culture}, UI culture: {finalUiCulture}");

                        // Sync với các cookies khác
                        SyncOtherCookies(httpContext, culture);

                        return Task.FromResult(new ProviderCultureResult(culture, finalUiCulture))!;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AspNetCoreCookieProvider] Error parsing AspNetCore culture cookie: {ex.Message}");
                }
            }

            return Task.FromResult<ProviderCultureResult?>(null);
        }

        private void SyncOtherCookies(HttpContext httpContext, string culture)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                Path = "/",
                HttpOnly = false,
                SameSite = SameSiteMode.Lax
            };

            // Sync custom cookies nếu chúng khác với culture hiện tại
            var currentCustomCookie = httpContext.Request.Cookies["culture"];
            if (currentCustomCookie != culture)
            {
                httpContext.Response.Cookies.Append("culture", culture, cookieOptions);
                httpContext.Response.Cookies.Append("Abp.Localization.CultureName", culture, cookieOptions);
                Console.WriteLine($"[AspNetCoreCookieProvider] Synced custom cookies to: {culture}");
            }
        }

        private bool IsSupportedCulture(string culture)
        {
            foreach (var supportedCulture in _supportedCultures)
            {
                if (string.Equals(culture, supportedCulture, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
