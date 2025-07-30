using Acme.ProductSelling.EntityFrameworkCore;
using Acme.ProductSelling;
using Volo.Abp.Modularity;

namespace Acme.ProductSelling.PaymentGateway.MoMo;

[DependsOn(
    typeof(ProductSellingEntityFrameworkCoreModule),
    typeof(ProductSellingDomainSharedModule),
    typeof(ProductSellingDomainModule),
    typeof(ProductSellingApplicationContractsModule)
)]
public class AcmeProductSellingPaymentGatewayMoMoModule : AbpModule
{

}