using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;

namespace Acme.ProductSelling.Web.Routing
{
    public class GlobalCulturePageRouteModelConvention : IPageRouteModelConvention
    {
        private readonly string[] _supportedCultures;
        private readonly string _defaultCulture;
        public GlobalCulturePageRouteModelConvention(string[]? supportedCultures = null, string defaultCulture = "en")
        {
            _supportedCultures = supportedCultures ?? new[] { "en", "vi" };
            _defaultCulture = defaultCulture;
        }
        public void Apply(PageRouteModel model)
        {
            if (HasCultureInExistingRoute(model))
                return;

            // Bỏ qua các pages đặc biệt (error pages, admin pages, etc.)
            if (ShouldSkipPage(model))
                return;

            // Lưu trữ original selectors
            var originalSelectors = model.Selectors.ToList();
            model.Selectors.Clear();

            foreach (var originalSelector in originalSelectors)
            {
                var template = originalSelector.AttributeRouteModel?.Template ?? "";

                // Loại bỏ leading slash nếu có
                template = template.TrimStart('/');

                // 1. Tạo culture route với regex constraint
                var cultureRoute = CreateCultureRoute(originalSelector, template);
                model.Selectors.Add(cultureRoute);

                // 2. Tạo default route (không có culture prefix)
                var defaultRoute = CreateDefaultRoute(originalSelector, template);
                model.Selectors.Add(defaultRoute);
            }
        }
        private SelectorModel CreateCultureRoute(SelectorModel originalSelector, string template)
        {
            var cultureRegex = string.Join("|", _supportedCultures);
            var cultureTemplate = string.IsNullOrEmpty(template)
                ? $"{{culture:regex(^({cultureRegex})$)}}"
                : $"{{culture:regex(^({cultureRegex})$)}}/" + template;

            return new SelectorModel(originalSelector)
            {
                AttributeRouteModel = new AttributeRouteModel
                {
                    Template = cultureTemplate,
                    Name = originalSelector.AttributeRouteModel?.Name,
                    Order = originalSelector.AttributeRouteModel?.Order ?? 0
                }
            };
        }

        private SelectorModel CreateDefaultRoute(SelectorModel originalSelector, string template)
        {
            return new SelectorModel(originalSelector)
            {
                AttributeRouteModel = new AttributeRouteModel
                {
                    Template = template,
                    Name = originalSelector.AttributeRouteModel?.Name,
                    Order = (originalSelector.AttributeRouteModel?.Order ?? 0) + 1 // Lower priority
                }
            };
        }

        private bool HasCultureInExistingRoute(PageRouteModel model)
        {
            return model.Selectors.Any(selector =>
                selector.AttributeRouteModel?.Template?.Contains("{culture") == true);
        }

        private bool ShouldSkipPage(PageRouteModel model)
        {
            var pagePath = model.ViewEnginePath;

            // Bỏ qua error pages
            if (pagePath.Contains("/Error/") || pagePath.Contains("/Errors/"))
                return true;

            // Bỏ qua admin pages
            if (pagePath.Contains("/Admin/"))
                return true;

            // Bỏ qua API endpoints
            if (pagePath.Contains("/Api/"))
                return true;

            // Bỏ qua account pages nếu không muốn localize
            // if (pagePath.Contains("/Account/"))
            //     return true;

            return false;
        }
    }
}
