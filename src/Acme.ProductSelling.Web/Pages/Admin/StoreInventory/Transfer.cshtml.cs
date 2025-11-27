using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.StoreInventories.Dtos;
using Acme.ProductSelling.StoreInventories.Services;
using Acme.ProductSelling.Stores.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Web.Pages.Admin.StoreInventory
{
    [Authorize(ProductSellingPermissions.Inventory.Transfer)]
    [Authorize(ProductSellingPermissions.Inventory.Default)]
    public class TransferModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }
        [BindProperty]
        public TransferInventoryDto Transfer { get; set; }

        public List<SelectListItem> Stores { get; set; }
        public List<SelectListItem> Products { get; set; }

        private readonly IStoreInventoryAppService _storeInventoryAppService;
        private readonly IStoreAppService _storeAppService;
        private readonly IProductAppService _productAppService;

        public TransferModel(IStoreInventoryAppService storeInventoryAppService, IStoreAppService storeAppService, IProductAppService productAppService)
        {
            _storeInventoryAppService = storeInventoryAppService;
            _storeAppService = storeAppService;
            _productAppService = productAppService;
        }

        public async Task OnGetAsync()
        {
            Transfer = new TransferInventoryDto();
            await LoadLookupsAsync();

            if (Prefix != RoleBasedPrefix)
            {
                Response.Redirect(GetUrl("/store-inventory/transfer"));
            }

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return Page();
            }

            await _storeInventoryAppService.TransferInventoryAsync(Transfer);
            return Redirect(GetUrl("/store-inventory"));
        }
        private async Task LoadLookupsAsync()
        {
            var stores = await _storeAppService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1000 });
            Stores = stores.Items
                .Select(s => new SelectListItem(s.Name, s.Id.ToString()))
                .ToList();

            // Load products
            var products = await _productAppService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1000 });
            Products = products.Items
                .Select(p => new SelectListItem(p.ProductName, p.Id.ToString()))
                .ToList();
        }

    }
}
