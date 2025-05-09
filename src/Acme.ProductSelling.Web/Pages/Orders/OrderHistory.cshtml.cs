using Acme.ProductSelling.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Acme.ProductSelling.Web.Pages.Orders
{
    [Authorize]
    public class OrderHistoryModel : AbpPageModel
    {
        private readonly IOrderAppService _orderAppService;

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string Sorting { get; set; }

        public int PageSize { get; set; } = 10; 

        public PagedResultDto<OrderDto> Orders { get; set; }

        public OrderHistoryModel(IOrderAppService orderAppService)
        {
            _orderAppService = orderAppService;
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
        }
    }
}
