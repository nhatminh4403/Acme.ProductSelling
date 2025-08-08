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
        private readonly ILogger<PaymentGatewayResolver> Logger;
        public PaymentGatewayResolver(IEnumerable<IPaymentGateway> gateways,ILogger<PaymentGatewayResolver> logger)
        {
            Logger = logger;
            _gateways = gateways;
        }
        public IPaymentGateway Resolve(string name)
        {
            var gateway = _gateways.FirstOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase)); 

            if (gateway == null)
            {
                var availableGateways = string.Join(", ", _gateways.Select(g => g.Name));
                Logger.LogInformation("Các gateway có sẵn: [{AvailableGateways}]", availableGateways);
                Logger.LogWarning("Không thể resolve gateway" +
                    " '{RequestedName}'. Các gateway có sẵn: [{AvailableGateways}]", name, availableGateways);
                throw new UserFriendlyException($"Phương thức thanh toán '{name}' không được hỗ trợ.");
            }

            Logger.LogInformation("Đã resolve thành công gateway: {GatewayName}", gateway.Name);
            return gateway;
        }
    }
}
