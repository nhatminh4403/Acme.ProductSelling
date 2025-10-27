using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Acme.ProductSelling.Web.Middleware
{
    public class QueryStringFilterMiddleware
    {
        private readonly RequestDelegate _next;

        public QueryStringFilterMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var queryString = context.Request.QueryString;

            var hasInvalidChars = context.Request.Query.Any(param =>
                param.Value.ToString().Contains("%2F", System.StringComparison.OrdinalIgnoreCase) ||
                param.Value.ToString().Contains("../") ||
                param.Value.ToString().Contains("..\\")
            );

            if (hasInvalidChars)
            {

                context.Response.StatusCode = 400; 

                return;
            }
            await _next(context);
        }
    }
}
