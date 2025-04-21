using Volo.Abp.Modularity;

namespace Acme.ProductSelling;

[DependsOn(
    typeof(ProductSellingApplicationModule),
    typeof(ProductSellingDomainTestModule)
)]
public class ProductSellingApplicationTestModule : AbpModule
{

}
