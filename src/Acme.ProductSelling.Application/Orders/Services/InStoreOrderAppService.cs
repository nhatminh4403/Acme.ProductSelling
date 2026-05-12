using Acme.ProductSelling.Identity;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Payments;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;

namespace Acme.ProductSelling.Orders.Services
{
    [Authorize]
    public class InStoreOrderAppService : ProductSellingAppService, IInStoreOrderAppService
    {
        private readonly OrderManager _orderManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly IOrderHistoryAppService _orderHistoryAppService;
        private readonly OrderToOrderDtoMapper _orderMapper;
        private readonly IDistributedEventBus _eventBus;
        public InStoreOrderAppService(
            OrderManager orderManager,
            IOrderRepository orderRepository,
            IIdentityUserRepository userRepository,
            IAppUserRepository appUserRepository,
            IStringLocalizer<ProductSellingResource> localizer,
            IOrderHistoryAppService orderHistoryAppService,
            OrderToOrderDtoMapper orderMapper,
            IDistributedEventBus eventBus)
        {
            _orderManager = orderManager;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _appUserRepository = appUserRepository;
            //_orderNotificationService = orderNotificationService;
            _localizer = localizer;
            _orderHistoryAppService = orderHistoryAppService;
            _orderMapper = orderMapper;
            _eventBus = eventBus;
        }
        [Authorize(ProductSellingPermissions.Orders.Create)]
        public async Task<OrderDto> CreateInStoreOrderAsync(CreateInStoreOrderDto input)
        {

            if (!input.CurrentUserStoreId.HasValue)
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.UserNotAssignedToStore);

            var currentUser = await _userRepository.GetAsync(CurrentUser.Id.Value);
            if (currentUser == null)
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.StaffNotFound);

            var isAdminOrManager = CurrentUser.IsInRole(ExtendedRoleConsts.Admin) ||
                                   CurrentUser.IsInRole(ExtendedRoleConsts.Manager);            

            // Non-admin/manager must be creating order for their own assigned store
            if (!isAdminOrManager)
            {
                var appUser = await _appUserRepository.GetAsync(CurrentUser.Id.Value);
                if (appUser.AssignedStoreId != input.CurrentUserStoreId.Value)
                    throw new UserFriendlyException(ProductSellingDomainErrorCodes.NoStoreAccess);
            }

            // Delegate to Domain Manager
            var items = input.Items.Select(x => (x.ProductId, x.Quantity)).ToList();
            
            var order = await _orderManager.CreateInStoreOrderAsync(
                input.CurrentUserStoreId.Value,
                currentUser.Id,
                currentUser.Name ?? currentUser.UserName,
                input.CustomerName,
                input.CustomerPhone,
                input.PaymentMethod,
                items
            );

            await _orderRepository.InsertAsync(order, autoSave: true);

            // Side Effects
            await _eventBus.PublishAsync(new OrderStatusChangedEto
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                OrderStatus = order.OrderStatus.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                StoreId = order.StoreId,
                IsInStore = order.OrderType == OrderType.InStore
            });
            await _orderHistoryAppService.LogOrderChangeAsync(order.Id, OrderStatus.Pending, order.OrderStatus, PaymentStatus.Unpaid, order.PaymentStatus, _localizer["Order:InStoreCreated"]);
            Logger.LogInformation("In-store order {OrderId} created successfully for user {UserId} in store {StoreId}", order.Id, CurrentUser.Id, input.CurrentUserStoreId.Value);

            return MapToDto(order, "");
        }

        [Authorize(ProductSellingPermissions.Orders.Complete)]
        public async Task<OrderDto> CompleteInStorePaymentAsync(Guid orderId, CompleteInStorePaymentDto input)
        {
            var order = await _orderRepository.GetAsync(orderId) as InStoreOrder;
            if (order.OrderType != OrderType.InStore) throw new UserFriendlyException(ProductSellingDomainErrorCodes.OrderOnlyForInStoreOrders);


            var isAdminOrManager = CurrentUser.IsInRole(ExtendedRoleConsts.Admin) ||
                                   CurrentUser.IsInRole(ExtendedRoleConsts.Manager);

            if (!isAdminOrManager)
            {
                var cashier = await _appUserRepository.GetAsync(CurrentUser.Id.Value);
                if (cashier.AssignedStoreId != order.StoreId)
                    throw new UserFriendlyException(ProductSellingDomainErrorCodes.NoStoreAccess);
            }

            var currentUser = await _userRepository.GetAsync(CurrentUser.Id.Value);
            var oldStatus = order.OrderStatus;
            var oldPayment = order.PaymentStatus;

            // Domain Entity Logic
            order.CompletePaymentInStore(currentUser.Id, currentUser.Name ?? currentUser.UserName, Clock.Now);
            await _orderRepository.UpdateAsync(order, autoSave: true);

            await _eventBus.PublishAsync(new OrderStatusChangedEto
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                OrderStatus = order.OrderStatus.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                StoreId = order.StoreId,
                IsInStore = order.OrderType == OrderType.InStore
            });
            await _orderHistoryAppService.LogOrderChangeAsync(orderId, oldStatus, order.OrderStatus, oldPayment, order.PaymentStatus, _localizer["Order:PaidInStore", input.PaidAmount]);

            return MapToDto(order);
        }

        [Authorize(ProductSellingPermissions.Orders.Fulfill)]
        public async Task<OrderDto> FulfillInStoreOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetAsync(orderId) as InStoreOrder;
            if (order.OrderType != OrderType.InStore)
                throw new UserFriendlyException(ProductSellingDomainErrorCodes.OrderOnlyForInStoreOrders);

            var isAdminOrManager = CurrentUser.IsInRole(ExtendedRoleConsts.Admin) ||
                                   CurrentUser.IsInRole(ExtendedRoleConsts.Manager);

            if (!isAdminOrManager)
            {
                var staff = await _appUserRepository.GetAsync(CurrentUser.Id.Value);
                if (staff.AssignedStoreId != order.StoreId)
                    throw new UserFriendlyException(ProductSellingDomainErrorCodes.NoStoreAccess);
            }

            var currentUser = await _userRepository.GetAsync(CurrentUser.Id.Value);
            order.FulfillInStore(currentUser.Id, currentUser.Name ?? currentUser.UserName, Clock.Now);
            await _orderRepository.UpdateAsync(order, autoSave: true);

            await _eventBus.PublishAsync(new OrderStatusChangedEto
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                OrderStatus = order.OrderStatus.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                StoreId = order.StoreId,
                IsInStore = order.OrderType == OrderType.InStore
            });

            return MapToDto(order);
        }

        private OrderDto MapToDto(Order order, string storeName = null)
        {
            var dto = _orderMapper.Map(order);
            dto.StoreName = storeName;
            return dto;
        }

        [Authorize(ProductSellingPermissions.Orders.Default)]
        public async Task<PagedResultDto<OrderDto>> GetListAsync()
        {
            var query = (await _orderRepository.GetQueryableAsync())
                    .Include(o => o.OrderItems)
                    .Where(o => o.OrderType == OrderType.InStore);

            var items = await query.ToListAsync();
            var totalCount = items.Count;

            return new PagedResultDto<OrderDto>(
                totalCount,
                _orderMapper.MapList(items)
            );
        }
    }
}
