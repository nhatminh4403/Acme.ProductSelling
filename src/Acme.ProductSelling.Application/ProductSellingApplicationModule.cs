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
        context.Services.AddSingleton<IHtmlSanitizer>(provider =>
            {

                var sanitizer = new HtmlSanitizer();

                // Configure allowed tags and attributes
                var allowedTags = new[]
                {
                    "h1", "h2", "h3", "h4", "h5", "h6", "p", "br", "strong", "em", "u",
                    "ul", "ol", "li", "blockquote", "a", "img", "code", "pre",
                    "table", "thead", "tbody", "tr", "th", "td", "div", "span",
                    "figure", "figcaption"
                };

                sanitizer.AllowedTags.Clear();
                foreach (var tag in allowedTags)
                {
                    sanitizer.AllowedTags.Add(tag);
                }

                var allowedAttributes = new[]
                {
                    "href", "src", "alt", "title", "class", "id", "target", "style",
                    "width", "height", "data-*"
                };

                sanitizer.AllowedAttributes.Clear();
                foreach (var attr in allowedAttributes)
                {
                    sanitizer.AllowedAttributes.Add(attr);
                }

                // Allow data attributes
                sanitizer.AllowDataAttributes = true;

                return sanitizer;
            }
        );

        var configuration = context.Services.GetConfiguration();

        //if (configuration.GetValue<bool>("Chatbot:AutoTrainOnStartup"))
        //{
        //    context.Services.AddHostedService<ChatbotModelTrainingService>();
        //}
        //context.Services.AddTransient<Volo.Abp.Account.IAccountAppService, Account.AccountAppService>();

    }

}
