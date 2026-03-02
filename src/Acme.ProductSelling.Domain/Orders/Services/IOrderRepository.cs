using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Orders.Services
{
    public interface IOrderRepository : IRepository<Order, Guid>
    {
        Task<List<Order>> GetListByStatusAsync(OrderStatus status);
        Task<Order> GetWithDetailsAsync(Guid id);
    }
}
