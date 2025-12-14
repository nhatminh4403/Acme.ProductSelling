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
using Acme.ProductSelling.Products.BackgroundJobs.RecentlyViewed;
using Acme.ProductSelling.Products.Specification;
using Acme.ProductSelling.Web.Filters;
using Acme.ProductSelling.Web.Hangfire;
using Acme.ProductSelling.Web.HealthChecks;
using Acme.ProductSelling.Web.Menus;
using Acme.ProductSelling.Web.Middleware;
using Acme.ProductSelling.Web.Routing;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
using System.Threading.Tasks;
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
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity.Web;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Security.Claims;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
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
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAspNetCoreMvcUiModule),
    //typeof(AbpBackgroundJobOptions),
    typeof(AbpBackgroundJobsHangfireModule),
    typeof(AbpMapperlyModule)

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
        //serverBuilder.AddProductionEncryptionAndSigningCertificate("openiddict.pfx", configuration["AuthServer:CertificatePassPhrase"]!);
        //serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                var certificateLoaded = false;

                var certThumbprint = configuration["AuthServer:CertificateThumbprint"];

                // Priority 1: Certificate store (Azure)
                if (!string.IsNullOrWhiteSpace(certThumbprint))
                {
                    try
                    {
                        var certificate = LoadCertificateFromStore(certThumbprint);
                        if (certificate != null)
                        {
                            serverBuilder.AddSigningCertificate(certificate);
                            serverBuilder.AddEncryptionCertificate(certificate);
                            certificateLoaded = true;
                        }
                    }
                    catch
                    {

                    }
                }
                if (!certificateLoaded)
                {
                    var certPath = Path.Combine(hostingEnvironment.ContentRootPath, "openiddict.pfx");
                    var certPass = configuration["AuthServer:CertificatePassPhrase"];

                    if (File.Exists(certPath) && !string.IsNullOrWhiteSpace(certPass))
                    {
                        try
                        {
                            // Use standard OpenIddict methods to add from file
                            if (new FileInfo(certPath).Length > 0)
                            {
                                // Assuming these keys serve as both
                                serverBuilder.AddEncryptionCertificate(new FileStream(certPath, FileMode.Open, FileAccess.Read), certPass);
                                serverBuilder.AddSigningCertificate(new FileStream(certPath, FileMode.Open, FileAccess.Read), certPass);
                                certificateLoaded = true;
                            }
                        }
                        catch
                        {
                            // File read failed
                        }
                    }
                }
                if (!certificateLoaded)
                {
                    serverBuilder.AddEphemeralEncryptionKey();
                    serverBuilder.AddEphemeralSigningKey();
                }

                // Database storage will be used if no certificate was loaded above
                serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
            });
        }

    }
    private static System.Security.Cryptography.X509Certificates.X509Certificate2? LoadCertificateFromStore(string thumbprint)
    {
        using var store = new System.Security.Cryptography.X509Certificates.X509Store(
            System.Security.Cryptography.X509Certificates.StoreName.My,
            System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser
        );

        try
        {
            store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);
            var certificates = store.Certificates.Find(
                System.Security.Cryptography.X509Certificates.X509FindType.FindByThumbprint,
                thumbprint,
                validOnly: false
            );
            return certificates.Count > 0 ? certificates[0] : null;
        }
        catch
        {
            return null;
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

        ConfigureHangfire(services, configuration);

        ConfigureBundles();
        ConfigureUrls(configuration);
        ConfigureHealthChecks(context);
        ConfigureAuthentication(context);
        //ConfigureAutoMapper(services);
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureNavigationServices();
        ConfigureAutoApiControllers();
        ConfigureSwaggerServices(services);

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

    private void ConfigureHangfire(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            config.UseMemoryStorage();
        });
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 1;
            options.SchedulePollingInterval = TimeSpan.FromHours(6); // From 15 seconds
            options.HeartbeatInterval = TimeSpan.FromMinutes(5);
            options.ServerCheckInterval = TimeSpan.FromMinutes(5); // Add this
            options.ServerTimeout = TimeSpan.FromMinutes(10);
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
            options.Conventions.AddPageRoute("/Account/Manage", "/admin/Account/Manage");


            //options.Conventions.AddPageRoute("/Admin/Blogs/Index", "/{prefix:regex(^(admin|blogger|manager)$)}/blogs");
            //options.Conventions.AddPageRoute("/Admin/Products/Index", "/{prefix:regex(^(admin|manager|seller|cashier|warehouse)$)}/products");
            //options.Conventions.AddPageRoute("/Admin/Orders/Index", "/{prefix:regex(^(admin|manager|seller|cashier|warehouse)$)}/orders");

            // Add more routes as needed
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
        context.Services.AddHealthChecks().AddCheck<VnPayHealthCheck>("VnPay");
        context.Services.AddHealthChecks().AddCheck<MoMoHealthCheck>("MoMo");
        context.Services.AddHealthChecks().AddCheck<PayPalHealthCheck>("PayPal");
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
                    bundle.AddFiles("/css/base.css");
                    bundle.AddFiles("/css/shared/header.css");
                }
            );

            options.ScriptBundles.Add(
                "Main.Global",
                bundle =>
                {
                    bundle.AddFiles("/global-scripts.js");

                    bundle.AddFiles("/js/global/main.js");
                    bundle.AddFiles("/js/global/header-sidebar.js");
                    bundle.AddFiles("/js/global/cart/cart.utils.js");
                    bundle.AddFiles("/js/global/cart/cart.widget.js");
                    bundle.AddFiles("/js/global/cart/cart.actions.js");

                }
            );

            options.ScriptBundles.Add(
                "Admin.Global",
                bundle =>
                {
                    bundle.AddContributors(typeof(DatatablesNetScriptContributor));
                    bundle.AddFiles("/js/admin/main.js");

                }
            );
            options.StyleBundles.Add(
                "Admin.Global",
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

                    bundle.AddFiles("/js/shared/orderSignalR.js");
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
        context.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            options.Events.OnRedirectToLogin = context =>
            {
                // Only redirect to custom error page if it's not already a redirect
                if (!context.Request.Path.StartsWithSegments("/loi") &&
                    !context.Request.Path.StartsWithSegments("/Account/Login"))
                {
                    context.Response.Redirect($"/loi?statusCode=401");
                }
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = context =>
            {
                // Only redirect to custom error page if it's not already a redirect
                if (!context.Request.Path.StartsWithSegments("/loi"))
                {
                    context.Response.Redirect($"/loi?statusCode=403");
                }
                return Task.CompletedTask;
            };
        });
    }

    private void ConfigureAutoMapper(IServiceCollection services)
    {
        services.AddMapperlyObjectMapper();
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ProductSellingWebModule>();
            options.FileSets.AddEmbedded<Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.AbpAspNetCoreMvcUiThemeSharedModule>();

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
        app.UseStatusCodePagesWithReExecute("/loi", "?statusCode={0}");

        app.UseMiddleware<QueryStringFilterMiddleware>();

        app.UseCorrelationId();
        app.MapAbpStaticAssets();
        app.UseAbpStudioLink();

        app.UseRouting();
        app.UseMiddleware<RequestCultureMiddleware>();

        app.UseMiddleware<PaymentIPWhitelistMiddleware>();


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
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            // Tell Hangfire to use our custom filter
            Authorization = new[] { new HangfireDashboardPermissionFilter() }
        });

        RecurringJob.AddOrUpdate<CleanupOldOrdersJob>(
                "cleanup-old-orders",
                job => job.ExecuteAsync(new CleanupOldOrdersJobArgs { MonthsOld = 6 }),
                Cron.Monthly(1, 2)
        );
        RecurringJob.AddOrUpdate<RecentlyViewedCleanupJob>(
                "recently-viewed-cleanup",
                job => job.ExecuteAsync(new RecentlyViewedCleanupArgs
                { DaysToKeep = RecentlyViewedConsts.CleanupDaysToKeep }),
                Cron.Daily(0, 30)
        );
        app.UseConfiguredEndpoints(endpoints =>
        {

            endpoints.MapRazorPages();

            endpoints.MapHub<OrderHub>("/signalr-hubs/orders");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();

        //app.UseExceptionHandler("/Error");

    }
}
