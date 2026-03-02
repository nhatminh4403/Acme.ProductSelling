using System;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Orders.Services
{
    public interface IOnlineOrderRepository : IRepository<OnlineOrder, Guid>
    {
    }
}
