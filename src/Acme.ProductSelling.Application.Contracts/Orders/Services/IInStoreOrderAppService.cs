using Acme.ProductSelling.Orders.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Orders.Services
{
    public interface IInStoreOrderAppService : IApplicationService
    {
        Task<PagedResultDto<OrderDto>> GetListAsync();
        Task<OrderDto> CreateInStoreOrderAsync(CreateInStoreOrderDto input);
        Task<OrderDto> FulfillInStoreOrderAsync(Guid orderId);
    }
}
