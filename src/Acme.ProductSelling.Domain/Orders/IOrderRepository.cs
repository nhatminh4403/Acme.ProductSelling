using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Orders
{
    public interface IOrderRepository : IRepository<Order, Guid>
    {
        Task<List<Order>> GetListByStoreAsync(Guid storeId, OrderStatus? status = null);
        Task<List<Order>> GetListByStatusAsync(OrderStatus status);
        Task<Order> GetWithDetailsAsync(Guid id);
        Task<List<Order>> GetInStoreOrdersAsync(Guid storeId, OrderStatus? status = null);
    }
}
