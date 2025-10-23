using Acme.ProductSelling.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Acme.ProductSelling.DbMigrator;

[DependsOn(
    typeof(ProductSellingDomainSharedModule),
    typeof(ProductSellingDomainModule),
    typeof(ProductSellingApplicationModule),
    typeof(AbpAutofacModule),
    typeof(ProductSellingEntityFrameworkCoreModule),
    typeof(ProductSellingApplicationContractsModule)
)]
public class ProductSellingDbMigratorModule : AbpModule
{
}
