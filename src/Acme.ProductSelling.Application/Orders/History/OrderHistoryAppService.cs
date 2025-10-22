using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Orders.History
{
    public class OrderHistoryAppService : IOrderHistoryAppService
    {

        private readonly IRepository<OrderHistory, Guid> _historyRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ICurrentUser _currentUser;

        public OrderHistoryAppService(IRepository<OrderHistory, Guid> historyRepository, IGuidGenerator guidGenerator, ICurrentUser currentUser)
        {
            _historyRepository = historyRepository;
            _guidGenerator = guidGenerator;
            _currentUser = currentUser;
        }

        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(Guid orderId)
        {
            var histories = await _historyRepository.GetListAsync();

            var orderHistories = histories
                            .Where(h => h.OrderId == orderId)
                            .OrderBy(h => h.CreationTime)
                            .ToList();


            return orderHistories.Select(h => new OrderHistoryDto
            {
                Id = h.Id,
                OrderId = h.OrderId,
                OldStatus = h.OldStatus.ToString(),
                NewStatus = h.NewStatus.ToString(),
                OldPaymentStatus = h.OldPaymentStatus.ToString(),
                NewPaymentStatus = h.NewPaymentStatus.ToString(),
                ChangeDescription = h.ChangeDescription,
                ChangedBy = h.ChangedBy,
                CreationTime = h.CreationTime
            }).ToList();
        }

        public async Task LogOrderChangeAsync(Guid orderId, OrderStatus oldStatus, OrderStatus newStatus, PaymentStatus oldPaymentStatus, PaymentStatus newPaymentStatus, string description)
        {
            if (oldStatus == newStatus && oldPaymentStatus == newPaymentStatus)
            {
                return;
            }

            var history = new OrderHistory(
                _guidGenerator.Create(),
                orderId,
                oldStatus,
                newStatus,
                oldPaymentStatus,
                newPaymentStatus,
                description ?? "Order updated",
                _currentUser.UserName ?? "System"
            );

            await _historyRepository.InsertAsync(history, autoSave: true);
        }
    }
}
