using Acme.ProductSelling.EntityFrameworkCore;
using Acme.ProductSelling;
using Volo.Abp.Modularity;

namespace Acme.ProductSelling.PaymentGateway.PayPal;

[DependsOn(
    typeof(ProductSellingEntityFrameworkCoreModule),
    typeof(ProductSellingApplicationContractsModule)
)]
public class AcmeProductSellingPaymentGatewayPayPalModule : AbpModule
{

}