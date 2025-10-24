using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;

namespace Acme.ProductSelling.Payments
{
    public class PaymentGatewayResolver : IPaymentGatewayResolver
    {
        private readonly IEnumerable<IPaymentGateway> _gateways;
        private readonly ILogger<PaymentGatewayResolver> _logger;
        public PaymentGatewayResolver(IEnumerable<IPaymentGateway> gateways, ILogger<PaymentGatewayResolver> logger)
        {
            _logger = logger;
            _gateways = gateways;
        }
        public IPaymentGateway Resolve(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("Attempted to resolve gateway with null or empty name");
                throw new ArgumentException("Payment method name cannot be null or empty", nameof(name));
            }
            _logger.LogDebug("Attempting to resolve payment gateway: {Name}", name);

            var gateway = _gateways.FirstOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (gateway == null)
            {
                var availableGateways = string.Join(", ", _gateways.Select(g => g.Name));
                _logger.LogInformation("Các gateway có sẵn: [{AvailableGateways}]", availableGateways);
                _logger.LogWarning("Không thể resolve gateway" +
                    " '{RequestedName}'. Các gateway có sẵn: [{AvailableGateways}]", name, availableGateways);
                throw new UserFriendlyException(
                               $"Phương thức thanh toán '{name}' không được hỗ trợ. " +
                               $"Các phương thức có sẵn: {availableGateways}"
                           );
            }

            _logger.LogInformation("Đã resolve thành công gateway: {GatewayName}", gateway.Name);
            return gateway;
        }
    }
}
