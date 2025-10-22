#region using directives
using Acme.ProductSelling.EntityFrameworkCore;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.MultiTenancy;
using Acme.ProductSelling.Orders.BackgroundJobs.OrderCleanup;
using Acme.ProductSelling.Orders.Hubs;
using Acme.ProductSelling.PaymentGateway.MoMo;
using Acme.ProductSelling.PaymentGateway.PayPal;
using Acme.ProductSelling.PaymentGateway.VnPay;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Web.Filters;
using Acme.ProductSelling.Web.HealthChecks;
using Acme.ProductSelling.Web.Menus;
using Acme.ProductSelling.Web.Middleware;
using Acme.ProductSelling.Web.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
//using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.DatatablesNet;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.JQuery;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.JQueryValidation;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity.Web;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Security.Claims;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.Ui.LayoutHooks;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.Hangfire;
using Volo.Abp.BackgroundJobs.Hangfire;
using Hangfire;
#endregion

namespace Acme.ProductSelling.Web;

#region Dependencies
[DependsOn(
    typeof(AcmeProductSellingPaymentGatewayVnPayModule),
    typeof(AcmeProductSellingPaymentGatewayPayPalModule),
    typeof(AcmeProductSellingPaymentGatewayMoMoModule),


    typeof(ProductSellingHttpApiClientModule),
    typeof(ProductSellingHttpApiModule),
    typeof(ProductSellingApplicationModule),
    typeof(ProductSellingEntityFrameworkCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpTenantManagementWebModule),
    typeof(AbpFeatureManagementWebModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpAspNetCoreSignalRModule),
    typeof(ProductSellingHttpApiClientModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAspNetCoreMvcUiModule),
    typeof(AbpBackgroundJobOptions),
    typeof(AbpBackgroundJobsHangfireModule)

)]

