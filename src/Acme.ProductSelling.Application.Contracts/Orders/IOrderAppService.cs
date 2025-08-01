using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace Acme.ProductSelling.Orders
{
    public interface IOrderAppService : IApplicationService
    {
        Task<CreateOrderResultDto> CreateAsync(CreateOrderDto input);

        // Giữ lại các phương thức cần thiết khác
        Task<OrderDto> GetAsync(Guid id);
        Task<PagedResultDto<OrderDto>> GetListAsync(PagedAndSortedResultRequestDto input);

        Task<OrderDto> GetByOrderNumberAsync(string orderNumber);
        Task<PagedResultDto<OrderDto>> GetListForCurrentUserAsync(PagedAndSortedResultRequestDto input);
        Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusDto input);
        Task DeleteAsync(Guid id);

        Task<OrderDto> ConfirmPayPalOrderAsync(Guid orderId);

    }
}
