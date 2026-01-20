using Acme.ProductSelling.PaymentGateway.MoMo.Configurations.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Web.HealthChecks
{
    public class MoMoHealthCheck : IHealthCheck, ITransientDependency
    {
        private readonly IMoMoService _moMoService;
        public MoMoHealthCheck(IMoMoService moMoService)
        {
            _moMoService = moMoService;
        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var config = _moMoService.GetMoMoOption();
                if (config == null)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("VNPay configuration is missing"));
                }
                // Ping VNPay endpoint or check configuration
                if (string.IsNullOrEmpty(config.PartnerCode))
                    return Task.FromResult(HealthCheckResult.Unhealthy("VNPay not configured"));

                return Task.FromResult(HealthCheckResult.Healthy("VNPay is ready"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("VNPay health check failed", ex));
            }
        }
    }
}
