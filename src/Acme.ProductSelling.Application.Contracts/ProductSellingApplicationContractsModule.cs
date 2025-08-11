using Acme.ProductSelling.PaymentGateway.MoMo;
using Acme.ProductSelling.PaymentGateway.PayPal;
using Acme.ProductSelling.PaymentGateway.VnPay;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace Acme.ProductSelling;

[DependsOn(
    typeof(AcmeProductSellingPaymentGatewayVnPayModule),
    typeof(ProductSellingDomainModule),
    typeof(ProductSellingDomainSharedModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(AbpIdentityApplicationContractsModule),
    typeof(AbpAccountApplicationContractsModule),
    typeof(AbpTenantManagementApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(AcmeProductSellingPaymentGatewayMoMoModule),
    typeof(AcmeProductSellingPaymentGatewayPayPalModule),
    typeof(AbpAspNetCoreSignalRModule)
)]

public class ProductSellingApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ProductSellingDtoExtensions.Configure();
    }
}
