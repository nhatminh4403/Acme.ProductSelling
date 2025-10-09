using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;

namespace Acme.ProductSelling.Web.Routing
{
    [HtmlTargetElement("a", Attributes = "asp-page")]
    [HtmlTargetElement("a", Attributes = "asp-controller")]
    [HtmlTargetElement("a", Attributes = "asp-action")]
    public class CultureAwareAnchorTagHelper : AnchorTagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CultureAwareAnchorTagHelper(IHttpContextAccessor httpContextAccessor, IHtmlGenerator generator) : base(generator)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (output.Attributes.ContainsName("href"))
            {
                return;
            }

            if (RouteValues == null)
            {
                RouteValues = new Dictionary<string, string>();
            }

            if (!RouteValues.ContainsKey("culture"))
            {
                var currentCulture = GetCurrentCulture();
                RouteValues["culture"] = currentCulture;
            }

            base.Process(context, output);
        }

        private string GetCurrentCulture()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // Lấy từ URL path
                var path = httpContext.Request.Path.Value;
                if (!string.IsNullOrEmpty(path))
                {
                    var segments = path.Split('/', System.StringSplitOptions.RemoveEmptyEntries);
                    if (segments.Length > 0)
                    {
                        var firstSegment = segments[0];
                        if (IsSupportedCulture(firstSegment))
                        {
                            return firstSegment;
                        }
                    }
                }

                // Lấy từ cookie
                if (httpContext.Request.Cookies.TryGetValue("culture", out var cookieCulture) &&
                    IsSupportedCulture(cookieCulture))
                {
                    return cookieCulture;
                }
            }

            // Default fallback
            return "vi";
        }

        private bool IsSupportedCulture(string culture)
        {
            var supportedCultures = new[] { "en", "vi" };
            foreach (var supportedCulture in supportedCultures)
            {
                if (string.Equals(culture, supportedCulture, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}