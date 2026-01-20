using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Payments;
using Acme.ProductSelling.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
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
        //private readonly IOrderNotificationService _orderNotificationService;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly IOrderHistoryAppService _orderHistoryAppService;
        private readonly OrderToOrderDtoMapper _orderMapper;
        private readonly IDistributedEventBus _eventBus;
        public InStoreOrderAppService(
            OrderManager orderManager,
            IOrderRepository orderRepository,
            IIdentityUserRepository userRepository,
            //IOrderNotificationService orderNotificationService,
            IStringLocalizer<ProductSellingResource> localizer,
            IOrderHistoryAppService orderHistoryAppService,
            OrderToOrderDtoMapper orderMapper,
            IDistributedEventBus eventBus)
        {
            _orderManager = orderManager;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            //_orderNotificationService = orderNotificationService;
            _localizer = localizer;
            _orderHistoryAppService = orderHistoryAppService;
            _orderMapper = orderMapper;
            _eventBus = eventBus;
        }
        [Authorize(ProductSellingPermissions.Orders.Create)]
        public async Task<OrderDto> CreateInStoreOrderAsync(CreateInStoreOrderDto input)
        {
            var userStoreId = await GetCurrentUserStoreIdAsync();
            if (!userStoreId.HasValue) throw new UserFriendlyException(_localizer["Order:UserNotAssignedToStore"]);

            var currentUser = await _userRepository.GetAsync(CurrentUser.Id.Value);

            // Delegate to Domain Manager
            var items = input.Items.Select(x => (x.ProductId, x.Quantity)).ToList();
            var order = await _orderManager.CreateInStoreOrderAsync(
                userStoreId.Value,
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
            await _orderHistoryAppService.LogOrderChangeAsync(order.Id, OrderStatus.Pending, order.OrderStatus, PaymentStatus.Unpaid, order.PaymentStatus, "In-store order created");

            return MapToDto(order, "");
            // We need store name? Fetched later or irrelevant for Create return
        }

        [Authorize(ProductSellingPermissions.Orders.Complete)]
        public async Task<OrderDto> CompleteInStorePaymentAsync(Guid orderId, CompleteInStorePaymentDto input)
        {
            var order = await _orderRepository.GetAsync(orderId);
            if (order.OrderType != OrderType.InStore) throw new UserFriendlyException(_localizer["Order:OnlyForInStoreOrders"]);

            await CheckStoreAccessAsync(order.StoreId.Value);

            var cashier = await _userRepository.GetAsync(CurrentUser.Id.Value);
            var oldStatus = order.OrderStatus;
            var oldPayment = order.PaymentStatus;

            // Domain Entity Logic
            order.CompletePaymentInStore(cashier.Id, cashier.Name ?? cashier.UserName, input.PaidAmount);
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
            await _orderHistoryAppService.LogOrderChangeAsync(orderId, oldStatus, order.OrderStatus, oldPayment, order.PaymentStatus, $"Paid In-Store: {input.PaidAmount:C}");

            return MapToDto(order);
        }

        [Authorize(ProductSellingPermissions.Orders.Fulfill)]
        public async Task<OrderDto> FulfillInStoreOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetAsync(orderId);
            if (order.OrderType != OrderType.InStore) throw new UserFriendlyException(_localizer["Order:OnlyForInStoreOrders"]);

            await CheckStoreAccessAsync(order.StoreId.Value);

            var staff = await _userRepository.GetAsync(CurrentUser.Id.Value);

            // Domain Entity Logic
            order.FulfillInStore(staff.Id, staff.Name ?? staff.UserName);
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

        private async Task CheckStoreAccessAsync(Guid storeId)
        {
            // Allow Admin/Manager override
            if (CurrentUser.IsInRole("admin") || CurrentUser.IsInRole("manager")) return;

            var userStoreId = await GetCurrentUserStoreIdAsync();
            if (userStoreId != storeId)
                throw new UserFriendlyException(_localizer["Order:NoStoreAccess"]);
        }

        private async Task<Guid?> GetCurrentUserStoreIdAsync()
        {
            if (CurrentUser.Id == null) return null;
            var user = await _userRepository.GetAsync(CurrentUser.Id.Value);
            return user.GetProperty<Guid?>("AssignedStoreId");
        }

        private OrderDto MapToDto(Order order, string storeName = null)
        {
            var dto = _orderMapper.Map(order);
            dto.StoreName = storeName;
            return dto;
        }
    }
}
