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
    {
        Task<OrderDto> GetByOrderNumberAsync(string orderNumber);
        Task<PagedResultDto<OrderDto>> GetListForCurrentUserAsync(PagedAndSortedResultRequestDto input);
        Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusDto input);
    }
}
