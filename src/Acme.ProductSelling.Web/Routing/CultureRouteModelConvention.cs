using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;

namespace Acme.ProductSelling.Web.Routing
{

    public class CultureRouteModelConvention : IPageRouteModelConvention
    {
        private readonly string[] _supportedCultures;
        private readonly string _defaultCulture;

        public CultureRouteModelConvention(string[] supportedCultures, string defaultCulture)
        {
            _supportedCultures = supportedCultures;
            _defaultCulture = defaultCulture;
        }
        public void Apply(PageRouteModel model)
        {
            var selectors = model.Selectors.ToList();
            model.Selectors.Clear();

            foreach (var selector in selectors)
            {
                foreach (var culture in _supportedCultures)
                {
                    var template = selector.AttributeRouteModel.Template;
                    model.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = new AttributeRouteModel
                        {
                            Template = $"{culture}/{template}"
                        }
                    });
                }

                // fallback: route mặc định (không prefix culture)
                model.Selectors.Add(selector);
            }
        }
    }
}
