using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.UI.Navigation;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Themes.LeptonXLite.Components.Menu;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Themes.LeptonXLite.Components.Menu;

public class MainNavbarMenuViewComponent : AbpViewComponent
{
    protected IMenuManager MenuManager { get; }

    public MainNavbarMenuViewComponent(IMenuManager menuManager)
    {
        MenuManager = menuManager;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        var menu = await MenuManager.GetMainMenuAsync();
        var model = new MenuViewModel
        {
            Menu = menu
        };

        return View("~/Themes/LeptonXLite/Components/Menu/Default.cshtml", model);
    }
}