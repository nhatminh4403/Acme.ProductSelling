using Acme.ProductSelling.Orders.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.ProductSelling.Orders.Services
{
    public interface IInStoreOrderAppService : IApplicationService
    {
        Task<OrderDto> CreateInStoreOrderAsync(CreateInStoreOrderDto input);
        Task<OrderDto> FulfillInStoreOrderAsync(Guid orderId);
    }
}
