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
            var path = context.HttpContext.Request.Path.Value?.ToLower();

            if (_currentUser.IsAuthenticated)
            {

                // List of paths to redirect (login and register)
                if (path != null && IsAuthPage(path))
                {
                    context.Result = new RedirectResult("/");
                    return;
                }
            }

            await next.Invoke();
        }
        private bool IsAuthPage(string? path)
        {
            if (path == null) return false;
            return path.Contains("/account/login")
                || path.Contains("/account/register")
                || path.Contains("/identity/account/login")
                || path.Contains("/identity/account/register");
        }
    }
}
