using Acme.ProductSelling.Orders;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Payments
{
    public class CodPaymentGateway : IPaymentGateway
    {
        public string Name => PaymentMethods.COD;
        public Task<PaymentGatewayResult> ProcessAsync(Order order)
        {
            return Task.FromResult(new PaymentGatewayResult
            {
                RedirectUrl = null,
            });
        }
    }
}
