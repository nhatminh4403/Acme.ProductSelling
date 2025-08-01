using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Acme.ProductSelling.PaymentGateway.VnPay;

public class AcmeProductSellingPaymentGatewayVnPayModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.Configure<VnPayOptions>(configuration.GetSection("VnPay"));
    }
}