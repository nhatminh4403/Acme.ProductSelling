using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Themes.LeptonXLite.Components.Menu;
using Volo.Abp.UI.Navigation;

namespace Acme.ProductSelling.Web.Pages.Admin.Shared.Components.Menu;

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

        return View("/Pages/Admin/Shared/Components/Menu/Default.cshtml", model);
    }
}