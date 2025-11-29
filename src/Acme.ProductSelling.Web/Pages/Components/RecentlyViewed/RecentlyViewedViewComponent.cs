using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Web.Pages.Components.RecentlyViewed
{
    public class RecentlyViewedViewComponent : AbpViewComponent
    {
        private readonly IRecentlyViewedProductAppService _recentlyViewedAppService;
        private readonly ICurrentUser _currentUser;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        public RecentlyViewedViewComponent(
            IRecentlyViewedProductAppService recentlyViewedAppService,
            ICurrentUser currentUser,
            IStringLocalizer<ProductSellingResource> localizer)
        {
            _recentlyViewedAppService = recentlyViewedAppService;
            _currentUser = currentUser;
            _localizer = localizer;
        }

        public IViewComponentResult Invoke(
                int maxCount = 6,
                Guid? excludeProductId = null,
                string title = null)
        {
            if (title == null)
            {
                title = _localizer["UI:RecentlyView"].Value;
            }
            return View(new RecentlyViewedViewModel
            {
                MaxCount = maxCount,
                ExcludeProductId = excludeProductId,
                Title = title,
                IsAuthenticated = _currentUser.IsAuthenticated
            });
        }
    }
}
