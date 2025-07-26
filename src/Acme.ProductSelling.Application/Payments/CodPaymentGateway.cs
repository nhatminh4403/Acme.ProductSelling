using Acme.ProductSelling.Orders;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Payments
{
    public class CodPaymentGateway : IPaymentGateway, ITransientDependency
    {
        public string Name => PaymentConst.COD;
        public Task<PaymentGatewayResult> ProcessAsync(Order order)
        {
            return Task.FromResult(new PaymentGatewayResult
            {
                RedirectUrl = null,
                NextOrderStatus = OrderStatus.Placed
            });
        }
    }
}
