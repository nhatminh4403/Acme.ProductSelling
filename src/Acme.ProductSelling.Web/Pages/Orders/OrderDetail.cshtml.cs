using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Orders
{
    [Authorize]
    public class OrderDetailModel : ProductSellingPageModel
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
        // ... trong class OrderDetailModel
        public async Task<IActionResult> OnPostCancelAsync(Guid orderId)
        {
            try
            {
                await _orderAppService.DeleteAsync(orderId);
                Alerts.Success(L["Order:OrderCancelledSuccessfully"]);

                // Tải lại dữ liệu trang sau khi hủy
                return RedirectToPage(new { id = orderId });
            }
            catch (UserFriendlyException ex)
            {
                Alerts.Warning(ex.Message);
                return RedirectToPage(new { id = orderId });
            }
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
        public string GetPaymentStatusBadgeClass(PaymentStatus paymentStatus)
        {
            return paymentStatus switch
            {
                PaymentStatus.Unpaid => "warning",
                PaymentStatus.Pending => "info",
                PaymentStatus.Paid => "success",
                PaymentStatus.Failed => "danger",
                PaymentStatus.Refunded => "secondary",
                PaymentStatus.Cancelled => "dark",
                _ => "secondary"
            };
        }
    }
}