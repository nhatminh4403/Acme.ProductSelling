using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.StoreInventories;
using Acme.ProductSelling.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
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
        public List<AvailableProducts> AvailableProducts { get; set; }

        private readonly IOrderPublicAppService _orderPublicAppService;
        private readonly IInStoreOrderAppService _inStoreOrderAppService;
        private readonly IProductAppService _productAppService;
        private readonly IStoreInventoryRepository _storeInventoryRepository;
        private readonly IOrderQueryAppService _orderQueryAppService;
        public CreateModel(IProductAppService productAppService,
            IStoreInventoryRepository storeInventoryRepository,
            IOrderPublicAppService orderPublicAppService,
            IInStoreOrderAppService inStoreOrderAppService,
            IOrderQueryAppService orderQueryAppService)
        {
            _productAppService = productAppService;
            _storeInventoryRepository = storeInventoryRepository;
            _orderPublicAppService = orderPublicAppService;
            _inStoreOrderAppService = inStoreOrderAppService;
            _orderQueryAppService = orderQueryAppService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (CurrentUser.IsInRole(Acme.ProductSelling.Identity.ExtendedRoleConsts.Admin) ||
                CurrentUserStoreId.HasValue)
            {
                Order = new CreateInStoreOrderDto
                {
                    Items = new List<CreateOrderItemDto>()
                };

                await LoadAvailableProductsAsync();
                LoadPaymentMethods();

                if (Prefix != RoleBasedPrefix)
                {
                    return RedirectToPage(GetUrl("/orders/in-store/create"));
                }
                return Page();
            }

            else
            {
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.UserNotAssignedToStore);
            }


        }

        public async Task<IActionResult> OnPostAsync()
        {
            Logger.LogWarning("Order is null: {isNull}", Order == null);
            Logger.LogWarning("Order.Items count: {count}", Order?.Items?.Count ?? -1);
            Logger.LogWarning("Order.CustomerName: {name}", Order?.CustomerName);
            Logger.LogWarning("Order.PaymentMethod: {method}", Order?.PaymentMethod);
            Logger.LogInformation("Order Item {count}", Order.Items.Count());
            if (!ModelState.IsValid)
            {
                Logger.LogWarning("--- Start checking MODELSTATE errors ---");
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        Logger.LogWarning($"Key: {modelStateKey} | Error: {error.ErrorMessage}");
                    }
                }
                // IMPORTANT: Reload cart before returning to page
                await LoadAvailableProductsAsync();
                LoadPaymentMethods();
                Logger.LogWarning("--- end checking MODELSTATE Error ---");


                return Page();
            }
            try
            {
                if (!CurrentUserStoreId.HasValue)
                {
                    throw new UserFriendlyException(ProductSellingDomainErrorCodes.UserNotAssignedToStore);
                }
                else Order.CurrentUserStoreId = CurrentUserStoreId.Value;
                if (Order == null || !Order.Items.Any())
                {
                    throw new UserFriendlyException("Order is Empty");
                    await LoadAvailableProductsAsync();
                    LoadPaymentMethods();
                    return Page();
                }
                await _inStoreOrderAppService.CreateInStoreOrderAsync(Order);
                return Redirect(GetUrl("/orders/in-store"));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to create in-store order: {Message}", ex.Message);
                Console.WriteLine($"Exception Error: {ex}");
                await LoadAvailableProductsAsync();
                LoadPaymentMethods();

                return Page();
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

        private async Task LoadAvailableProductsAsync()
        {
            // Case 1: User is assigned to a specific store
            var availableProducts = await _productAppService.LoadAvailableProductsAsync(CurrentUserStoreId);

            AvailableProducts = availableProducts.Select(p => new AvailableProducts
            {
                Id = p.Id,
                Price = p.Price,
                Text = p.IsAdminPreview
                    ? $"{p.ProductName} - {p.Price:N0} VND (Admin Preview - No Store Selected)"
                    : $"{p.ProductName} - {p.Price:N0} VND (Stock: {p.Stock ?? 0})"
            }).ToList();
        }
    }
}