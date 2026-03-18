using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Payments;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Orders.Services
{
    [Authorize(ProductSellingPermissions.Orders.Default)]
    public class OrderQueryAppService : ProductSellingAppService, IOrderQueryAppService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderNotificationService _orderNotificationService;
        private readonly IOrderHistoryAppService _orderHistoryAppService;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly IDataFilter<ISoftDelete> _softDeleteFilter;
        private readonly OrderToOrderDtoMapper _orderMapper;
        private readonly IPaymentStatusPolicy _paymentStatusPolicy;
        private readonly ICurrentUser _currentUser;
        private readonly IIdentityUserRepository _userRepository;
        private readonly ILogger<OrderQueryAppService> _logger;

        private readonly IDistributedCache<OrderDto, Guid> _orderCache;

        public OrderQueryAppService(IOrderRepository orderRepository,
                                    IOrderNotificationService orderNotificationService,
                                    IOrderHistoryAppService orderHistoryAppService,
                                    IStringLocalizer<ProductSellingResource> localizer,
                                    IDataFilter<ISoftDelete> softDeleteFilter,
                                    OrderToOrderDtoMapper orderMapper,
                                    ICurrentUser currentUser,
                                    IIdentityUserRepository userRepository,
                                    ILogger<OrderQueryAppService> logger,
                                    IPaymentStatusPolicy paymentStatusPolicy,
                                    IDistributedCache<OrderDto, Guid> orderCache)
        {
            _orderRepository = orderRepository;
            _orderNotificationService = orderNotificationService;
            _orderHistoryAppService = orderHistoryAppService;
            _localizer = localizer;
            _softDeleteFilter = softDeleteFilter;
            _orderMapper = orderMapper;
            _currentUser = currentUser;
            _userRepository = userRepository;
            _logger = logger;
            _paymentStatusPolicy = paymentStatusPolicy;
            _orderCache = orderCache;
        }

        public async Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListInput input)
        {
            Logger.LogInformation(
               "[GetList] START - Filters: StoreId={StoreId}, OrderType={OrderType}, Status={Status}, Payment={Payment}",
               input.StoreId, input.OrderType, input.OrderStatus, input.PaymentStatus);

            using (input.IncludeDeleted ? _softDeleteFilter.Disable() : null)
            {
                var query = (await _orderRepository.GetQueryableAsync()).Include(o => o.OrderItems);
                IQueryable<Order> filtered = query;

                if (input.StoreId.HasValue) filtered = filtered.Where(o => o.StoreId == input.StoreId.Value);
                if (input.OrderType.HasValue) filtered = filtered.Where(o => o.OrderType == input.OrderType.Value);
                if (input.OrderStatus.HasValue) filtered = filtered.Where(o => o.OrderStatus == input.OrderStatus.Value);
                if (input.PaymentStatus.HasValue) filtered = filtered.Where(o => o.PaymentStatus == input.PaymentStatus.Value);
                if (input.StartDate.HasValue) filtered = filtered.Where(o => o.CreationTime >= input.StartDate.Value);
                if (input.EndDate.HasValue) filtered = filtered.Where(o => o.CreationTime <= input.EndDate.Value);

                var userStoreId = await GetCurrentUserStoreIdAsync();
                var isAdmin = await IsAdminOrManagerAsync();

                if (userStoreId.HasValue && !isAdmin)
                    filtered = filtered.Where(o => o.StoreId == userStoreId.Value);

                var totalCount = await AsyncExecuter.CountAsync(filtered);
                var items = await AsyncExecuter.ToListAsync(
                    filtered.OrderBy(input.Sorting ?? "CreationTime DESC").PageBy(input));

                return new PagedResultDto<OrderDto>(totalCount, _orderMapper.MapList(items));
            }
        }

        [Authorize(ProductSellingPermissions.Orders.Edit)]
        public async Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusDto input)
        {
            var order = await _orderRepository.GetAsync(id);
            var oldStatus = order.OrderStatus;
            var oldPayment = order.PaymentStatus;

            order.SetStatus(input.NewStatus);
            if (!_paymentStatusPolicy.CanTransition(order.PaymentStatus, input.NewPaymentStatus))
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.OrderInvalidPaymentStatusChange);
            order.SetPaymentStatus(input.NewPaymentStatus);

            await _orderRepository.UpdateAsync(order, autoSave: true);

            await _orderHistoryAppService.LogOrderChangeAsync(id, oldStatus, order.OrderStatus, oldPayment, order.PaymentStatus, _localizer["Order:UpdatedByAdmin"]);
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            await _orderCache.RemoveAsync(id);

            return _orderMapper.Map(order);
        }

        [Authorize(ProductSellingPermissions.Orders.Edit)]
        public async Task ShipOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetAsync(orderId) as OnlineOrder;

            if (order.OrderStatus != OrderStatus.Confirmed && order.OrderStatus != OrderStatus.Processing)
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.OrderCannotShip);

            order.SetStatus(OrderStatus.Shipped);
            if (order.PaymentMethod == PaymentMethods.COD) order.SetPendingOnDelivery();

            await _orderRepository.UpdateAsync(order, autoSave: true);
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            await _orderCache.RemoveAsync(orderId);
        }

        [Authorize(ProductSellingPermissions.Orders.Edit)]
        public async Task DeliverOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetAsync(orderId) as OnlineOrder;

            if (order == null)
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.OrderNotFound);

            if (order.OrderStatus != OrderStatus.Shipped)
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.OrderCannotDeliver);

            if (order.PaymentMethod == PaymentMethods.COD)
                order.MarkAsCodPaidAndCompleted();
            else
                order.SetStatus(OrderStatus.Delivered);

            await _orderRepository.UpdateAsync(order, autoSave: true);
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            await _orderCache.RemoveAsync(orderId);
        }

        [Authorize(ProductSellingPermissions.Orders.ConfirmCodPayment)]
        public async Task MarkAsCodPaidAndCompletedAsync(Guid orderId)
        {
            var order = await _orderRepository.GetAsync(orderId) as OnlineOrder;
            if (order == null)
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.OrderNotFound);
            order.MarkAsCodPaidAndCompleted();
            await _orderRepository.UpdateAsync(order, autoSave: true);
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            await _orderCache.RemoveAsync(orderId);

        }
        [RemoteService(false)]
        public async Task RestoreOrderAsync(Guid orderId)
        {
            var query = await _orderRepository.GetQueryableAsync();
            var order = await query.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null && order.IsDeleted)
            {
                order.IsDeleted = false;
                order.DeletionTime = null;
                order.DeleterId = null;
                await _orderRepository.UpdateAsync(order);
                await _orderCache.RemoveAsync(orderId);

            }
        }

        [Authorize(ProductSellingPermissions.Orders.Default)]
        public async Task<PagedResultDto<OrderDto>> GetDeletedOrdersAsync(PagedAndSortedResultRequestDto input)
        {
            Logger.LogInformation("[GetDeletedOrders] START - Skip: {Skip}, MaxResultCount: {MaxResultCount}",
                input.SkipCount, input.MaxResultCount);

            var query = (await _orderRepository.GetQueryableAsync())
                        .IgnoreQueryFilters()
                        .Where(o => o.IsDeleted)
                        .Include(o => o.OrderItems);

            var totalCount = await AsyncExecuter.CountAsync(query);
            var items = await AsyncExecuter.ToListAsync(
                query.OrderBy(input.Sorting ?? "DeletionTime DESC").PageBy(input)
            );

            Logger.LogInformation("[GetDeletedOrders] COMPLETED - TotalDeletedOrders: {TotalCount}, ReturnedCount: {ReturnedCount}",
                totalCount, items.Count);

            return new PagedResultDto<OrderDto>(
                totalCount,
                _orderMapper.MapList(items)
            );
        }
        [Authorize(ProductSellingPermissions.Orders.Default)]
        public async Task<PagedResultDto<OrderDto>> GetProfitReportAsync(PagedAndSortedResultRequestDto input)
        {
            Logger.LogInformation("[GetProfitReport] START - Skip: {Skip}, MaxResultCount: {MaxResultCount}",
                input.SkipCount, input.MaxResultCount);

            var query = (await _orderRepository.GetQueryableAsync())
                .Include(o => o.OrderItems)
                .Where(o => o.OrderStatus == OrderStatus.Delivered && o.PaymentStatus == PaymentStatus.Paid);

            var totalCount = await AsyncExecuter.CountAsync(query);
            var items = await AsyncExecuter.
                ToListAsync(query.OrderBy(input.Sorting ?? "CreationTime DESC").PageBy(input));

            Logger.LogInformation("[GetProfitReport] COMPLETED - TotalDeliveredPaidOrders: {TotalCount}, ReturnedCount: {ReturnedCount}",
                totalCount, items.Count);

            return new PagedResultDto<OrderDto>(
                totalCount,
                _orderMapper.MapList(items)
            );
        }
        private async Task<bool> IsAdminOrManagerAsync()
        {
            if (!_currentUser.Id.HasValue)
                return false;

            var isAdmin = await IsInRoleAsync("admin");
            var isManager = await IsInRoleAsync("manager");
            var result = isAdmin || isManager;

            Logger.LogDebug("[IsAdminOrManager] UserId: {UserId}, IsAdmin: {IsAdmin}, IsManager: {IsManager}, Result: {Result}",
                _currentUser.Id.Value, isAdmin, isManager, result);

            return result;
        }
        private async Task<bool> IsInRoleAsync(string roleName)
        {
            if (!_currentUser.Id.HasValue)
                return false;

            var user = await _userRepository.GetAsync(_currentUser.Id.Value);
            var roles = await _userRepository.GetRolesAsync(user.Id);
            var isInRole = roles.Any(r => string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));

            Logger.LogDebug("[IsInRole] UserId: {UserId}, Role: {RoleName}, Result: {Result}",
                _currentUser.Id.Value, roleName, isInRole);

            return isInRole;
        }
        private async Task<Guid?> GetCurrentUserStoreIdAsync()
        {
            if (!_currentUser.Id.HasValue)
            {
                Logger.LogDebug("[GetCurrentUserStoreId] No authenticated user");
                return null;
            }

            var user = await _userRepository.GetAsync(_currentUser.Id.Value);
            var storeIdProperty = user.GetProperty<Guid?>("AssignedStoreId");

            Logger.LogDebug("[GetCurrentUserStoreId] UserId: {UserId}, AssignedStoreId: {StoreId}",
                _currentUser.Id.Value, storeIdProperty?.ToString() ?? "None");

            return storeIdProperty;
        }
    }
}
