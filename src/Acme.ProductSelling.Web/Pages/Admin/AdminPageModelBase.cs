using Acme.ProductSelling.Users;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Web.Pages.Admin
{
    public abstract class AdminPageModelBase : AbpPageModel
    {
        protected IIdentityUserRepository UserRepository => LazyServiceProvider.LazyGetRequiredService<IIdentityUserRepository>();
        protected ICurrentUser CurrentUserService => LazyServiceProvider.LazyGetRequiredService<ICurrentUser>();

        public string RoleBasedPrefix { get; private set; }
        public Guid? CurrentUserStoreId { get; private set; }
        public string CurrentUserStoreName { get; private set; }
        public UserType CurrentUserType { get; private set; }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            // Set role-based URL prefix
            RoleBasedPrefix = await GetRoleBasedPrefixAsync();
            ViewData["RoleBasedPrefix"] = RoleBasedPrefix;

            // Get current user's store assignment
            if (CurrentUserService.IsAuthenticated && CurrentUserService.Id.HasValue)
            {
                var user = await UserRepository.GetAsync(CurrentUserService.Id.Value) as AppUser;
                CurrentUserStoreId = user?.AssignedStoreId;
                CurrentUserType = user?.GetUserType() ?? UserType.Admin;

                if (CurrentUserStoreId.HasValue)
                {
                    var storeRepo = LazyServiceProvider.LazyGetRequiredService<Acme.ProductSelling.Stores.IStoreRepository>();
                    var store = await storeRepo.GetAsync(CurrentUserStoreId.Value);
                    CurrentUserStoreName = store.Name;
                }

                ViewData["CurrentUserStoreId"] = CurrentUserStoreId;
                ViewData["CurrentUserStoreName"] = CurrentUserStoreName;
                ViewData["CurrentUserType"] = CurrentUserType;
            }

            await base.OnPageHandlerExecutionAsync(context, next);
            await next();
        }

        protected async Task<string> GetRoleBasedPrefixAsync()
        {
            if (!CurrentUserService.IsAuthenticated || !CurrentUserService.Id.HasValue)
                return "admin";

            var user = await UserRepository.GetAsync(CurrentUserService.Id.Value);
            var roles = await UserRepository.GetRolesAsync(user.Id);

            // Priority order
            if (roles.Any(r => string.Equals(r.Name, Acme.ProductSelling.Identity.IdentityRoleConsts.Blogger, StringComparison.OrdinalIgnoreCase)))
                return "blogger";
            if (roles.Any(r => string.Equals(r.Name, Acme.ProductSelling.Identity.IdentityRoleConsts.Manager, StringComparison.OrdinalIgnoreCase)))
                return "manager";
            if (roles.Any(r => string.Equals(r.Name, Acme.ProductSelling.Identity.IdentityRoleConsts.Seller, StringComparison.OrdinalIgnoreCase)))
                return "seller";
            if (roles.Any(r => string.Equals(r.Name, Acme.ProductSelling.Identity.IdentityRoleConsts.Cashier, StringComparison.OrdinalIgnoreCase)))
                return "cashier";
            if (roles.Any(r => string.Equals(r.Name, Acme.ProductSelling.Identity.IdentityRoleConsts.WarehouseStaff, StringComparison.OrdinalIgnoreCase)))
                return "warehouse";

            return "admin";
        }

        protected string GetUrl(string path)
        {
            return $"/{RoleBasedPrefix}{path}";
        }

        public bool IsAdminOrManager()
        {
            return CurrentUserType == UserType.Admin;
        }

        protected async Task<bool> IsInRoleAsync(string roleName)
        {
            if (!CurrentUserService.IsAuthenticated || !CurrentUserService.Id.HasValue)
                return false;

            var user = await UserRepository.GetAsync(CurrentUserService.Id.Value);
            var roles = await UserRepository.GetRolesAsync(user.Id);
            return roles.Any(r => string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
