using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Checkout
{
    public class OrderConfirmationModel : AbpPageModel
    {
        public OrderDto Order { get; private set; }

        private readonly IOrderAppService _orderAppService;
        public OrderConfirmationModel(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
        }

        public async Task<IActionResult> OnGetAsync(Guid orderId, string orderNumber)
        {
            if (orderId == Guid.Empty && string.IsNullOrEmpty(orderNumber))
            {
                return NotFound();
            }

            try
            {
                if (orderId != Guid.Empty)
                {
                    Order = await _orderAppService.GetAsync(orderId);
                }
                else
                {
                    Order = await _orderAppService.GetByOrderNumberAsync(orderNumber);
                }

                return Page();
            }
            catch (UserFriendlyException ex)
            {
                Alerts.Warning(ex.Message);
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving order.");
                Alerts.Danger("An error occurred while retrieving your order.");
                return RedirectToPage("/Index");
            }
        }

    }
}
