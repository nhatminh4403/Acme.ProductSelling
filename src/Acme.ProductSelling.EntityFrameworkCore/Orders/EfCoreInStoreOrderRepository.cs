using Acme.ProductSelling.EntityFrameworkCore;
using Acme.ProductSelling.Orders.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.ProductSelling.Orders
{
    public class EfCoreInStoreOrderRepository : EfCoreRepository<ProductSellingDbContext, InStoreOrder, Guid>,
                                                IInStoreOrderRepository
    {
        public EfCoreInStoreOrderRepository(IDbContextProvider<ProductSellingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
            {
            }

        }

        public async Task<List<InStoreOrder>> GetInStoreOrdersAsync(Guid storeId, OrderStatus? status = null)
        {
            var dbSet = await GetDbSetAsync();
            var query = dbSet.Where(x => x.StoreId == storeId && x.OrderType == OrderType.InStore);

            if (status.HasValue)
            {
                query = query.Where(x => x.OrderStatus == status.Value);
            }

            return await query
                .Include(o => o.OrderItems)
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync();
        }
    }
}
