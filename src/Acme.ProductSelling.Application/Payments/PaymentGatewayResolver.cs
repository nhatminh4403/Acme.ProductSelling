using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;

namespace Acme.ProductSelling.Payments
{
    public class PaymentGatewayResolver : IPaymentGatewayResolver
    {
        private readonly IEnumerable<IPaymentGateway> _gateways;

        public PaymentGatewayResolver(IEnumerable<IPaymentGateway> gateways)
        {
            _gateways = gateways ?? throw new ArgumentNullException(nameof(gateways));
        }


        public IPaymentGateway Resolve(string name)
        {
            // Dùng LINQ để tìm gateway đầu tiên có Name khớp với tên được yêu cầu.
            // Dùng StringComparison.OrdinalIgnoreCase để không phân biệt chữ hoa-thường ("VnPay" hay "vnpay" đều được).
            var gateway = _gateways.FirstOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            // Nếu không tìm thấy, ném ra một lỗi thân thiện với người dùng.
            if (gateway == null)
            {
                throw new UserFriendlyException($"Phương thức thanh toán '{name}' không được hỗ trợ.");
            }

            // Trả về gateway đã tìm thấy.
            return gateway;
        }
    }
}
