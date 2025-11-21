using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.StoreInventories.Dtos;
using Acme.ProductSelling.StoreInventories.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Admin.StoreInventory
{
    [Authorize(ProductSellingPermissions.Inventory.Manage)]
    [Authorize(ProductSellingPermissions.Inventory.Default)]
    [Authorize(ProductSellingPermissions.Products.Edit)]

    public class AdjustModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }
        private readonly IStoreInventoryAppService _storeInventoryAppService;

        [BindProperty]
        public AdjustStoreInventoryDto Adjustment { get; set; }

        public string ProductName { get; set; }
        public string StoreName { get; set; }
        public int CurrentQuantity { get; set; }
        public AdjustModel(IStoreInventoryAppService inventoryAppService)
        {
            _storeInventoryAppService = inventoryAppService;
        }

        public async Task OnGetAsync()
        {
            var inventory = await _storeInventoryAppService.GetAsync(Id);
            ProductName = inventory.ProductName;
            StoreName = inventory.StoreName;
            CurrentQuantity = inventory.Quantity;

            Adjustment = new AdjustStoreInventoryDto();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _storeInventoryAppService.AdjustQuantityAsync(Id, Adjustment);
            return NoContent();
        }
    }
}
