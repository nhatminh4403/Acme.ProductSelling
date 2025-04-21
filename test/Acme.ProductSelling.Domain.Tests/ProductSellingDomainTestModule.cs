using Volo.Abp.Modularity;

namespace Acme.ProductSelling;

[DependsOn(
    typeof(ProductSellingDomainModule),
    typeof(ProductSellingTestBaseModule)
)]
public class ProductSellingDomainTestModule : AbpModule
{

}
