using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace Acme.ProductSelling.Orders
{
    public interface IOrderAppService : ICrudAppService<
     OrderDto,
     Guid,
     PagedAndSortedResultRequestDto,
     CreateOrderDto>
    { // Không cần kế thừa ICrudAppService nữa
        //Task<OrderDto> CreateAsync(CreateOrderDto input);
        //Task<OrderDto> GetAsync(Guid id);
        //Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListInput input);
        Task<OrderDto> GetByOrderNumberAsync(string orderNumber);
        Task<PagedResultDto<OrderDto>> GetListForCurrentUserAsync(PagedAndSortedResultRequestDto input);
        Task ChangeOrderStatus(Guid orderId, OrderStatus newStatus);
    }
}
