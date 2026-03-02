using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Orders.Services
{
    public interface IInStoreOrderRepository : IRepository<InStoreOrder, Guid>
    {
        Task<List<InStoreOrder>> GetInStoreOrdersAsync(Guid storeId, OrderStatus? status = null);
    }
}
