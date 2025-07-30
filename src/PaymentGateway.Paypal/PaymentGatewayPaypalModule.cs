using Acme.ProductSelling.Web;
using Acme.ProductSelling.EntityFrameworkCore;
using Acme.ProductSelling.DbMigrator;
using Acme.ProductSelling;
using Volo.Abp.Modularity;

namespace PaymentGateway.Paypal;

[DependsOn(
    typeof(ProductSellingWebModule),
    typeof(ProductSellingHttpApiClientModule),
    typeof(ProductSellingHttpApiModule),
    typeof(ProductSellingEntityFrameworkCoreModule),
    typeof(ProductSellingDomainSharedModule),
    typeof(ProductSellingDomainModule),
    typeof(ProductSellingDbMigratorModule),
    typeof(ProductSellingApplicationContractsModule),
    typeof(ProductSellingApplicationModule)
)]
public class PaymentGatewayPaypalModule : AbpModule
{

}