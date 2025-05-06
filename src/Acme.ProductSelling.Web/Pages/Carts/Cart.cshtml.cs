using Acme.ProductSelling.Carts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
                // Tải lại trang để hiển thị giỏ hàng mới
                return RedirectToPage();
            }
            catch (UserFriendlyException ex)
            {
                // Hiển thị lỗi cho người dùng (ví dụ: không đủ hàng)
                // Lưu lỗi vào TempData hoặc một cách khác để hiển thị trên trang sau khi redirect
                Alerts.Warning(ex.Message); // Sử dụng hệ thống Alert của ABP
                return RedirectToPage(); // Vẫn tải lại trang
            }
            catch (Exception ex) // Lỗi không mong muốn
            {
                Logger.LogError(ex, "Error updating cart item.");
                Alerts.Danger("An error occurred while updating your cart.");
                return RedirectToPage();
            }
        }

        // Page Handler xử lý khi người dùng nhấn nút Remove
        public async Task<IActionResult> OnPostRemoveItemAsync(Guid cartItemId)
        {
            try
            {
                await _cartAppService.RemoveItemAsync(cartItemId);
                Alerts.Success("Item removed from cart."); // Thông báo thành công
                return RedirectToPage(); // Tải lại trang
            }
            catch (Exception ex) // Lỗi không mong muốn
            {
                Logger.LogError(ex, "Error removing cart item.");
                Alerts.Danger("An error occurred while removing the item.");
                return RedirectToPage();
            }
        }
    }

}