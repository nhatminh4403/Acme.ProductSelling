using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Acme.ProductSelling
{
    public static class RepositoryUpsertExtensions
    {
        public static async Task<TEntity> UpsertAsync<TEntity, TKey>(
            this IRepository<TEntity, TKey> repository,
            TEntity entity,
            bool autoSave = false,
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity<TKey>
        {
            var existingEntity = await repository.FindAsync(entity.Id, cancellationToken: cancellationToken);

            if (existingEntity == null)
            {
                return await repository.InsertAsync(entity, autoSave, cancellationToken);
            }
            else
            {
                // Update existing entity properties
                await repository.UpdateAsync(entity, autoSave, cancellationToken);
                return entity;
            }
        }
    }
}
