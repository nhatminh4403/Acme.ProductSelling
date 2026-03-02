using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Payments;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Orders.Services
{
    public class OrderHistoryAppService : IOrderHistoryAppService
    {
        private const int HistoryCacheTtlMinutes = 30;
        private const string CacheKeyPrefix = "order:history:";

        private readonly IRepository<OrderHistory, Guid> _historyRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ICurrentUser _currentUser;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly OrderHistoryToOrderHistoryDtoMapper _historyMapper;

        private readonly IDistributedCache<List<OrderHistoryDto>, string> _historyCache;

        public OrderHistoryAppService(IRepository<OrderHistory, Guid> historyRepository, IGuidGenerator guidGenerator, ICurrentUser currentUser, OrderHistoryToOrderHistoryDtoMapper historyMapper, IStringLocalizer<ProductSellingResource> localizer, IDistributedCache<List<OrderHistoryDto>, string> historyCache)
        {
            _historyRepository = historyRepository;
            _guidGenerator = guidGenerator;
            _currentUser = currentUser;
            _historyMapper = historyMapper;
            _localizer = localizer;
            _historyCache = historyCache;
        }

        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(Guid orderId)
        {
            var cacheKey = CacheKeyPrefix + orderId;

            return await _historyCache.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    // NOTE: GetListAsync loads all rows; for orders with many history entries
                    // a filtered repository method (GetListAsync(h => h.OrderId == orderId)) is
                    // preferable to avoid a full table scan.
                    var histories = await _historyRepository.GetListAsync(h => h.OrderId == orderId);

                    return histories
                        .OrderBy(h => h.CreationTime)
                        .Select(h => _historyMapper.Map(h))
                        .ToList();
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(HistoryCacheTtlMinutes)
                });
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
            await _historyCache.RemoveAsync(CacheKeyPrefix + orderId);

        }
    }
}
