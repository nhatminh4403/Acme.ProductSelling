using Acme.ProductSelling.EntityFrameworkCore;
using Acme.ProductSelling.PaymentGateway.MoMo;
using Acme.ProductSelling.PaymentGateway.PayPal;
using Acme.ProductSelling.PaymentGateway.VnPay;
using Ganss.Xss;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
namespace Acme.ProductSelling;
[DependsOn(
    typeof(AcmeProductSellingPaymentGatewayVnPayModule),
    typeof(AcmeProductSellingPaymentGatewayPayPalModule),
    typeof(AcmeProductSellingPaymentGatewayMoMoModule),
    typeof(ProductSellingEntityFrameworkCoreModule),
    typeof(ProductSellingDomainModule),
    typeof(ProductSellingApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class ProductSellingApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ProductSellingApplicationModule>();
        });
        context.Services.AddSingleton<IHtmlSanitizer>(new HtmlSanitizer());
    }
}
