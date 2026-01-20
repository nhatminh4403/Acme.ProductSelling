using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Acme.ProductSelling.PaymentGateway.PayPal;


public class AcmeProductSellingPaymentGatewayPayPalModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Configure<PayPalOption>(context.Services.GetConfiguration().GetSection("PayPal"));
    }

}