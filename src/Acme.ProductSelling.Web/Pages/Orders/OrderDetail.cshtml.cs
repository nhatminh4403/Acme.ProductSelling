using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IOrderPublicAppService _orderPublicAppService;
        private readonly IOrderHistoryAppService _orderHistoryAppService;
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }
        public List<OrderHistoryDto> OrderHistory { get; set; }

        public OrderDto Order { get; set; }

        public OrderDetailModel(IOrderAppService orderAppService, IOrderPublicAppService orderPublicAppService, IOrderHistoryAppService orderHistoryAppService)
        {
            _orderAppService = orderAppService;
            _orderPublicAppService = orderPublicAppService;
            _orderHistoryAppService = orderHistoryAppService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id.ToString()))
            {
                return RedirectToPage("/Orders/OrderHistory");
            }

            //Order = await _orderAppService.GetAsync(Id);
            Order = await _orderPublicAppService.GetAsync(Id);
            if (Order.CustomerId != CurrentUser.Id)
                return Forbid();

            if (Order == null)
            {
                return RedirectToPage("/Orders/OrderHistory");
            }
            //OrderHistory = await _orderAppService.GetOrderHistoryAsync(Id);
            OrderHistory = await _orderHistoryAppService.GetOrderHistoryAsync(Id);

            return Page();
        }
        public async Task<IActionResult> OnPostCancelOrderAjaxAsync([FromBody] CancelOrderRequest request)
        {
            try
            {
                Logger.LogInformation("[CancelOrderAjax] START - OrderId: {OrderId}, UserId: {UserId}",
                    request.OrderId, CurrentUser.Id);

                //await _orderAppService.DeleteAsync(request.OrderId);
                await _orderPublicAppService.DeleteAsync(request.OrderId);
                Logger.LogInformation("[CancelOrderAjax] SUCCESS - OrderId: {OrderId}", request.OrderId);

                return new JsonResult(new
                {
                    success = true,
                    message = L["Order:OrderCancelledSuccessfully"].Value
                });
            }
            catch (UserFriendlyException ex)
            {
                Logger.LogWarning(ex, "[CancelOrderAjax] UserFriendlyException - OrderId: {OrderId}, Message: {Message}",
                    request.OrderId, ex.Message);

                return new JsonResult(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "[CancelOrderAjax] ERROR - OrderId: {OrderId}", request.OrderId);

                return new JsonResult(new
                {
                    success = false,
                    message = L["Error:GeneralError"].Value
                });
            }
        }
        public class CancelOrderRequest
        {
            public Guid OrderId { get; set; }
        }
        public async Task<IActionResult> OnPostCancelAsync(Guid orderId)
        {
            try
            {
                //await _orderAppService.DeleteAsync(orderId);
                await _orderPublicAppService.DeleteAsync(orderId);
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
                "Placed" => "bg-primary",
                "Pending" => "bg-warning",
                "Confirmed" => "bg-info",
                "Processing" => "bg-info",
                "Shipped" => "bg-success",
                "Delivered" => "bg-success",
                "Cancelled" => "bg-danger ",
                _ => "bg-secondary"
            };
        }
        public string GetOrderStatusBadgeClass(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Placed => "bg-primary",
                OrderStatus.Pending => "bg-warning",
                OrderStatus.Confirmed => "bg-info",
                OrderStatus.Processing => "bg-info",
                OrderStatus.Shipped => "bg-primary",
                OrderStatus.Delivered => "bg-success",
                OrderStatus.Cancelled => "bg-danger",
                _ => "bg-secondary"
            };
        }
        public string GetPaymentStatusBadgeClass(PaymentStatus paymentStatus)
        {
            return paymentStatus switch
            {
                PaymentStatus.Unpaid => "bg-warning",
                PaymentStatus.Pending => "bg-info",
                PaymentStatus.PendingOnDelivery => "bg-info",
                PaymentStatus.Paid => "bg-success",
                PaymentStatus.Failed => "bg-danger",
                PaymentStatus.Refunded => "bg-secondary",
                PaymentStatus.Cancelled => "bg-danger",
                _ => "secondary"
            };
        }
        public string GetPaymentStatusBadgeClass(string paymentStatus)
        {
            return paymentStatus switch
            {
                "Unpaid" => "bg-warning",
                "PendingOnDelivery" => "bg-info",
                "Pending" => "bg-warning",
                "Paid" => "bg-success",
                "Failed" => "bg-danger",
                "Refunded" => "bg-dark",
                "Cancelled" => "bg-secondary",
                _ => "bg-dark"
            };
        }
    }
}