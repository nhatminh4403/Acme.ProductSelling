using Acme.ProductSelling.Orders.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Acme.ProductSelling.Orders.BackgroundJobs
{
    public class SetOrderPendingJob : IAsyncBackgroundJob<SetOrderPendingJobArgs>, ITransientDependency
    {
        private readonly IRepository<Order, Guid> _orderRepository;
        private readonly IHubContext<OrderHub, IOrderClient> _orderHubContext;
        public SetOrderPendingJob(
                  IRepository<Order, Guid> orderRepository,
                  IHubContext<OrderHub, IOrderClient> orderHubContext)
        {
            _orderRepository = orderRepository;
            _orderHubContext = orderHubContext;
        }

        [UnitOfWork]
        public async Task ExecuteAsync(SetOrderPendingJobArgs args)
        {
            var order = await _orderRepository.GetAsync(args.OrderId);
            if (order == null)
            {
                throw new Exception($"Order with ID {args.OrderId} not found.");
            }

            if (order.Status != OrderStatus.Placed)
            {
                throw new Exception($"Order with ID {args.OrderId} is not in a valid state to be set to pending.");
            }
            order.SetPendingStatus(); // Dùng phương thức nội bộ để đổi status
            await _orderRepository.UpdateAsync(order);

            // Gửi thông báo real-time tới tất cả client
            await _orderHubContext.Clients.All.ReceiveOrderStatusUpdate(
                order.Id,
                order.Status.ToString()
            );
        }
    }
   
}
