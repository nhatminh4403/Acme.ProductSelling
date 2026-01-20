using Acme.ProductSelling.Orders.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Orders.Services
{
    public interface IOrderPublicAppService : IApplicationService
    {
        Task<CreateOrderResultDto> CreateAsync(CreateOrderDto input);
        Task<OrderDto> ConfirmPayPalOrderAsync(Guid guid);
        Task DeleteAsync(Guid id);
        Task<OrderDto> GetAsync(Guid id); 
        Task<PagedResultDto<OrderDto>> GetListForCurrentUserAsync(PagedAndSortedResultRequestDto input);
        Task<OrderDto> GetByOrderNumberAsync(string orderNumber);

    }
}
