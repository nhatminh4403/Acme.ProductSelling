using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Orders.Hubs
{
    public interface IOrderClient
    {
        Task RecieveOrderStatusUpdate(Guid orderId, string orderNumber, OrderStatus newStatus, string message);
    }
}
