using Acme.ProductSelling.PaymentGateway.PayPal;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Web.HealthChecks
{
    public class PayPalHealthCheck : IHealthCheck, ITransientDependency
    {
        private readonly IPayPalService _payPalService;
        public PayPalHealthCheck(IPayPalService payPalService)
        {
            _payPalService = payPalService;
        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var config = _payPalService.GetPayPalOption();
                if (config == null)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("VNPay configuration is missing"));
                }
                // Ping VNPay endpoint or check configuration
                if (string.IsNullOrEmpty(config.ClientId))
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