using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Checkout
{
    [Authorize]
    public class CheckoutModel : ProductSellingPageModel
    {
        [BindProperty]
        public CreateOrderDto OrderInput { get; set; }
        [BindProperty]
        public CartDto CurrentCart { get; private set; }

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
            CurrentCart = await _cartAppService.GetAsync();
            if (CurrentCart == null || !CurrentCart.CartItems.Any())
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
            // Step 1: Reload cart to verify it's not empty
            CurrentCart = await _cartAppService.GetAsync();
            if (CurrentCart == null || !CurrentCart.CartItems.Any())
            {
                Alerts.Warning(L["ShoppingCartIsEmptyCannotPlaceOrder"]);
                return RedirectToPage("/Cart");
            }

            // Step 2: Map cart items to order items
            OrderInput.Items = CurrentCart.CartItems.Select(
                                cartItem => new CreateOrderItemDto
                                {
                                    ProductId = cartItem.ProductId,
                                    Quantity = cartItem.Quantity
                                })
                                .ToList();

            // Step 3: Validate model state
            if (!ModelState.IsValid)
            {
                Logger.LogWarning("--- BẮT ĐẦU KIỂM TRA MODELSTATE LỖI ---");
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        Logger.LogWarning($"Key: {modelStateKey} | Error: {error.ErrorMessage}");
                    }
                }
                // IMPORTANT: Reload cart before returning to page
                CurrentCart = await _cartAppService.GetAsync();
                Logger.LogWarning("--- KẾT THÚC KIỂM TRA MODELSTATE LỖI ---");
                return Page();
            }

            try
            {
                Logger.LogInformation("ModelState hợp lệ. Bắt đầu gọi OrderAppService.CreateAsync.");
                Logger.LogInformation("Phương thức thanh toán đã chọn: {PaymentMethod}", OrderInput.PaymentMethod);

                // Step 4: Create the order
                var createdOrder = await _orderAppService.CreateAsync(OrderInput);

                // Step 5: Validate result
                if (createdOrder == null)
                {
                    Logger.LogError("CreateAsync returned null result");
                    Alerts.Danger(L["Order:CreationFailed"]);
                    CurrentCart = await _cartAppService.GetAsync();
                    return Page();
                }

                if (createdOrder.Order == null)
                {
                    Logger.LogError("Order creation failed, createdOrder.Order is null");
                    Alerts.Danger(L["Order:CreationFailed"]);
                    CurrentCart = await _cartAppService.GetAsync();
                    return Page();
                }

                Logger.LogInformation("Order created successfully: {OrderId}, {OrderNumber}",
                    createdOrder.Order.Id, createdOrder.Order.OrderNumber);

                // Step 6: Check if we need to redirect to payment gateway
                if (!string.IsNullOrEmpty(createdOrder.RedirectUrl))
                {
                    Logger.LogInformation("Redirecting to payment gateway: {RedirectUrl}", createdOrder.RedirectUrl);
                    return Redirect(createdOrder.RedirectUrl);
                }

                // Step 7: For COD orders, clear cart and redirect to confirmation
                Logger.LogInformation("Clearing cart and redirecting to order confirmation");
                await _cartAppService.ClearAsync();

                return RedirectToPage("/Orders/OrderConfirmation",
                    new { orderId = createdOrder.Order.Id, orderNumber = createdOrder.Order.OrderNumber });
            }
            catch (UserFriendlyException ex)
            {
                Logger.LogWarning(ex, "User friendly exception during order creation: {Message}", ex.Message);
                Alerts.Warning(ex.Message);
                // Reload cart before returning to page
                CurrentCart = await _cartAppService.GetAsync();
                return Page();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error creating order: {Message}", ex.Message);
                Alerts.Danger(L["Order:UnexpectedError"]);
                // Reload cart before returning to page
                CurrentCart = await _cartAppService.GetAsync();
                return Page();
            }
        }
    }
}