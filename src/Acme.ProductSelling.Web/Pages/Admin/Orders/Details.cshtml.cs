using Acme.ProductSelling.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Pages.Admin.Orders
{
    public class DetailsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string OrderNumber { get; set; }
        
        private readonly IOrderAppService _orderAppService;
        private readonly ILogger<DetailsModel> _logger;

        public OrderDto Order { get; set; }
        public DetailsModel(IOrderAppService orderAppService, ILogger<DetailsModel> logger)
        {
            _orderAppService = orderAppService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Order = await _orderAppService.GetByOrderNumberAsync(OrderNumber);

                if (Order == null)
                {
                    return NotFound();
                }

                return Page();
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "Error loading order detail for OrderNumber: {OrderNumber}", OrderNumber);
                return RedirectToPage("/Admin/Orders/Index");
            }
        }
    }
}
