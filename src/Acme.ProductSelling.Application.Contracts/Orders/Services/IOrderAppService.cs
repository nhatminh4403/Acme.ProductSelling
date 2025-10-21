using Acme.ProductSelling.Orders.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
namespace Acme.ProductSelling.Orders.Services
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
        Task MarkAsCodPaidAndCompletedAsync(Guid orderId);
        Task ShipOrderAsync(Guid orderId);
        Task DeliverOrderAsync(Guid orderId);
        Task<List<OrderHistoryDto>> GetOrderHistoryAsync(Guid orderId);
        Task<PagedResultDto<OrderDto>> GetProfitReportAsync(PagedAndSortedResultRequestDto input);
    }
}
