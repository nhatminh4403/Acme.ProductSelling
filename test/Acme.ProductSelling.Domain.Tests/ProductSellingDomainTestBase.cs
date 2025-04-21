using Volo.Abp.Modularity;

namespace Acme.ProductSelling;

/* Inherit from this class for your domain layer tests. */
public abstract class ProductSellingDomainTestBase<TStartupModule> : ProductSellingTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
