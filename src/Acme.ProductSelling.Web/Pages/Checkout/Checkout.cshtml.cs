using Acme.ProductSelling.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using System.Linq;
using Acme.ProductSelling.Carts;
using Microsoft.AspNetCore.Authorization;

namespace Acme.ProductSelling.Web.Pages.Checkout
{
    [Authorize]
    public class CheckoutModel : AbpPageModel
    {
        [BindProperty]
        public CreateOrderDto OrderInput { get; set; } // DTO đơn giản

        public CartDto CurrentCart { get; set; }
        private readonly ICartAppService _cartAppService;

        private readonly IOrderAppService _orderAppService;

        public CheckoutModel(IOrderAppService orderAppService, ICartAppService cartService)
        {
            _orderAppService = orderAppService;
            _cartAppService = cartService;
            OrderInput = new CreateOrderDto();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentCart = await _cartAppService.GetAsync(); // Lấy giỏ hàng hiện tại
            if (!CurrentCart.CartItems.Any())
            {
                // Không có gì để checkout, quay về trang giỏ hàng hoặc trang chủ
                return RedirectToPage("/Cart");
            }

            // Điền sẵn thông tin nếu có thể
            if (CurrentUser.IsAuthenticated)
            {
                OrderInput.CustomerName = CurrentUser.Name ?? CurrentUser.UserName;
                OrderInput.CustomerPhone = CurrentUser.PhoneNumber;
                // Lấy địa chỉ mặc định nếu có...
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CurrentCart = await _cartAppService.GetAsync();
            if (!CurrentCart.CartItems.Any())
            {
                // Ai đó đã xóa giỏ hàng trong lúc người dùng ở trang checkout?
                Alerts.Warning(L["ShoppingCartIsEmptyCannotPlaceOrder"]);
                return RedirectToPage("/Cart");
            }
            if (!ModelState.IsValid)
            {
                // CurrentCart = await _cartService.GetAsync(); // Lấy lại giỏ hàng để hiển thị lại form
                return Page(); // Hiển thị lại form với lỗi validation
            }

            try
            {
                var createdOrder = await _orderAppService.CreateAsync(OrderInput);

                // *** XÓA GIỎ HÀNG SAU KHI ĐẶT HÀNG THÀNH CÔNG ***
                await _cartAppService.ClearAsync();

                // Chuyển hướng đến trang xác nhận
                return RedirectToPage("/OrderConfirmation",
                    new { orderId = createdOrder.Id, orderNumber = createdOrder.OrderNumber });
            }
            catch (UserFriendlyException ex)
            {
                Alerts.Warning(ex.Message); // Hiển thị lỗi (vd: hết hàng)
                return Page(); // Hiển thị lại form checkout
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating order.");
                Alerts.Danger("An error occurred while placing your order.");
                return Page();
            }
        }
    }
}
