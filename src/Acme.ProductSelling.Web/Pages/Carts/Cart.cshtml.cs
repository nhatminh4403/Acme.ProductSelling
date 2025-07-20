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

        public CartModel(ICartAppService cartAppService)
        {
            _cartAppService = cartAppService;
            Cart = new CartDto();
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var cart = await _cartAppService.GetAsync();
            CartItems = cart.CartItems.ToList();
            TotalPrice = cart.TotalPrice;
            Cart = cart;
            return Page();
        }
        public async Task<IActionResult> OnPostUpdateItemAsync(Guid cartItemId, int quantity)
        {
            // Tạo Input DTO
            var input = new UpdateCartItemInput
            {
                CartItemId = cartItemId,
                Quantity = quantity
            };

            try
            {
                // Gọi service để cập nhật
                await _cartAppService.UpdateItemAsync(input);
                return RedirectToPage();
            }
            catch (UserFriendlyException ex)
            {

                Alerts.Warning(ex.Message); 
                return RedirectToPage(); 
            }
            catch (Exception ex) // Lỗi không mong muốn
            {
                Logger.LogError(ex, "Error updating cart item.");
                Alerts.Danger("An error occurred while updating your cart.");
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(Guid cartItemId)
        {
            try
            {
                await _cartAppService.RemoveItemAsync(cartItemId);
                Alerts.Success("Item removed from cart."); 
                return RedirectToPage();
            }
            catch (Exception ex) 
            {
                Logger.LogError(ex, "Error removing cart item.");
                Alerts.Danger("An error occurred while removing the item.");
                return RedirectToPage();
            }
        }
    }

}