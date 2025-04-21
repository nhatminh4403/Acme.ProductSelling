using Volo.Abp.Modularity;

namespace Acme.ProductSelling;

public abstract class ProductSellingApplicationTestBase<TStartupModule> : ProductSellingTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
