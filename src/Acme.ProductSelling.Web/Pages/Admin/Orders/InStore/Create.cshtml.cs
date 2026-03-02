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

namespace Acme.ProductSelling.Web.Pages.Admin.Orders.InStore
{
    [Authorize(ProductSellingPermissions.Orders.Create)]
    public class CreateModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }
        [BindProperty]
        public CreateInStoreOrderDto Order { get; set; }
        public List<SelectListItem> PaymentMethods { get; set; }
        public List<SelectListItem> AvailableProducts { get; set; }

        private readonly IOrderAppService _orderAppService;
        private readonly IProductRepository _productRepository;
        private readonly IProductAppService _productAppService;
        private readonly IStoreInventoryRepository _storeInventoryRepository;

        public CreateModel(IOrderAppService orderAppService, IProductAppService productAppService,
            IProductRepository productRepository, IStoreInventoryRepository storeInventoryRepository)
        {
            _orderAppService = orderAppService;
            _productAppService = productAppService;
            _productRepository = productRepository;
            _storeInventoryRepository = storeInventoryRepository;
        }
        public async Task OnGet()
        {
            if (CurrentUser.IsInRole(Acme.ProductSelling.Identity.IdentityRoleConsts.Admin) || CurrentUserStoreId.HasValue)
            {
                Order = new CreateInStoreOrderDto
                {
                    Items = new List<CreateOrderItemDto>()
                };
                await LoadAvailableProductsAsync();
                LoadPaymentMethods();

                if (Prefix != RoleBasedPrefix)
                {
                    Response.Redirect(GetUrl("/orders/in-store/create"));
                }
            }

            else
            {
                throw new UserFriendlyException("You must be assigned to a store to create orders.");
            }


        }
        private void LoadPaymentMethods()
        {
            PaymentMethods = new List<SelectListItem>
                {
                    new SelectListItem(L["Cash"], "Cash"),
                    new SelectListItem(L["Card"], "Card"),
                    new SelectListItem(L["E-Wallet"], "E-Wallet")
                };
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadAvailableProductsAsync();
                LoadPaymentMethods();

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
                LoadPaymentMethods();

                return Page();
            }
        }


        private async Task LoadAvailableProductsAsync()
        {
            // Case 1: User is assigned to a specific store
            if (CurrentUserStoreId.HasValue)
            {
                var inventories = await _storeInventoryRepository.GetByStoreAsync(CurrentUserStoreId.Value);

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
                        Text = $"{p.ProductName} - {(p.DiscountedPrice ?? p.OriginalPrice):N0} VND " +
                               $"(Stock: {inventories.FirstOrDefault(i => i.ProductId == p.Id)?.Quantity ?? 0})"
                    })
                    .ToList();
            }
            // Case 2: Admin checking (No assigned store)
            else if (CurrentUser.IsInRole(Acme.ProductSelling.Identity.IdentityRoleConsts.Admin))
            {
                var products = await _productRepository.GetListAsync();

                AvailableProducts = products
                    .Where(p => p.IsAvailableForPurchase())
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = $"{p.ProductName} - {(p.DiscountedPrice ?? p.OriginalPrice):N0} VND (Admin Preview - No Store Selected)"
                    })
                    .ToList();
            }
            else
            {
                AvailableProducts = new List<SelectListItem>();
            }
        }
    }
}
