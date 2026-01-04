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
    public class EFCoreRecentlyViewedProductRepository : EfCoreRepository<ProductSellingDbContext, RecentlyViewedProduct, Guid>, IRecentlyViewedProductRepository
    {
        public EFCoreRecentlyViewedProductRepository(IDbContextProvider<ProductSellingDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
        public async Task<List<RecentlyViewedProduct>> GetByUserIdAsync(Guid userId, int maxCount = 10, bool includeProduct = false, CancellationToken cancellationToken = default)
        {
            maxCount = Math.Min(Math.Max(maxCount, 1), 100);


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

        public async Task<RecentlyViewedProduct?> FindByUserAndProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId, cancellationToken);
        }

        public async Task DeleteOldestForUserAsync(Guid userId,
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
                const int batchSize = 500;

                for (int i = 0; i < idsToDelete.Count; i += batchSize)
                {
                    var batch = idsToDelete.Skip(i).Take(batchSize).ToList();

                    await dbSet
                        .Where(x => batch.Contains(x.Id))
                        .ExecuteDeleteAsync(cancellationToken);
                }
            }
        }

        public async Task DeleteOlderThanAsync(DateTime cutoffDate, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            // OPTIMIZATION 7: Add safety check
            if (cutoffDate > DateTime.UtcNow)
            {
                throw new ArgumentException("Cutoff date cannot be in the future", nameof(cutoffDate));
            }

            // OPTIMIZATION 8: For large deletes, consider batching
            // Count first to decide strategy
            var count = await dbSet
                .Where(x => x.ViewedAt < cutoffDate)
                .CountAsync(cancellationToken);

            if (count == 0)
            {
                return; // Nothing to delete
            }

            // For very large deletes (>10000), use batching
            if (count > 10000)
            {
                const int batchSize = 1000;

                while (true)
                {
                    var deleted = await dbSet
                        .Where(x => x.ViewedAt < cutoffDate)
                        .Take(batchSize)
                        .ExecuteDeleteAsync(cancellationToken);

                    if (deleted < batchSize)
                    {
                        break; // No more records to delete
                    }
                }
            }
            else
            {
                // Single delete for smaller datasets
                await dbSet
                    .Where(x => x.ViewedAt < cutoffDate)
                    .ExecuteDeleteAsync(cancellationToken);
            }
        }

        public async Task InsertManyOptimizedAsync(List<RecentlyViewedProduct> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null || !entities.Any())
            {
                return;
            }

            // Limit batch size for Azure SQL
            const int batchSize = 100;

            for (int i = 0; i < entities.Count; i += batchSize)
            {
                var batch = entities.Skip(i).Take(batchSize).ToList();
                await InsertManyAsync(batch, cancellationToken: cancellationToken);
            }
        }
    }
}