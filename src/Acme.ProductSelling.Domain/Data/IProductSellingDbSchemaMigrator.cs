using System.Threading.Tasks;

namespace Acme.ProductSelling.Data;

public interface IProductSellingDbSchemaMigrator
{
    Task MigrateAsync();
}
