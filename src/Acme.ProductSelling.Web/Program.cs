using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting web host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host
                .AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog((context, services, loggerConfiguration) =>
                {
                    loggerConfiguration
#if DEBUG
                        .MinimumLevel.Debug()
#else
                        .MinimumLevel.Information()
#endif
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                         .MinimumLevel.Override("OpenIddict", LogEventLevel.Warning)
                        .MinimumLevel.Override("OpenIddict.Validation", LogEventLevel.Warning)
                        .MinimumLevel.Override("OpenIddict.Server", LogEventLevel.Warning)
                        // Suppress HealthCheck polling
                        .MinimumLevel.Override("Microsoft.Extensions.Diagnostics.HealthChecks", LogEventLevel.Warning)
                        .MinimumLevel.Override("HealthChecks.UI", LogEventLevel.Warning)
                        // Suppress ABP tenant resolution per-request noise
                        .MinimumLevel.Override("Volo.Abp.MultiTenancy", LogEventLevel.Warning)
                        // Suppress static file serving
                        .MinimumLevel.Override("Microsoft.AspNetCore.StaticFiles", LogEventLevel.Warning)
                        // Suppress routing/endpoint selection details
                        .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.Async(c => c.File("Logs/logs.txt"))
                        .WriteTo.Async(c => c.Console())
                        .WriteTo.Async(c => c.AbpStudio(services));
                });
            await builder.AddApplicationAsync<ProductSellingWebModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
