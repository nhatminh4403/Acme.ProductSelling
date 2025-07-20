using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Web.Pages.Checkout
{
    [Authorize]
    public class CheckoutModel : AbpPageModel
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
            CurrentCart = await _cartAppService.GetAsync();
            if (CurrentCart == null || !CurrentCart.CartItems.Any())
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
                        Logger.LogWarning($"ModelState Error - Key: {modelStateKey}, Error: {error.ErrorMessage}");
                         Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                    }
                }
                return Page();
            }
            try
            {

                var createdOrder = await _orderAppService.CreateAsync(OrderInput);
                if(!string.IsNullOrEmpty(createdOrder.RedirectUrl))
                {
                    return Redirect(createdOrder.RedirectUrl);
                }
                await _cartAppService.ClearAsync();

                return RedirectToPage("/Orders/OrderConfirmation",
                    new { orderId = createdOrder.Order.Id, orderNumber = createdOrder.Order.OrderNumber });
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
