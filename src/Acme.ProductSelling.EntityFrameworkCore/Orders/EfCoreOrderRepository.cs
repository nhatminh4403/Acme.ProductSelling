using Acme.ProductSelling.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.ProductSelling.Orders
{
    public class EfCoreOrderRepository : EfCoreRepository<ProductSellingDbContext, Order, Guid>, IOrderRepository
    {
        public EfCoreOrderRepository(IDbContextProvider<ProductSellingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Order>> GetListByStoreAsync(Guid storeId, OrderStatus? status = null)
        {
            var dbSet = await GetDbSetAsync();
            var query = dbSet.Where(x => x.StoreId == storeId);

            if (status.HasValue)
            {
                query = query.Where(x => x.OrderStatus == status.Value);
            }

            return await query
                .Include(o => o.OrderItems)
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync();
        }

        public async Task<List<Order>> GetListByStatusAsync(OrderStatus status)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Where(x => x.OrderStatus == status)
                .Include(o => o.OrderItems)
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync();
        }

        public async Task<Order> GetWithDetailsAsync(Guid id)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Include(x => x.OrderItems)
                .Include(x => x.OrderHistories)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Order>> GetInStoreOrdersAsync(Guid storeId, OrderStatus? status = null)
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
