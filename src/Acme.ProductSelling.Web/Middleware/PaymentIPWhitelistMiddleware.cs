using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Web.Middleware
{
    public class PaymentIPWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PaymentIPWhitelistMiddleware> _logger;
        private readonly HashSet<string> _allowedIPs;

        public PaymentIPWhitelistMiddleware(
            RequestDelegate next,
            ILogger<PaymentIPWhitelistMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;

            // Load allowed IPs from configuration
            var ipList = configuration.GetSection("PaymentGateways:AllowedIPNIPs").Get<List<string>>()
                         ?? new List<string>();

            // Add localhost for development
            ipList.Add("::1");
            ipList.Add("127.0.0.1");

            _allowedIPs = new HashSet<string>(ipList, StringComparer.OrdinalIgnoreCase);

            _logger.LogInformation("Payment IPN IP whitelist initialized with {Count} IPs", _allowedIPs.Count);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only apply to IPN endpoints
            if (context.Request.Path.StartsWithSegments("/api/payment") &&
                context.Request.Path.Value.Contains("-ipn", StringComparison.OrdinalIgnoreCase))
            {
                var remoteIp = GetClientIP(context);

                _logger.LogDebug("IPN request from IP: {IP} to {Path}", remoteIp, context.Request.Path);

                if (!_allowedIPs.Contains(remoteIp))
                {
                    _logger.LogWarning(
                        "Blocked IPN request from unauthorized IP: {IP} to {Path}",
                        remoteIp, context.Request.Path
                    );

                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Forbidden");
                    return;
                }

                _logger.LogInformation("Allowed IPN request from {IP}", remoteIp);
            }

            await _next(context);
        }

        private string GetClientIP(HttpContext context)
        {
            // Check X-Forwarded-For header first
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            // Check X-Real-IP header
            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(realIp))
            {
                return realIp;
            }

            // Fall back to RemoteIpAddress
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}