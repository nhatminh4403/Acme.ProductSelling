using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Admin.Orders
{
    [Authorize(ProductSellingPermissions.Orders.Default)]
    public class DetailsModel : AdminPageModelBase
    {
        [BindProperty(SupportsGet = true)]
        public string OrderNumber { get; set; }
        //[BindProperty] 
        //public Guid Id { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Prefix { get; set; }

        private readonly IOrderQueryAppService _orderAppService;
        private readonly ILogger<DetailsModel> _logger;
        private readonly IOrderHistoryAppService _orderHistoryAppService;
        public List<OrderHistoryDto> OrderHistory { get; set; }

        public OrderDto Order { get; set; }
        public DetailsModel(IOrderQueryAppService orderAppService, ILogger<DetailsModel> logger, IOrderHistoryAppService orderHistoryAppService)
        {
            _orderAppService = orderAppService;
            _logger = logger;
            _orderHistoryAppService = orderHistoryAppService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                if (Prefix != RoleBasedPrefix)
                {
                    Response.Redirect($"/{RoleBasedPrefix}/orders/details/{OrderNumber}");
                }
                Order = await _orderAppService.GetByOrderNumberAsync(OrderNumber);

                if (Order == null)
                {
                    _logger.LogWarning("Order not found: {OrderNumber}", OrderNumber);

                    return NotFound();
                }

                OrderHistory = await _orderHistoryAppService.GetOrderHistoryAsync(Order.Id);
                return Page();
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "Error loading order detail for OrderNumber: {OrderNumber}", OrderNumber);
                return RedirectToPage(GetUrl("/orders"));
            }
        }
    }
}