#endregion
public class ProductSellingWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(ProductSellingResource),
                typeof(ProductSellingDomainModule).Assembly,
                typeof(ProductSellingDomainSharedModule).Assembly,
                typeof(ProductSellingApplicationModule).Assembly,
                typeof(ProductSellingApplicationContractsModule).Assembly,
                typeof(ProductSellingWebModule).Assembly
            );
        });

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("ProductSelling");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                serverBuilder.AddProductionEncryptionAndSigningCertificate("openiddict.pfx", configuration["AuthServer:CertificatePassPhrase"]!);
                serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
            });
        }
        
    }



    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();
        var services = context.Services;
        if (!configuration.GetValue<bool>("App:DisablePII"))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.LogCompleteSecurityArtifact = true;
        }

        if (!configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata"))
        {
            Configure<OpenIddictServerAspNetCoreOptions>(options =>
            {
                options.DisableTransportSecurityRequirement = true;
            });

            Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
            });
        }

        ConfigureBundles();
        ConfigureUrls(configuration);
        ConfigureHealthChecks(context);
        ConfigureAuthentication(context);
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureNavigationServices();
        ConfigureAutoApiControllers();
        ConfigureSwaggerServices(context.Services);

        Configure<PermissionManagementOptions>(options =>
        {
            options.IsDynamicPermissionStoreEnabled = true;
        });
        Configure<MvcOptions>(options =>
        {
            options.Filters.AddService<AuthenticatedRedirectFilter>();
        });
        context.Services.AddSignalR();
        context.Services.AddTransient<ISpecificationService, SpecificationService>();

        Configure<AbpSignalROptions>(options =>
        {
            options.Hubs.AddOrUpdate<OrderHub>();
        });


        Configure<RazorPagesOptions>(options =>
        {
            var supportedCultures = new[] { "en", "vi" };
            var defaultCulture = supportedCultures.Contains("vi") ? "vi" : supportedCultures.First(c => c == "en");
            options.Conventions.Add(new CultureRouteModelConvention(
                supportedCultures, defaultCulture
            ));
        });

        ConfigureRequestLocalization(services);
        context.Services.AddTransient<CultureAwareAnchorTagHelper>();

        ConfigureAdminPages(services);
        ConfigureRouting(services);
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = true;
        });
    }
    private void ConfigureAdminPages(IServiceCollection services)
    {
        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AddPageRoute("/Identity/Users/Index", "/admin/Identity/Users");
            options.Conventions.AddPageRoute("/Identity/Roles/Index", "/admin/Identity/Roles");
            options.Conventions.AddPageRoute("/TenantManagement/Tenants/Index", "/admin/TenantManagement/Tenants");
            options.Conventions.AddPageRoute("/SettingManagement/Index", "/admin/SettingManagement");
        });

        Configure<RazorPagesOptions>(options =>
        {

            options.Conventions.AddPageApplicationModelConvention("/Identity", model =>
            {
                // Fix: Use model.Properties to set layout for the folder's pages
                model.Properties["Layout"] = "/Pages/Admin/Shared/_AdminLayout.cshtml";
            });

            // Setting Management pages
            options.Conventions.AddPageApplicationModelConvention("/SettingManagement", model =>
            {
                model.Properties["Layout"] = "/Pages/Admin/Shared/_AdminLayout.cshtml";
            });

            // Tenant Management pages
            options.Conventions.AddPageApplicationModelConvention("/TenantManagement", model =>
            {
                model.Properties["Layout"] = "/Pages/Admin/Shared/_AdminLayout.cshtml";
            });
        });

    }
    private void ConfigureRequestLocalization(IServiceCollection services)
    {
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("vi")
            };
            var defaultCulture = "vi"; // Đặt vi làm default để match với URL
            options.DefaultRequestCulture = new RequestCulture(defaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;

            options.RequestCultureProviders.Clear();
            options.RequestCultureProviders.Add(new AspNetCoreCookieRequestCultureProvider());
            options.RequestCultureProviders.Add(new RouteValueRequestCultureProvider());
            options.RequestCultureProviders.Add(new CookieRequestCultureProvider
            {
                CookieName = "culture"
            });
            options.RequestCultureProviders.Add(new CookieRequestCultureProvider
            {
                CookieName = "Abp.Localization.CultureName"
            });
            options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());

            options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());

        });
        services.AddHttpContextAccessor();

    }
    private void ConfigureHealthChecks(ServiceConfigurationContext context)
    {
        context.Services.AddProductSellingHealthChecks();
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddContributors(typeof(BootstrapStyleContributor));
                }
            );
            options.ScriptBundles.Configure(
                 LeptonXLiteThemeBundles.Scripts.Global,
                bundle =>
                {
                    bundle.AddContributors(typeof(JQueryScriptContributor));
                    bundle.AddContributors(typeof(BootstrapScriptContributor));
                    bundle.AddContributors(typeof(DatatablesNetScriptContributor));

                    bundle.AddFiles("/js/shared/culture.js");
                }
            );


            options.StyleBundles.Add(
                "Main.Global",
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );

            options.ScriptBundles.Add(
                "Main.Global",
                bundle =>
                {
                    bundle.AddFiles("/js/global/cart/cart.js");
                    bundle.AddFiles("/global-scripts.js");
                    bundle.AddFiles("/js/global/main.js");
                }
            );

            options.ScriptBundles.Add(
                "Admin.Global", // Give it a descriptive name
                bundle =>
                {
                    bundle.AddContributors(typeof(DatatablesNetScriptContributor));
                    bundle.AddFiles("/js/admin/main.js");

                }
            );
            options.StyleBundles.Add(
                "Admin.Global", // Give it a descriptive name
                bundle =>
                {
                    bundle.AddFiles("/css/admin/main/style.css");
                }
            );
            options.ScriptBundles.Add(
                "Shared.Order.SignalR",
                bundle =>
                {
                    bundle.AddFiles("/libs/signalr/browser/signalr.js");

                    bundle.AddFiles("/js/orders/order-signalr.js");
                }
            );
        });
    }
    private void ConfigureRouting(IServiceCollection services)
    {
        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
        });
    }
    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ProductSellingWebModule>();
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ProductSellingWebModule>();

            if (hostingEnvironment.IsDevelopment())
            {
                options.FileSets.ReplaceEmbeddedByPhysical<ProductSellingDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Acme.ProductSelling.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<ProductSellingDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Acme.ProductSelling.Domain", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<ProductSellingApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Acme.ProductSelling.Application.Contracts", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<ProductSellingApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Acme.ProductSelling.Application", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<ProductSellingHttpApiModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}..{0}src{0}Acme.ProductSelling.HttpApi", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<ProductSellingWebModule>(hostingEnvironment.ContentRootPath);
            }
        });
    }

    private void ConfigureNavigationServices()
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new ProductSellingMenuContributor());
        });

        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new ProductSellingToolbarContributor());
        });
    }

    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(ProductSellingApplicationModule).Assembly);
        });
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductSelling API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );
    }


    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        app.UseForwardedHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }


        if (!env.IsDevelopment())
        {
            //app.UseErrorPage();
            app.UseExceptionHandler("/loi");
            app.UseHsts();
        }

        app.UseCorrelationId();
        app.MapAbpStaticAssets();
        app.UseAbpStudioLink();

        app.UseRouting();
        app.UseMiddleware<CultureRedirectMiddleware>();
        app.UseMiddleware<CultureSyncMiddleware>();
        //app.UseMiddleware<CultureMiddleware>();

        app.UseAbpRequestLocalization();

        app.UseAbpSecurityHeaders();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();
        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }
        //app.UseRequestLocalization();
        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseSwagger();

        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductSelling API");
        });
        app.UseConfiguredEndpoints(endpoints =>
        {

            endpoints.MapRazorPages();

            endpoints.MapHub<OrderHub>("/signalr-hubs/orders");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();

        RecurringJob.AddOrUpdate<CleanupOldOrdersJob>(
                "cleanup-old-orders",
                job => job.ExecuteAsync(new CleanupOldOrdersJobArgs { MonthsOld = 6 }),
                Cron.Monthly(1, 2) 
        );

        app.UseHangfireDashboard();
        app.UseExceptionHandler("/Error");

        app.UseStatusCodePagesWithReExecute("/loi", "?statusCode={0}");
    }
}
