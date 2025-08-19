using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Globalization;

namespace Acme.ProductSelling.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        private static void AddCultureToRouteValues(RouteValueDictionary values)
        {
            if (!values.ContainsKey("culture"))
            {
                values["culture"] = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            }
        }

        public static string PageWithCulture(this IUrlHelper urlHelper,
            string pageName,
            string pageHandler = null,
            object values = null,
            string protocol = null,
            string host = null,
            string fragment = null)
        {
            var routeValues = new RouteValueDictionary(values);
            AddCultureToRouteValues(routeValues);

            return urlHelper.Page(pageName, pageHandler, routeValues, protocol, host, fragment);
        }
    }
}
