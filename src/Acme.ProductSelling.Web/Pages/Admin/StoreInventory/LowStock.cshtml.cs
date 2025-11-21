using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.StoreInventories.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Acme.ProductSelling.Web.Pages.Admin.StoreInventory
{
    [Authorize(ProductSellingPermissions.Inventory.Default)]
    public class LowStockModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }

        private readonly IStoreInventoryAppService _storeInventoryAppService;

        public LowStockModel(IStoreInventoryAppService storeInventoryAppService)
        {
            _storeInventoryAppService = storeInventoryAppService;
        }

        public void OnGet()
        {
            if (Prefix != RoleBasedPrefix)
            {
                Response.Redirect(GetUrl("/store-inventory/low-stock"));
            }
        }
    }
}
