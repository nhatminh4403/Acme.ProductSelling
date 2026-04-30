using Acme.ProductSelling.Orders.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Orders.Services
{
    public interface IOrderPublicAppService : IApplicationService
    {
        Task<CreateOrderResultDto> CreateAsync(CreateOrderDto input);
        Task<OrderDto> ConfirmPayPalOrderAsync(Guid guid, string token);
        Task DeleteAsync(Guid id);
        Task<OrderDto> GetAsync(Guid id);
        Task<OrderDto> GetByOrderNumberAsync(string orderNumber);
        Task<PagedResultDto<OrderDto>> GetListForCurrentUserAsync(PagedAndSortedResultRequestDto input);
        Task<OrderDto> ConfirmMoMoOrderAsync(Guid id, long transId);

    }
}
