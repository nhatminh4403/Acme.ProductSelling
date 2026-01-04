using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling.Products.Services
{
    public interface IRecentlyViewedProductRepository : IRepository<RecentlyViewedProduct, Guid>
    {
        Task<List<RecentlyViewedProduct>> GetByUserIdAsync(Guid userId, int maxCount = 10, bool includeProduct = false, CancellationToken cancellationToken = default);

        Task<RecentlyViewedProduct?> FindByUserAndProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);

        Task DeleteOldestForUserAsync(Guid userId, int keepCount = 50, CancellationToken cancellationToken = default);

        Task DeleteOlderThanAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);
        Task InsertManyOptimizedAsync(List<RecentlyViewedProduct> entities, CancellationToken cancellationToken = default);
    }
}
