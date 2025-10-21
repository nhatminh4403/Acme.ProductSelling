using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Payments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.ProductSelling.Orders.Services
{
    public interface IOrderHistoryAppService : ITransientDependency
    {
        Task LogOrderChangeAsync(
            Guid orderId,
            OrderStatus oldStatus,
            OrderStatus newStatus,
            PaymentStatus oldPaymentStatus,
            PaymentStatus newPaymentStatus,
            string description);

        Task<List<OrderHistoryDto>> GetOrderHistoryAsync(Guid orderId);
    }
}
