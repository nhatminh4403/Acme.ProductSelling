using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Layout;

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
