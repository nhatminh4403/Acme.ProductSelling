using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;

namespace Acme.ProductSelling.Web.Pages.Orders
{
    [Authorize]
    public class OrderDetailModel : ProductSellingPageModel
    {
        private readonly IOrderAppService _orderAppService;

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }
        public List<OrderHistoryDto> OrderHistory { get; set; }

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
            if (Order.CustomerId != CurrentUser.Id)
                return Forbid();

            if (Order == null)
            {
                return RedirectToPage("/Orders/OrderHistory");
            }
            OrderHistory = await _orderAppService.GetOrderHistoryAsync(Id);

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
        public string GetOrderStatusBadgeClass(string orderStatus)
        {
            return orderStatus switch
            {
                "Placed" => "bg-info text-white",
                "Pending" => "bg-light text-dark",
                "Confirmed" => "bg-primary text-white",
                "Processing" => "bg-warning text-dark",
                "Shipped" => "bg-success text-white",
                "Delivered" => "bg-dark text-white",
                "Cancelled" => "bg-danger text-white",
                _ => "bg-secondary text-white"
            };
        }
        public string GetPaymentStatusBadgeClass(string paymentStatus)
        {
            return paymentStatus switch
            {
                "Unpaid" => "bg-secondary text-white",
                "PendingOnDelivery" => "bg-info text-white",
                "Pending" => "bg-warning text-dark",
                "Paid" => "bg-success text-white",
                "Failed" => "bg-danger text-white",
                "Refunded" => "bg-dark text-white",
                "Cancelled" => "bg-secondary text-white",
                _ => "bg-dark text-white"
            };
        }
    }
}