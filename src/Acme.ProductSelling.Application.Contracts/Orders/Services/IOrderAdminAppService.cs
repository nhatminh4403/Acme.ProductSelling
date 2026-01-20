using Acme.ProductSelling.Orders.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Orders.Services
{
    public interface IOrderAdminAppService : IApplicationService
    {
        Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListInput input);
        Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusDto input);
        Task ShipOrderAsync(Guid orderId);
        Task DeliverOrderAsync(Guid orderId);
        Task MarkAsCodPaidAndCompletedAsync(Guid orderId);
        Task RestoreOrderAsync(Guid orderId); 
        Task<PagedResultDto<OrderDto>> GetProfitReportAsync(PagedAndSortedResultRequestDto input);
        Task<PagedResultDto<OrderDto>> GetDeletedOrdersAsync(PagedAndSortedResultRequestDto input);
    }
}
