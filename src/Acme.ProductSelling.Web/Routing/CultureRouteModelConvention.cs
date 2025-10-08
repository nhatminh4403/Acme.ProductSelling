using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;

namespace Acme.ProductSelling.Web.Routing
{

    public class CultureRouteModelConvention : IPageRouteModelConvention
    {
        private readonly string[] _supportedCultures;
        private readonly string _defaultCulture;
        private readonly string[] _excludedPrefixes = new[]
        {
            "/admin",
            "/identity",
            "/account",
            "/tenantmanagement",
            "/setting-management",
            "/swagger",
            "/api"
        };
        public CultureRouteModelConvention(string[] supportedCultures, string defaultCulture)
        {
            _supportedCultures = supportedCultures;
            _defaultCulture = defaultCulture;
        }
        public void Apply(PageRouteModel model)
        {
            var selectorModel = model.Selectors.FirstOrDefault();
            if (selectorModel?.AttributeRouteModel?.Template == null)
                return;

            var template = selectorModel.AttributeRouteModel.Template;

            // Check if this page should be excluded from culture routing
            foreach (var excludedPrefix in _excludedPrefixes)
            {
                if (template.StartsWith(excludedPrefix.TrimStart('/'), System.StringComparison.OrdinalIgnoreCase))
                {
                    // Don't apply culture routes to excluded pages
                    return;
                }
            }

            // Apply culture routing only to non-excluded pages
            var selectors = model.Selectors.ToList();
            model.Selectors.Clear();

            foreach (var selector in selectors)
            {
                foreach (var culture in _supportedCultures)
                {
                    var originalTemplate = selector.AttributeRouteModel.Template;
                    model.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel
                        {
                            Template = $"{culture}/{originalTemplate}"
                        }
                    });
                }

                // Keep the original route as fallback
                model.Selectors.Add(selector);
            }
        }
    }
}
