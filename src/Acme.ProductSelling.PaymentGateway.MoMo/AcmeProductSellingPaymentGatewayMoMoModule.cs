using Acme.ProductSelling.EntityFrameworkCore;
using Acme.ProductSelling;
using Volo.Abp.Modularity;
using Acme.ProductSelling.PaymentGateway.MoMo.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.ProductSelling.PaymentGateway.MoMo;

[DependsOn(
    typeof(ProductSellingEntityFrameworkCoreModule),
    typeof(ProductSellingDomainSharedModule),
    typeof(ProductSellingDomainModule)
)]
public class AcmeProductSellingPaymentGatewayMoMoModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.Configure<MoMoOption>(configuration.GetSection("MoMo"));
    }

}