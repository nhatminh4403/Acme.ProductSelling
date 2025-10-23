using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Web.Pages.Admin.Orders
{
    public class DeletedOrderModel : ProductSellingPageModel
    {
        private readonly IOrderAppService _orderAppService;
        public PagedResultDto<OrderDto> DeletedOrders { get; private set; }

        [BindProperty(SupportsGet = true)]
        public PagedAndSortedResultRequestDto Input { get; set; } = new PagedAndSortedResultRequestDto();
        public DeletedOrderModel(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
        }
        public void OnGet()
        {
            var deletedOrders = _orderAppService.GetDeletedOrdersAsync(Input);
            DeletedOrders = deletedOrders.Result;
        }
    }
}
