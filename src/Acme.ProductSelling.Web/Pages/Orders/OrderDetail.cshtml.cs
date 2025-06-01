using Acme.ProductSelling.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Orders
{
    [Authorize]
    public class OrderDetailModel : AbpPageModel
    {
        private readonly IOrderAppService _orderAppService;

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public OrderDto Order { get; set; }

        public OrderDetailModel(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id.ToString()))
            {
                return RedirectToPage("/Orders/OrderHistory");
            }

            Order = await _orderAppService.GetAsync(Id);

            if (Order == null)
            {
                return RedirectToPage("/Orders/OrderHistory");
            }

            return Page();
        }

        public string GetStatusBadgeClass(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Placed => "primary",
                OrderStatus.Pending => "warning",
                OrderStatus.Confirmed => "info",
                OrderStatus.Processing => "info",
                OrderStatus.Shipped => "primary",
                OrderStatus.Delivered => "success",
                OrderStatus.Cancelled => "danger",
                _ => "secondary"
            };
        }
    }
} 