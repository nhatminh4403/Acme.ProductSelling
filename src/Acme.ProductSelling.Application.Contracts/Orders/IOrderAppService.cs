using Acme.ProductSelling.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace Acme.ProductSelling.Orders
{
    public interface IOrderAppService : IApplicationService
    { // Không cần kế thừa ICrudAppService nữa
        Task<OrderDto> CreateAsync(CreateOrderDto input);
        Task<OrderDto> GetAsync(Guid id);
        //Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListInput input);
        Task<OrderDto> GetByOrderNumberAsync(string orderNumber);
        Task<PagedResultDto<OrderDto>> GetListForCurrentUserAsync(PagedAndSortedResultRequestDto input);

    }
}
