using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;

namespace Acme.ProductSelling.Web.Pages.Orders
{
    [Authorize]
    public class OrderHistoryModel : ProductSellingPageModel
    {
        private readonly IOrderAppService _orderAppService;

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        //[BindProperty(SupportsGet = true)]
        public string Sorting = "OrderDate DESC";

        public int PageSize { get; set; } = 10;
        public PagerModel PagerModel { get; set; }

        public PagedResultDto<OrderDto> Orders { get; set; }

        public OrderHistoryModel(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
            Orders = new PagedResultDto<OrderDto>();
        }

        public async Task OnGetAsync()
        {
            var input = new PagedAndSortedResultRequestDto
            {
                SkipCount = (CurrentPage - 1) * PageSize,
                MaxResultCount = PageSize,
                Sorting = Sorting
            };
            Orders = await _orderAppService.GetListForCurrentUserAsync(input);

            PagerModel = new PagerModel(Orders.TotalCount, 3, CurrentPage, PageSize, "/");
        }
        public async Task<IActionResult> OnPostCancelOrderAjaxAsync([FromBody] CancelOrderRequest request)
        {
            try
            {
                Logger.LogInformation("[CancelOrderAjax] START - OrderId: {OrderId}, UserId: {UserId}",
                    request.OrderId, CurrentUser.Id);

                await _orderAppService.DeleteAsync(request.OrderId);

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
