using Acme.ProductSelling.EntityFrameworkCore;
using Acme.ProductSelling.Orders.Services;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.ProductSelling.Orders
{
    public class EfCoreOnlineOrderRepository : EfCoreRepository<ProductSellingDbContext, OnlineOrder, Guid>, IOnlineOrderRepository
    {
        public EfCoreOnlineOrderRepository(IDbContextProvider<ProductSellingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
