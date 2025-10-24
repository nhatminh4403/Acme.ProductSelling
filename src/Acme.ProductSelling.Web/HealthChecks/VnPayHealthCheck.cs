using Acme.ProductSelling.PaymentGateway.VnPay.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Web.HealthChecks
{
    public class VnPayHealthCheck : IHealthCheck, ITransientDependency
    {
        private readonly IVnPayService _vnPayService;

        public VnPayHealthCheck(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var config = _vnPayService.GetVnPayOptions();
                if (config == null)
                {
                    return HealthCheckResult.Unhealthy("VNPay configuration is missing");
                }
                // Ping VNPay endpoint or check configuration
                if (string.IsNullOrEmpty(config.TmnCode))
                    return HealthCheckResult.Unhealthy("VNPay not configured");

                return HealthCheckResult.Healthy("VNPay is ready");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("VNPay health check failed", ex);
            }
        }
    }
}
