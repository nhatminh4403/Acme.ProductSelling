using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Orders
{
    public interface IOrderNotificationService : ITransientDependency
    {
        Task NotifyOrderStatusChangeAsync(Order order);

    }
}
