using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Web.Filters
{
    public class AuthenticatedRedirectFilter : IAsyncPageFilter, ITransientDependency
    {
        private readonly ICurrentUser _currentUser;
        public AuthenticatedRedirectFilter(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }
        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (!_currentUser.IsAuthenticated)
            {
                context.Result = new RedirectToPageResult("/Account/Login");
            }
            var path = context.HttpContext.Request.Path.Value?.ToLower();
            if (path != null && (
                               path.Contains("/account/login") ||
                               path.Contains("/account/register") ||
                               path.Contains("/identity/account/login") ||
                               path.Contains("/identity/account/register")))
            {
                context.Result = new RedirectResult("/");
                return;
            }
            await next.Invoke();
        }
    }
}
