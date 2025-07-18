using Acme.ProductSelling.VNPay;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Acme.ProductSelling.PaymentGateway;

public class AcmeProductSellingPaymentGatewayModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.Configure<VnPayOptions>(configuration.GetSection("VnPay"));
    }
}