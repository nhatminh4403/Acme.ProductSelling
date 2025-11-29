using Acme.ProductSelling.Carts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Carts
{
    [Authorize]
    public class CartModel : AbpPageModel
    {
        public CartDto Cart { get; set; }
        private readonly ICartAppService _cartAppService;

        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
        public decimal TotalPrice { get; set; }
        public int TotalItemCount { get; set; }

        public CartModel(ICartAppService cartAppService)
        {
            _cartAppService = cartAppService;
            Cart = new CartDto();
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var cart = await _cartAppService.GetUserCartAsync();
                Cart = cart;
                CartItems = cart.CartItems.ToList();
                TotalPrice = cart.TotalPrice;
                TotalItemCount = cart.TotalItemCount;

                return Page();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading cart");
                Alerts.Danger(L["Cart:LoadError"]);
                return Page();
            }
        }
        public async Task<IActionResult> OnPostUpdateItemAsync(Guid cartItemId, int quantity)
        {
            if (!ModelState.IsValid)
            {
                Logger.LogWarning("Invalid ModelState for UpdateItem");
                Alerts.Warning(L["Cart:InvalidInput"]);
                return await OnGetAsync();
            }

            var input = new UpdateCartItemInput
            {
                CartItemId = cartItemId,
                Quantity = quantity
            };

            Logger.LogInformation($"Updating cart item: {cartItemId}, Quantity: {quantity}");

            try
            {
                await _cartAppService.UpdateCartItemAsync(input);
                Alerts.Success(L["Cart:ItemUpdated"]);
                return RedirectToPage();
            }
            catch (UserFriendlyException ex)
            {
                Logger.LogWarning(ex, "User friendly error updating cart item");
                Alerts.Warning(ex.Message);
                return await OnGetAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating cart item");
                Alerts.Danger(L["Cart:GenericError"]);
                return await OnGetAsync();
            }
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(Guid cartItemId)
        {
            Logger.LogInformation($"Attempting to remove cart item: {cartItemId}");

            if (cartItemId == Guid.Empty)
            {
                Logger.LogWarning("CartItemId is empty");
                Alerts.Warning(L["Cart:InvalidInput"]);
                return await OnGetAsync();
            }

            try
            {
                await _cartAppService.RemoveCartItemAsync(cartItemId);
                Alerts.Success(L["Cart:ItemRemoved"]);
                return RedirectToPage();
            }
            catch (UserFriendlyException ex)
            {
                Logger.LogWarning(ex, "User friendly error removing cart item");
                Alerts.Warning(ex.Message);
                return await OnGetAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error removing cart item {cartItemId}");
                Alerts.Danger(L["Cart:GenericError"]);
                return await OnGetAsync();
            }
        }
        public async Task<IActionResult> OnPostClearCartAsync()
        {
            Logger.LogInformation("Attempting to clear cart");

            try
            {
                await _cartAppService.ClearCartAsync();
                Alerts.Success(L["Cart:CartCleared"]);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error clearing cart");
                Alerts.Danger(L["Cart:GenericError"]);
                return await OnGetAsync();
            }
        }
    }

}