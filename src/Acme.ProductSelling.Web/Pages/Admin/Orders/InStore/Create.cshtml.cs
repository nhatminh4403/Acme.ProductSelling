using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.StoreInventories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Web.Pages.Admin.Orders.InStore
{
    [Authorize(ProductSellingPermissions.Orders.Create)]
    public class CreateModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }
        [BindProperty]
        public CreateInStoreOrderDto Order { get; set; }

        public List<SelectListItem> AvailableProducts { get; set; }

        private readonly IOrderAppService _orderAppService;
        private readonly IProductRepository _productRepository;
        private readonly IStoreInventoryRepository _storeInventoryRepository;

        public CreateModel(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
        }
        public async Task OnGet()
        {
            if (!CurrentUserStoreId.HasValue)
            {
                throw new UserFriendlyException("You must be assigned to a store to create orders.");
            }

            Order = new CreateInStoreOrderDto
            {
                Items = new List<CreateOrderItemDto>()
            };
            await LoadAvailableProductsAsync();
            if (Prefix != RoleBasedPrefix)
            {
                Response.Redirect(GetUrl("/orders/in-store/create"));
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadAvailableProductsAsync();
                return Page();
            }

            try
            {
                await _orderAppService.CreateInStoreOrderAsync(Order);
                return Redirect(GetUrl("/orders/in-store"));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await LoadAvailableProductsAsync();
                return Page();
            }
        }


        private async Task LoadAvailableProductsAsync()
        {
            if (!CurrentUserStoreId.HasValue)
            {
                AvailableProducts = new List<SelectListItem>();
                return;
            }

            var inventories = await _storeInventoryRepository
                .GetByStoreAsync(CurrentUserStoreId.Value);

            var productIds = inventories
                .Where(i => i.IsAvailableForSale && i.Quantity > 0)
                .Select(i => i.ProductId)
                .ToList();

            var products = await _productRepository.GetListAsync(p => productIds.Contains(p.Id));

            AvailableProducts = products
                .Where(p => p.IsAvailableForPurchase())
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.ProductName} - {(p.DiscountedPrice ?? p.OriginalPrice):N0} VND (Stock: {inventories.First(i => i.ProductId == p.Id).Quantity})"
                })
                .ToList();
        }
    }
}
