using Acme.ProductSelling.EntityFrameworkCore;
using Acme.ProductSelling.Products.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.ProductSelling.Products
{
    public class EFCoreRecentlyViewedProductRepository :
        EfCoreRepository<ProductSellingDbContext, RecentlyViewedProduct, Guid>,
        IRecentlyViewedProductRepository
    {
        public EFCoreRecentlyViewedProductRepository(
            IDbContextProvider<ProductSellingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
        public async Task<List<RecentlyViewedProduct>> GetByUserIdAsync(
        Guid userId,
        int maxCount = 10,
        bool includeProduct = false,
        CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();
            var query = dbSet
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.ViewedAt)
                .Take(maxCount);

            if (includeProduct)
            {
                query = query
                    .Include(x => x.Product)
                        .ThenInclude(p => p.StoreInventories);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<RecentlyViewedProduct?> FindByUserAndProductAsync(
            Guid userId,
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(
                x => x.UserId == userId && x.ProductId == productId,
                cancellationToken);
        }

        public async Task DeleteOldestForUserAsync(
            Guid userId,
            int keepCount = 50,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var idsToDelete = await dbSet
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.ViewedAt)
                .Skip(keepCount)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            if (idsToDelete.Any())
            {
                await dbSet
                    .Where(x => idsToDelete.Contains(x.Id))
                    .ExecuteDeleteAsync(cancellationToken);
            }
        }

        public async Task DeleteOlderThanAsync(
            DateTime cutoffDate,
            CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();
            await dbSet
                .Where(x => x.ViewedAt < cutoffDate)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}