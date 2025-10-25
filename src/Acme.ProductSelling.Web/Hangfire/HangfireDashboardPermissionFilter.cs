using Acme.ProductSelling.Permissions;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ProductSelling.Web.Hangfire
{
    public class HangfireDashboardPermissionFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            var authorizationService = httpContext.RequestServices.GetRequiredService<IAuthorizationService>();

            var isGranted = authorizationService
                                .IsGrantedAsync(ProductSellingPermissions.Hangfire.Dashboard)
                                .GetAwaiter()
                                .GetResult();

            return isGranted;
        }
    }
}
