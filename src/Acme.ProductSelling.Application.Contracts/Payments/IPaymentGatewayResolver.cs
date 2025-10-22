using Volo.Abp.DependencyInjection;
namespace Acme.ProductSelling.Payments
{
    public interface IPaymentGatewayResolver : ITransientDependency
    {
        IPaymentGateway Resolve(string name);
    }
}
