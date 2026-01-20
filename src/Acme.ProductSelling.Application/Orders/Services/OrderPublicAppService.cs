using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.BackgroundJobs.OrderPending;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Payments;
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
using Volo.Abp.Auditing;
using Volo.Abp.Authorization;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Orders.Services
{
    [Authorize]
    public class OrderPublicAppService : ProductSellingAppService, IOrderPublicAppService
    {
        private readonly OrderManager _orderManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IRepository<Cart, Guid> _cartRepository;
        private readonly IPaymentGatewayResolver _paymentGatewayResolver;
        private readonly IBackgroundJobManager _backgroundJobManager;
        //private readonly IOrderNotificationService _orderNotificationService;
        private readonly IDistributedEventBus _eventBus;
        private readonly IOrderHistoryAppService _orderHistoryAppService;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly OrderToOrderDtoMapper _orderMapper;
        private readonly ICurrentUser _currentUser;
        public OrderPublicAppService(
            OrderManager orderManager,
            IOrderRepository orderRepository,
            IRepository<Cart, Guid> cartRepository,
            IPaymentGatewayResolver paymentGatewayResolver,
            IBackgroundJobManager backgroundJobManager,
            //IOrderNotificationService orderNotificationService,
            IOrderHistoryAppService orderHistoryAppService,
            IStringLocalizer<ProductSellingResource> localizer,
            OrderToOrderDtoMapper orderMapper,
            ICurrentUser currentUser,
            IDistributedEventBus eventBus)
        {
            _orderManager = orderManager;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _paymentGatewayResolver = paymentGatewayResolver;
            _backgroundJobManager = backgroundJobManager;
            //_orderNotificationService = orderNotificationService;
            _orderHistoryAppService = orderHistoryAppService;
            _localizer = localizer;
            _orderMapper = orderMapper;
            _currentUser = currentUser;
            _eventBus = eventBus;
        }

        public async Task<CreateOrderResultDto> CreateAsync(CreateOrderDto input)
        {
            var customerId = CurrentUser.Id.Value;

            // 1. Get Cart
            var cart = await _cartRepository.WithDetailsAsync(c => c.Items)
                .Result.FirstOrDefaultAsync(c => c.UserId == customerId);

            if (cart == null || !cart.Items.Any()) throw new UserFriendlyException(_localizer["Cart:IsEmpty"]);

            // 2. Delegate Creation to Domain (Includes Stock Check)
            var order = await _orderManager.CreateOnlineOrderAsync(
                customerId,
                input.CustomerName,
                input.CustomerPhone,
                input.ShippingAddress,
                input.PaymentMethod,
                cart.Items.ToList()
            );

            // 3. Process Payment
            var gateway = _paymentGatewayResolver.Resolve(input.PaymentMethod);
            if (input.PaymentMethod != PaymentMethods.COD) order.SetStatus(OrderStatus.Placed);
            // ^ Small logic adjustment based on Gateway specifics if needed

            var gatewayResult = await gateway.ProcessAsync(order);

            // 4. Persist
            await _orderRepository.InsertAsync(order, autoSave: true);
            await _cartRepository.DeleteAsync(cart);

            // 5. Jobs & Notifications
            if (input.PaymentMethod == PaymentMethods.COD)
            {
                await _backgroundJobManager.EnqueueAsync(new SetOrderBackgroundJobArgs { OrderId = order.Id }, delay: TimeSpan.FromHours(1));
            }
            //await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            await _eventBus.PublishAsync(new OrderStatusChangedEto
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                OrderStatus = order.OrderStatus.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                StoreId = order.StoreId,
                IsInStore = order.OrderType == OrderType.Online
            });
            await _orderHistoryAppService.LogOrderChangeAsync(order.Id, OrderStatus.Pending, order.OrderStatus, PaymentStatus.Unpaid, order.PaymentStatus, _localizer["Order:Created"]);

            return new CreateOrderResultDto { Order = _orderMapper.Map(order), RedirectUrl = gatewayResult.RedirectUrl };
        }
        [RemoteService(false)]
        public async Task<OrderDto> ConfirmPayPalOrderAsync(Guid guid)
        {
            var order = await _orderRepository.GetAsync(guid);
            if (order.CustomerId != CurrentUser.Id) throw new EntityNotFoundException(typeof(Order), guid);

            if (order.PaymentStatus == PaymentStatus.Pending)
            {
                order.MarkAsPaidOnline();
                await _orderRepository.UpdateAsync(order, autoSave: true);
                //await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
                await _eventBus.PublishAsync(new OrderStatusChangedEto
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    OrderStatus = order.OrderStatus.ToString(),
                    PaymentStatus = order.PaymentStatus.ToString(),
                    StoreId = order.StoreId,
                    IsInStore = order.OrderType == OrderType.Online
                });
            }
            return _orderMapper.Map(order);
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _orderRepository.GetAsync(id, includeDetails: true);
            if (order.CustomerId != CurrentUser.Id) throw new AbpAuthorizationException(L["Account:Unauthorized"]);

            // Domain Manager handles logic + restoring stock
            await _orderManager.RestoreStockAsync(order);

            order.CancelByUser(_localizer);
            await _orderRepository.UpdateAsync(order, autoSave: true);

            //await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            await _eventBus.PublishAsync(new OrderStatusChangedEto
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                OrderStatus = order.OrderStatus.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                StoreId = order.StoreId,
                IsInStore = order.OrderType == OrderType.Online
            });
            await _orderHistoryAppService.LogOrderChangeAsync(id, OrderStatus.Pending, order.OrderStatus, PaymentStatus.Unpaid, order.PaymentStatus, _localizer["Order:CancelledByUser"]);
        }

        public async Task<OrderDto> GetAsync(Guid id)
        {
            Logger.LogInformation("[GetOrder] START - OrderId: {OrderId}, UserId: {UserId}", id, _currentUser.Id);

            var order = await (await _orderRepository.WithDetailsAsync(o => o.OrderItems))
                             .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                Logger.LogWarning("[GetOrder] FAILED - Order not found. OrderId: {OrderId}", id);
                throw new EntityNotFoundException(typeof(Order), id);
            }

            Logger.LogInformation("[GetOrder] COMPLETED - OrderId: {OrderId}, OrderNumber: {OrderNumber}, Status: {Status}, ItemCount: {ItemCount}",
                order.Id, order.OrderNumber, order.OrderStatus, order.OrderItems.Count);

            return _orderMapper.Map(order);
        }
        [DisableAuditing]
        [Authorize]
        public async Task<PagedResultDto<OrderDto>> GetListForCurrentUserAsync(PagedAndSortedResultRequestDto input)
        {
            Logger.LogInformation("[GetListForCurrentUser] START - UserId: {UserId}, Skip: {Skip}, MaxResultCount: {MaxResultCount}",
                _currentUser.Id, input.SkipCount, input.MaxResultCount);

            if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
            {
                Logger.LogWarning("[GetListForCurrentUser] FAILED - User not authenticated");
                throw new AbpAuthorizationException(L["Account:UserNotAuthenticated"]);
            }

            var currentUserId = _currentUser.Id.Value;
            var queryable = (await _orderRepository.GetQueryableAsync())
                            .Where(o => o.CustomerId == currentUserId)
                            .Include(o => o.OrderItems)
                             .OrderByDescending(o => o.CreationTime).AsNoTracking();

            var totalCount = await AsyncExecuter.CountAsync(queryable);
            var orders = await AsyncExecuter.ToListAsync(
                queryable
                    .OrderBy(input.Sorting ?? $"{nameof(Order.OrderDate)} DESC")
                    .PageBy(input)
            );

            Logger.LogInformation("[GetListForCurrentUser] COMPLETED - UserId: {UserId}, TotalOrders: {TotalCount}, ReturnedCount: {ReturnedCount}",
                currentUserId, totalCount, orders.Count);

            return new PagedResultDto<OrderDto>(
                totalCount,
                _orderMapper.MapList(orders)
            );
        }
        public async Task<OrderDto> GetByOrderNumberAsync(string orderNumber)
        {
            Logger.LogInformation("[GetByOrderNumber] START - OrderNumber: {OrderNumber}", orderNumber);

            var order = await (await _orderRepository.WithDetailsAsync(o => o.OrderItems))
                             .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
            {
                Logger.LogWarning("[GetByOrderNumber] FAILED - Order not found. OrderNumber: {OrderNumber}", orderNumber);
                throw new UserFriendlyException(L["Order:OrderNotFound"]);
            }

            Logger.LogInformation("[GetByOrderNumber] COMPLETED - OrderId: {OrderId}, OrderNumber: {OrderNumber}",
                order.Id, order.OrderNumber);

            return _orderMapper.Map(order);
        }

    }
}
