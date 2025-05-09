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
        [BindProperty]
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
                return RedirectToPage("/" + "Cart".ToLower());
            }

            if (CurrentUser.IsAuthenticated)
            {
                OrderInput.CustomerName = CurrentUser.Name ?? CurrentUser.UserName;
                OrderInput.CustomerPhone = CurrentUser.PhoneNumber;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CurrentCart = await _cartAppService.GetAsync();
            if (!CurrentCart.CartItems.Any())
            {
                Alerts.Warning(L["ShoppingCartIsEmptyCannotPlaceOrder"]);
                return RedirectToPage("/Cart");
            }
            OrderInput.Items = CurrentCart.CartItems.Select(
                                cartItem => new CreateOrderItemDto
                                {
                                    ProductId = cartItem.ProductId,
                                    Quantity = cartItem.Quantity
                                })
                                .ToList();
            if (!ModelState.IsValid)
            {
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                    }
                }
                CurrentCart = await _cartAppService.GetAsync();
                return Page();
            }
            try
            {

                var createdOrder = await _orderAppService.CreateAsync(OrderInput);

                await _cartAppService.ClearAsync();

                // Chuyển hướng đến trang xác nhận
                return RedirectToPage("/Orders/OrderConfirmation",
                    new { orderId = createdOrder.Id, orderNumber = createdOrder.OrderNumber });
            }
            catch (UserFriendlyException ex)
            {
                Alerts.Warning(ex.Message); 
                return Page(); 
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating order.");
                Alerts.Danger("An error occurred while placing your order.");
                Console.WriteLine("Error", ex.Message);
                return Page();
            }
        }
    }
}
