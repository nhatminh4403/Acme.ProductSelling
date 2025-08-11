using Acme.ProductSelling.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Acme.ProductSelling.PaymentGateway.PayPal;

[DependsOn(
    typeof(ProductSellingEntityFrameworkCoreModule)
)]
public class AcmeProductSellingPaymentGatewayPayPalModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Configure<PayPalOption>(context.Services.GetConfiguration().GetSection("PayPal"));
    }

}