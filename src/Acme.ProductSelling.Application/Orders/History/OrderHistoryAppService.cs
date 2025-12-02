using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Payments;
using Microsoft.Extensions.Localization;
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
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly OrderHistoryToOrderHistoryDtoMapper _historyMapper;

        public OrderHistoryAppService(IRepository<OrderHistory, Guid> historyRepository, IGuidGenerator guidGenerator, ICurrentUser currentUser, OrderHistoryToOrderHistoryDtoMapper historyMapper, IStringLocalizer<ProductSellingResource> localizer)
        {
            _historyRepository = historyRepository;
            _guidGenerator = guidGenerator;
            _currentUser = currentUser;
            _historyMapper = historyMapper;
            _localizer = localizer;
        }

        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(Guid orderId)
        {
            var histories = await _historyRepository.GetListAsync();

            var orderHistories = histories
                            .Where(h => h.OrderId == orderId)
                            .OrderBy(h => h.CreationTime)
                            .ToList();


            var result = new List<OrderHistoryDto>();
            foreach (var history in orderHistories)
            {
                result.Add(_historyMapper.Map(history));
            }
            return result;
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
                description ?? _localizer["Order:HistoryUpdated"],
                _currentUser.UserName ?? "System"
            );

            await _historyRepository.InsertAsync(history, autoSave: true);
        }
    }
}
