using Acme.ProductSelling.Orders;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [BindProperty(SupportsGet = true)]
        public string Sorting { get; set; }

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

        public async Task<IActionResult> OnPostCancelOrderAsync(Guid orderId)
        {
            try
            {
                await _orderAppService.DeleteAsync(orderId);
                Alerts.Success(L["Order:OrderCancelledSuccessfully"]);
            }
            catch (UserFriendlyException ex)
            {
                Alerts.Warning(ex.Message);
            }
            return RedirectToPage(); // Tải lại trang lịch sử
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
