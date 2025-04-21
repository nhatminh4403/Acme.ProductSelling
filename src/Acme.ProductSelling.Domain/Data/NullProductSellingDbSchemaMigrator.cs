using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Data;

/* This is used if database provider does't define
 * IProductSellingDbSchemaMigrator implementation.
 */
public class NullProductSellingDbSchemaMigrator : IProductSellingDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
