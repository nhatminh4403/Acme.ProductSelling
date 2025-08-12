using System;
using System.Threading.Tasks;

namespace Acme.ProductSelling.Orders.Hubs
{
    public interface IOrderClient
    {
        Task ReceiveOrderStatusUpdate(Guid orderId, string newStatus, string newOrderStatusText,
                                                string newPaymentStatus,string newPaymentStatusText);
    }
}
