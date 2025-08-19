using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Acme.ProductSelling.Web.Routing
{
    public static class UrlHelperExtensions
    {
        public static string CultureAction(this IUrlHelper urlHelper, string culture, string page = null)
        {
            var routeValues = new RouteValueDictionary(urlHelper.ActionContext.RouteData.Values);

            if (!string.IsNullOrEmpty(culture))
            {
                routeValues["culture"] = culture;
            }
            else
            {
                routeValues.Remove("culture");
            }

            if (!string.IsNullOrEmpty(page))
            {
                return urlHelper.Page(page, routeValues);
            }

            return urlHelper.Page(routeValues["page"]?.ToString(), routeValues);
        }
    }
}
