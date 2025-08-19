using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Routing
{
    public class RouteValueRequestCultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var path = httpContext.Request.Path.Value;

            if (string.IsNullOrEmpty(path))
                return Task.FromResult<ProviderCultureResult>(null);

            // lấy segment đầu tiên: /en/... hoặc /vi/...
            var parts = path.Split('/');
            if (parts.Length < 2)
                return Task.FromResult<ProviderCultureResult>(null);

            var culture = parts[1];
            if (string.IsNullOrEmpty(culture))
                return Task.FromResult<ProviderCultureResult>(null);

            return Task.FromResult(new ProviderCultureResult(culture, culture));
        }
    }
}
