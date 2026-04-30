using Acme.ProductSelling.Account;
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
using Volo.Abp.Users;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        private readonly IOrderPublicAppService _orderPublicAppService;
        private readonly IProfileAppService _profileAppService;
        public CheckoutModel(ICartAppService cartService,

                             IOrderPublicAppService orderPublicAppService)
        {
            _cartAppService = cartService;
            OrderInput = new CreateOrderDto();
            _orderPublicAppService = orderPublicAppService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentCart = await _cartAppService.GetUserCartAsync();
            if (CurrentCart == null || !CurrentCart.CartItems.Any())
            {
                return RedirectToPage("/cart");
            }


            if (CurrentUser.IsAuthenticated)
            {
                var currentProfile = await _profileAppService.GetAsync();
                if (currentProfile == null)
                {
                    throw new Exception($"there is no {currentProfile} profile in db");
                }

                OrderInput.CustomerName = currentProfile.Name ?? CurrentUser.UserName;
                OrderInput.CustomerPhone = currentProfile.PhoneNumber;
                OrderInput.ShippingAddress = currentProfile.ShippingAddress;

                OrderInput.ShippingAddress = currentProfile.ShippingAddress;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Step 1: Reload cart to verify it's not empty
            CurrentCart = await _cartAppService.GetUserCartAsync();
            if (CurrentCart == null || !CurrentCart.CartItems.Any())
            {
                Alerts.Warning(L["IsEmptyCannotOrder"]);
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
                CurrentCart = await _cartAppService.GetUserCartAsync();
                Logger.LogWarning("--- KẾT THÚC KIỂM TRA MODELSTATE LỖI ---");
                return Page();
            }

            try
            {
                Logger.LogInformation("ModelState hợp lệ. Bắt đầu gọi OrderAppService.CreateAsync.");
                Logger.LogInformation("Phương thức thanh toán đã chọn: {PaymentMethod}", OrderInput.PaymentMethod);

                // Step 4: Create the order
                //var createdOrder = await _orderAppService.CreateAsync(OrderInput);
                var createdOrderPublic = await _orderPublicAppService.CreateAsync(OrderInput);
                // Step 5: Validate result
                if (createdOrderPublic == null)
                {
                    Logger.LogError("CreateAsync returned null result");
                    Alerts.Danger(L["Order:CreationFailed"]);
                    CurrentCart = await _cartAppService.GetUserCartAsync();
                    return Page();
                }

                if (createdOrderPublic.Order == null)
                {
                    Logger.LogError("Order creation failed, createdOrder.Order is null");
                    Alerts.Danger(L["Order:CreationFailed"]);
                    CurrentCart = await _cartAppService.GetUserCartAsync();
                    return Page();
                }

                Logger.LogInformation("Order created successfully: {OrderId}, {OrderNumber}",
                    createdOrderPublic.Order.Id, createdOrderPublic.Order.OrderNumber);

                // Step 6: Check if we need to redirect to payment gateway
                if (!string.IsNullOrEmpty(createdOrderPublic.RedirectUrl))
                {
                    Logger.LogInformation("Redirecting to payment gateway: {RedirectUrl}", createdOrderPublic.RedirectUrl);
                    return Redirect(createdOrderPublic.RedirectUrl);
                }

                // Step 7: For COD orders, clear cart and redirect to confirmation
                Logger.LogInformation("Clearing cart and redirecting to order confirmation");
                await _cartAppService.ClearCartAsync();

                return RedirectToPage("/Orders/OrderConfirmation",
                    new { orderId = createdOrderPublic.Order.Id, orderNumber = createdOrderPublic.Order.OrderNumber });
            }
            catch (UserFriendlyException ex)
            {
                Logger.LogWarning(ex, "User friendly exception during order creation: {Message}", ex.Message);
                Alerts.Warning(ex.Message);
                // Reload cart before returning to page
                CurrentCart = await _cartAppService.GetUserCartAsync();
                return Page();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error creating order: {Message}", ex.Message);
                Alerts.Danger(L["Order:UnexpectedError"]);
                // Reload cart before returning to page
                CurrentCart = await _cartAppService.GetUserCartAsync();
                return Page();
            }
        }
    }
}