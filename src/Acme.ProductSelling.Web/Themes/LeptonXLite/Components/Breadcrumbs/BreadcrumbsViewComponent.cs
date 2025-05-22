using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Layout;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Themes.LeptonXLite.Components.BreadCrumbs;

namespace Acme.ProductSelling.Web.Themes.LeptonXLite.Components.Breadcrumbs
{
    public class BreadcrumbsViewComponent : AbpViewComponent
    {
        private readonly IPageLayout _pageLayout;

        public BreadcrumbsViewComponent(IPageLayout pageLayout)
        {
            _pageLayout = pageLayout;
        }

        public virtual IViewComponentResult Invoke()
        {
            return View("~/Themes/LeptonXLite/Components/Breadcrumbs/Default.cshtml", _pageLayout.Content.BreadCrumb.Items);
        }
    }

}
