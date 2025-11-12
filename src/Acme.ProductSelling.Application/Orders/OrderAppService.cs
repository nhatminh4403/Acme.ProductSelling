using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.BackgroundJobs.OrderPending;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Hubs;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Payments;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Auditing;
using Volo.Abp.Authorization;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Users;
namespace Acme.ProductSelling.Orders
{
    public class OrderAppService : ApplicationService, IOrderAppService
    {
        private readonly IOrderRepository _orderRepository;

        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Cart, Guid> _cartRepository;
        private readonly IHubContext<OrderHub, IOrderClient> _orderHubContext;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IPaymentGatewayResolver _paymentGatewayResolver;
        private readonly IOrderNotificationService _orderNotificationService;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        private readonly IOrderHistoryAppService _orderHistoryAppService;
        private readonly IDataFilter<ISoftDelete> _softDeleteFilter;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IStoreInventoryRepository _storeInventoryRepository;
        public OrderAppService(
            IOrderRepository orderRepository,
            IRepository<Product, Guid> productRepository,
            IGuidGenerator guidGenerator,
            ICurrentUser currentUser,
            IRepository<Cart, Guid> cartRepository,
            IHubContext<OrderHub, IOrderClient> hubContext,
            IBackgroundJobManager backgroundJobManager,
            IPaymentGatewayResolver gatewayResolver,
            IOrderNotificationService orderNotificationService,
            IStringLocalizer<ProductSellingResource> localizer,
            IOrderHistoryAppService orderHistoryAppService,
            IDataFilter<ISoftDelete> softDeleteFilter,
            IIdentityUserRepository userRepository,
            IStoreRepository storeRepository,
            IStoreInventoryRepository storeInventoryRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _guidGenerator = guidGenerator;
            _currentUser = currentUser;
            _cartRepository = cartRepository;
            _orderHubContext = hubContext;
            _backgroundJobManager = backgroundJobManager;
            _paymentGatewayResolver = gatewayResolver;
            _orderNotificationService = orderNotificationService;
            _localizer = localizer;
            _orderHistoryAppService = orderHistoryAppService;
            _softDeleteFilter = softDeleteFilter;
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _storeInventoryRepository = storeInventoryRepository;
        }
        [Authorize]
        public async Task<OrderDto> ConfirmPayPalOrderAsync(Guid guid)
        {
            var customerId = _currentUser.Id;
            var order = await _orderRepository.GetAsync(guid);
            if (customerId == null)
            {
                throw new AbpAuthorizationException(L["Account:UserNotAuthenticated"]);
            }
            if (order == null || order.CustomerId != customerId)
            {
                throw new EntityNotFoundException(typeof(Order), guid);
            }
            if (order.PaymentStatus == PaymentStatus.Pending)
            {
                order.MarkAsPaidOnline();
                await _orderRepository.UpdateAsync(order, autoSave: true);
                Logger.LogInformation("Đã cập nhật trạng thái đơn hàng {OrderId} thành {Status}", order.Id, order.OrderStatus);
                await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
                return ObjectMapper.Map<Order, OrderDto>(order);
            }
            else
            {
                Logger.LogWarning("Bỏ qua xác nhận." +
                      " Đơn hàng {OrderId} không ở trạng thái PendingPayment. " +
                      "Trạng thái hiện tại: {Status}", order.Id, order.OrderStatus);
                return ObjectMapper.Map<Order, OrderDto>(order);
            }
        }
        [Authorize(ProductSellingPermissions.Orders.Default)]
        public async Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListInput input)
        {
            using (input.IncludeDeleted ? _softDeleteFilter.Disable() : null)
            {
                var query = (await _orderRepository.GetQueryableAsync())
                    .Include(o => o.OrderItems);
                IQueryable<Order> filteredQuery = query;

                if (input.StoreId.HasValue)
                {
                    filteredQuery = query.Where(o => o.StoreId == input.StoreId.Value);
                }

                if (input.OrderType.HasValue)
                {
                    filteredQuery = query.Where(o => o.OrderType == input.OrderType.Value);
                }

                if (input.OrderStatus.HasValue)
                {
                    filteredQuery = query.Where(o => o.OrderStatus == input.OrderStatus.Value);
                }

                if (input.PaymentStatus.HasValue)
                {
                    filteredQuery = query.Where(o => o.PaymentStatus == input.PaymentStatus.Value);
                }

                if (input.StartDate.HasValue)
                {
                    filteredQuery = query.Where(o => o.CreationTime >= input.StartDate.Value);
                }
                if (input.EndDate.HasValue)
                {
                    filteredQuery = query.Where(o => o.CreationTime <= input.EndDate.Value);
                }

                var userStoreId = await GetCurrentUserStoreIdAsync();
                if (userStoreId.HasValue && !await IsAdminOrManagerAsync())
                {
                    // Non-admin users can only see orders from their assigned store
                    filteredQuery = query.Where(o => o.StoreId == userStoreId.Value);
                }

                var totalCount = await AsyncExecuter.CountAsync(filteredQuery);
                var items = await AsyncExecuter.ToListAsync(
                    query.OrderBy(input.Sorting ?? "CreationTime DESC").PageBy(input)
                );
                return new PagedResultDto<OrderDto>(
                    totalCount,
                    ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
                );
            }
        }
        [Authorize(ProductSellingPermissions.Orders.Create)]
        public async Task<CreateOrderResultDto> CreateAsync(CreateOrderDto input)
        {
            var customerId = _currentUser.Id.Value;
            var existingPendingOrder = await (await _orderRepository.WithDetailsAsync(o => o.OrderItems))
                .FirstOrDefaultAsync(o =>
                    o.CustomerId == customerId &&
                    o.PaymentStatus == PaymentStatus.Pending
                );
            // B1: Lấy giỏ hàng hiện tại của người dùng
            var cart = await (await _cartRepository.WithDetailsAsync(c => c.Items))
                                  .FirstOrDefaultAsync(c => c.UserId == customerId);
            if (cart == null || !cart.Items.Any())
            {
                throw new UserFriendlyException(L["Cart:ShoppingCartIsEmpty"]);
            }
            Order orderToProcess;
            if (existingPendingOrder != null)
            {
                Logger.LogInformation("Phát hiện đơn hàng {OrderNumber} đang chờ thanh toán. Tái sử dụng...", existingPendingOrder.OrderNumber);
                orderToProcess = existingPendingOrder;
                // Hoàn lại stock cho các sản phẩm trong đơn hàng cũ
                foreach (var oldItem in orderToProcess.OrderItems)
                {
                    var product = await _productRepository.FindAsync(oldItem.ProductId);
                    if (product != null)
                    {
                        product.StockCount += oldItem.Quantity;
                        await _productRepository.UpdateAsync(product);
                    }
                }
                // Xóa tất cả các item cũ khỏi đơn hàng
                orderToProcess.OrderItems.Clear();
                // Cập nhật lại thông tin giao hàng và phương thức thanh toán
                orderToProcess.UpdateShippingInfo(input.CustomerName, input.CustomerPhone, input.ShippingAddress);
                orderToProcess.UpdatePaymentMethod(input.PaymentMethod);
            }
            else
            {
                Logger.LogInformation("Bắt đầu tiến trình tạo đơn hàng mới cho user {UserId}.", customerId);
                orderToProcess = new Order(
                    GuidGenerator.Create(),
                    $"DH-{DateTime.UtcNow:yyyyMMddHHmmss}-{GuidGenerator.Create().ToString("N").Substring(0, 6)}",
                    DateTime.Now, customerId, input.CustomerName, input.CustomerPhone,
                    input.ShippingAddress, input.PaymentMethod
                );
                orderToProcess.SetStatus(OrderStatus.Placed);
            }
            // --- ĐỒNG BỘ ĐƠN HÀNG VỚI GIỎ HÀNG HIỆN TẠI ---
            var productIdsInCart = cart.Items.Select(i => i.ProductId).ToList();
            var productsInDb = (await _productRepository.GetListAsync(p => productIdsInCart.Contains(p.Id)))
                               .ToDictionary(p => p.Id);
            foreach (var cartItem in cart.Items)
            {
                if (productsInDb.TryGetValue(cartItem.ProductId, out var product))
                {
                    if (product.StockCount < cartItem.Quantity)
                    {
                        string ExceptionMessage = L["Product:Stock:NotEnoughStock", product.ProductName, product.StockCount];
                        throw new UserFriendlyException(ExceptionMessage);
                    }
                    orderToProcess.AddOrderItem(product.Id,
                                                    product.ProductName,
                                                    product.DiscountedPrice ?? product.OriginalPrice,
                                                    cartItem.Quantity);
                    product.StockCount -= cartItem.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
            }
            // Tính lại tổng tiền
            orderToProcess.CalculateTotals();
            // FIXED: Đặt trạng thái thanh toán
            if (input.PaymentMethod == PaymentMethods.COD)
            {
                // COD orders stay Unpaid (already set by constructor)
                // No need to call SetPaymentStatus here
                orderToProcess.SetStatus(OrderStatus.Placed);
            }
            else
            {
                // Online payment orders: set to Pending
                orderToProcess.SetPaymentStatus(PaymentStatus.Pending);
                orderToProcess.SetStatus(OrderStatus.Placed);
            }
            // Xử lý qua gateway
            var gateway = _paymentGatewayResolver.Resolve(input.PaymentMethod);
            var gatewayResult = await gateway.ProcessAsync(orderToProcess);
            // Lưu đơn hàng
            await _orderRepository.UpsertAsync(orderToProcess, autoSave: true);
            // Lên lịch job cho COD
            if (orderToProcess.PaymentMethod == PaymentMethods.COD && orderToProcess.OrderStatus == OrderStatus.Placed)
            {
                await _backgroundJobManager.EnqueueAsync<SetOrderBackgroundJobArgs>(
                    new SetOrderBackgroundJobArgs { OrderId = orderToProcess.Id },
                    delay: TimeSpan.FromSeconds(30)
                );
            }
            // Dọn dẹp giỏ hàng
            await _cartRepository.DeleteAsync(cart, autoSave: true);
            // Gửi thông báo
            await _orderNotificationService.NotifyOrderStatusChangeAsync(orderToProcess);
            await _orderHistoryAppService.LogOrderChangeAsync(
                orderToProcess.Id,
                OrderStatus.Pending, // No previous status
                orderToProcess.OrderStatus,
                PaymentStatus.Unpaid,
                orderToProcess.PaymentStatus,
                "Order created"
            );
            return new CreateOrderResultDto
            {
                Order = ObjectMapper.Map<Order, OrderDto>(orderToProcess),
                RedirectUrl = gatewayResult.RedirectUrl
            };
        }
        [Authorize]
        public async Task<OrderDto> GetAsync(Guid id)
        {
            var order = await (await
                            _orderRepository.WithDetailsAsync(o => o.OrderItems))
                             .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                throw new EntityNotFoundException(typeof(Order), id);
            }
            return ObjectMapper.Map<Order, OrderDto>(order);
        }
        public async Task<OrderDto> GetByOrderNumberAsync(string orderNumber)
        {
            //var order = await _orderRepository.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
            var order = await (await
                            _orderRepository.WithDetailsAsync(o => o.OrderItems))
                             .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
            if (order == null)
            {
                throw new UserFriendlyException(L["Order:OrderNotFound"]);
            }
            return ObjectMapper.Map<Order, OrderDto>(order);
        }
        [DisableAuditing]
        [Authorize]
        public async Task<PagedResultDto<OrderDto>>
            GetListForCurrentUserAsync(PagedAndSortedResultRequestDto input)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
            {
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
            return new PagedResultDto<OrderDto>(
                totalCount,
                 ObjectMapper.Map<List<Order>, List<OrderDto>>(orders)
            );
        }
        [Authorize(ProductSellingPermissions.Orders.Edit)]
        public async Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusDto input)
        {
            var order = await _orderRepository.GetAsync(id);
            var oldStatus = order.OrderStatus;
            var oldPaymentStatus = order.PaymentStatus;
            order.SetStatus(input.NewStatus);
            order.SetPaymentStatus(input.NewPaymentStatus);
            await _orderRepository.UpdateAsync(order, autoSave: true);
            await _orderHistoryAppService.LogOrderChangeAsync(
                id,
                oldStatus,
                order.OrderStatus,
                oldPaymentStatus,
                order.PaymentStatus,
                "Status updated by admin"
            );
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            return ObjectMapper.Map<Order, OrderDto>(order);
        }
        [Authorize]
        public async Task DeleteAsync(Guid id)
        {
            var order = await _orderRepository.GetAsync(id, includeDetails: true);
            if (order.CustomerId != CurrentUser.Id) throw new AbpAuthorizationException(L["Account:Unauthorized"]);
            order.CancelByUser(_localizer);
            order.OrderItems.ForEach(async item =>
            {
                var product = await _productRepository.GetAsync(item.ProductId);
                product.StockCount += item.Quantity;
                await _productRepository.UpdateAsync(product, autoSave: true);
            });
            await _orderRepository.UpdateAsync(order);
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
        }
        /// <summary>
        /// NEW: Admin ships an order. For COD orders, sets payment status to PendingOnDelivery
        /// </summary>
        [Authorize(ProductSellingPermissions.Orders.Edit)]
        public async Task ShipOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetAsync(orderId);
            var oldOrderStatus = order.OrderStatus;
            var oldPaymentStatus = order.PaymentStatus;
            // Validate order can be shipped
            if (order.OrderStatus != OrderStatus.Confirmed && order.OrderStatus != OrderStatus.Processing)
            {
                throw new UserFriendlyException(
                    L["Order:CannotShip"],
                    $"Chỉ có thể ship đơn hàng ở trạng thái Confirmed hoặc Processing. Trạng thái hiện tại: {order.OrderStatus}"
                );
            }
            // Update order status to Shipped
            order.SetStatus(OrderStatus.Shipped);
            // For COD orders, update payment status to PendingOnDelivery
            if (order.PaymentMethod == PaymentMethods.COD)
            {
                order.SetPendingOnDelivery(); // ← Uses the new method!
                Logger.LogInformation(
                    "COD Order {OrderId} shipped. Payment status set to PendingOnDelivery",
                    orderId
                );
            }
            else
            {
                // For online payment orders, they should already be paid before shipping
                if (order.PaymentStatus != PaymentStatus.Paid)
                {
                    Logger.LogWarning(
                        "Shipping order {OrderId} with payment method {PaymentMethod} but payment status is {PaymentStatus}",
                        orderId, order.PaymentMethod, order.PaymentStatus
                    );
                }
            }
            await _orderRepository.UpdateAsync(order, autoSave: true);
            await _orderHistoryAppService.LogOrderChangeAsync(
                orderId,
                oldOrderStatus,
                order.OrderStatus,
                oldPaymentStatus,
                order.PaymentStatus,
                "Order shipped"
            );
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            Logger.LogInformation("Order {OrderId} has been shipped", orderId);
        }
        /// <summary>
        /// NEW: Delivery driver delivers order. For COD, admin confirms payment collected
        /// </summary>
        [Authorize(ProductSellingPermissions.Orders.Edit)]
        public async Task DeliverOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetAsync(orderId);
            var oldOrderStatus = order.OrderStatus;
            var oldPaymentStatus = order.PaymentStatus;
            // Validate order can be delivered
            if (order.OrderStatus != OrderStatus.Shipped)
            {
                throw new UserFriendlyException(
                    L["Order:CannotDeliver"],
                    $"Chỉ có thể giao đơn hàng ở trạng thái Shipped. Trạng thái hiện tại: {order.OrderStatus}"
                );
            }
            // For COD orders, mark as paid and delivered
            if (order.PaymentMethod == PaymentMethods.COD)
            {
                order.MarkAsCodPaidAndCompleted(_localizer);
                Logger.LogInformation(
                    "COD Order {OrderId} delivered. Payment collected and confirmed.",
                    orderId
                );
            }
            else
            {
                // For online payment orders, just mark as delivered
                order.SetStatus(OrderStatus.Delivered);
                Logger.LogInformation("Order {OrderId} delivered", orderId);
            }
            await _orderRepository.UpdateAsync(order, autoSave: true);
            // Log the change
            await _orderHistoryAppService.LogOrderChangeAsync(
                orderId,
                oldOrderStatus,
                order.OrderStatus,
                oldPaymentStatus,
                order.PaymentStatus,
                "Order delivered"
            );
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
        }
        [Authorize(ProductSellingPermissions.Orders.ConfirmCodPayment)]
        public async Task MarkAsCodPaidAndCompletedAsync(Guid orderId)
        {
            var order = await _orderRepository.GetAsync(orderId);
            // Gọi phương thức nghiệp vụ trong Entity
            order.MarkAsCodPaidAndCompleted(_localizer);
            // Lưu lại thay đổi
            await _orderRepository.UpdateAsync(order, autoSave: true);
            // Gửi các thông báo cần thiết
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            Logger.LogInformation("Đơn hàng COD {OrderId} đã được xác nhận thanh toán và hoàn thành.", orderId);
        }
        [Authorize(ProductSellingPermissions.Orders.Default)]
        public async Task<PagedResultDto<OrderDto>> GetProfitReportAsync(PagedAndSortedResultRequestDto input)
        {
            var query = (await _orderRepository.GetQueryableAsync())
                .Include(o => o.OrderItems)
                .Where(o => o.OrderStatus == OrderStatus.Delivered && o.PaymentStatus == PaymentStatus.Paid);
            var totalCount = await AsyncExecuter.CountAsync(query);
            var items = await AsyncExecuter.
                ToListAsync(query.OrderBy(input.Sorting ?? "CreationTime DESC").PageBy(input));
            return new PagedResultDto<OrderDto>(
                totalCount,
                ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
            );
        }
        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(Guid orderId)
        {
            return await _orderHistoryAppService.GetOrderHistoryAsync(orderId);
        }

        [Authorize(ProductSellingPermissions.Orders.Default)]
        public async Task<PagedResultDto<OrderDto>> GetDeletedOrdersAsync(PagedAndSortedResultRequestDto input)
        {
            var query = (await _orderRepository.GetQueryableAsync())
                        .IgnoreQueryFilters()
                        .Where(o => o.IsDeleted)
                        .Include(o => o.OrderItems);

            var totalCount = await AsyncExecuter.CountAsync(query);
            var items = await AsyncExecuter.ToListAsync(
                query.OrderBy(input.Sorting ?? "DeletionTime DESC").PageBy(input)
            );

            return new PagedResultDto<OrderDto>(
                totalCount,
                ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
            );
        }

        [Authorize(ProductSellingPermissions.Orders.Default)]
        public async Task RestoreOrderAsync(Guid orderId)
        {
            var query = await _orderRepository.GetQueryableAsync();

            var order = await query.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || !order.IsDeleted)
            {
                throw new UserFriendlyException(L["Order:NotFoundOrNotDeleted"]);
            }

            order.IsDeleted = false;
            order.DeletionTime = null;
            order.DeleterId = null;

            await _orderRepository.UpdateAsync(order, autoSave: true);

            Logger.LogInformation("Order {OrderId} restored by admin {AdminId}", orderId, CurrentUser.Id);
        }
        [Authorize(ProductSellingPermissions.Orders.Create)]
        public async Task<OrderDto> CreateInStoreOrderAsync(CreateInStoreOrderDto input)
        {
            var userStoreId = await GetCurrentUserStoreIdAsync();
            if (!userStoreId.HasValue)
            {
                throw new UserFriendlyException(
                    _localizer["Order:UserNotAssignedToStore"],
                    "Người dùng phải được gán vào một cửa hàng để tạo đơn hàng."
                );
            }

            // Get current user info (seller)
            var currentUser = await _userRepository.GetAsync(_currentUser.Id.Value);
            var store = await _storeRepository.GetAsync(userStoreId.Value);

            // Generate order number for in-store order
            var orderNumber = await GenerateInStoreOrderNumberAsync(userStoreId.Value);

            // Create in-store order
            var order = new Order(
                _guidGenerator.Create(),
                orderNumber,
                DateTime.Now,
                userStoreId.Value,
                currentUser.Id,
                currentUser.Name ?? currentUser.UserName,
                input.CustomerName,
                input.CustomerPhone,
                input.PaymentMethod
            );

            // Get products and validate stock
            var productIds = input.Items.Select(i => i.ProductId).ToList();
            var products = (await _productRepository.GetListAsync(p => productIds.Contains(p.Id)))
                          .ToDictionary(p => p.Id);

            // Add order items
            foreach (var itemDto in input.Items)
            {
                if (!products.TryGetValue(itemDto.ProductId, out var product))
                {
                    throw new UserFriendlyException(
                        _localizer["Product:NotFound"],
                        $"Không tìm thấy sản phẩm với ID: {itemDto.ProductId}"
                    );
                }

                if (!product.IsActive)
                {
                    throw new UserFriendlyException(
                        _localizer["Product:NotActive"],
                        $"Sản phẩm '{product.ProductName}' không còn hoạt động."
                    );
                }
                if (product.ReleaseDate.HasValue && product.ReleaseDate.Value > DateTime.Now)
                {
                    throw new UserFriendlyException(
                        _localizer["Product:NotYetReleased"],
                        $"Sản phẩm '{product.ProductName}' sẽ có sẵn từ {product.ReleaseDate.Value:dd/MM/yyyy}."
                    );
                }
                // NEW: Check store-specific inventory
                var hasStock = await _storeInventoryRepository.HasSufficientStockAsync(
                    userStoreId.Value,
                    product.Id,
                    itemDto.Quantity
                );

                if (!hasStock)
                {
                    var availableStock = await GetAvailableStockAsync(userStoreId.Value, product.Id);
                    throw new UserFriendlyException(
                        _localizer["Product:Stock:NotEnoughStockInStore", product.ProductName, store.Name, availableStock]
                    );
                }

                // Add item to order
                order.AddOrderItem(
                    product.Id,
                    product.ProductName,
                    product.DiscountedPrice ?? product.OriginalPrice,
                    itemDto.Quantity
                );

                // NEW: Reduce store-specific inventory
                var storeInventory = await _storeInventoryRepository.GetByStoreAndProductAsync(
                    userStoreId.Value,
                    product.Id
                );
                storeInventory.RemoveStock(itemDto.Quantity);
                await _storeInventoryRepository.UpdateAsync(storeInventory);
            }

            // Calculate totals
            order.CalculateTotals();

            // Save order
            await _orderRepository.InsertAsync(order, autoSave: true);

            // Log order creation
            await _orderHistoryAppService.LogOrderChangeAsync(
                order.Id,
                OrderStatus.Pending,
                order.OrderStatus,
                PaymentStatus.Unpaid,
                order.PaymentStatus,
                "In-store order created"
            );

            // Notify via SignalR
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);

            Logger.LogInformation(
                "In-store order {OrderNumber} created at store {StoreId} by seller {SellerId}",
                order.OrderNumber, userStoreId.Value, currentUser.Id
            );

            return await MapToOrderDtoAsync(order);
        }

        [Authorize(ProductSellingPermissions.Orders.Complete)]
        public async Task<OrderDto> CompleteInStorePaymentAsync(Guid orderId, CompleteInStorePaymentDto input)
        {
            var order = await _orderRepository.GetAsync(orderId);

            // Validate order type
            if (order.OrderType != OrderType.InStore)
            {
                throw new UserFriendlyException(
                    _localizer["Order:OnlyForInStoreOrders"],
                    "Phương thức này chỉ dành cho đơn hàng tại cửa hàng."
                );
            }

            // Check store access
            await CheckStoreAccessAsync(order.StoreId.Value);

            // Get current user (cashier)
            var currentUser = await _userRepository.GetAsync(_currentUser.Id.Value);

            var oldOrderStatus = order.OrderStatus;
            var oldPaymentStatus = order.PaymentStatus;

            // Complete payment
            order.CompletePaymentInStore(
                currentUser.Id,
                currentUser.Name ?? currentUser.UserName,
                input.PaidAmount
            );

            await _orderRepository.UpdateAsync(order, autoSave: true);

            // Log the change
            await _orderHistoryAppService.LogOrderChangeAsync(
                orderId,
                oldOrderStatus,
                order.OrderStatus,
                oldPaymentStatus,
                order.PaymentStatus,
                $"Payment completed by cashier. Amount: {input.PaidAmount:C}"
            );

            // Notify via SignalR
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);

            Logger.LogInformation(
                "In-store order {OrderId} payment completed by cashier {CashierId}",
                orderId, currentUser.Id
            );

            return await MapToOrderDtoAsync(order);
        }


        private async Task<int> GetAvailableStockAsync(Guid storeId, Guid productId)
        {
            var inventory = await _storeInventoryRepository.GetByStoreAndProductAsync(storeId, productId);
            return inventory?.Quantity ?? 0;
        }
        [Authorize(ProductSellingPermissions.Orders.Fulfill)]
        public async Task<OrderDto> FulfillInStoreOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetAsync(orderId);

            // Validate order type
            if (order.OrderType != OrderType.InStore)
            {
                throw new UserFriendlyException(
                    _localizer["Order:OnlyForInStoreOrders"],
                    "Phương thức này chỉ dành cho đơn hàng tại cửa hàng."
                );
            }

            // Check store access
            await CheckStoreAccessAsync(order.StoreId.Value);

            // Get current user (warehouse staff)
            var currentUser = await _userRepository.GetAsync(_currentUser.Id.Value);

            var oldOrderStatus = order.OrderStatus;

            // Fulfill order
            order.FulfillInStore(
                currentUser.Id,
                currentUser.Name ?? currentUser.UserName
            );

            await _orderRepository.UpdateAsync(order, autoSave: true);

            // Log the change
            await _orderHistoryAppService.LogOrderChangeAsync(
                orderId,
                oldOrderStatus,
                order.OrderStatus,
                order.PaymentStatus,
                order.PaymentStatus,
                "Order fulfilled - items given to customer"
            );

            // Notify via SignalR
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);

            Logger.LogInformation(
                "In-store order {OrderId} fulfilled by warehouse staff {FulfillerId}",
                orderId, currentUser.Id
            );

            return await MapToOrderDtoAsync(order);
        }

        [Authorize]
        public async Task<PagedResultDto<OrderDto>> GetStoreOrdersAsync(GetOrderListInput input)
        {
            var userStoreId = await GetCurrentUserStoreIdAsync();

            if (!userStoreId.HasValue)
            {
                throw new UserFriendlyException(
                    _localizer["Order:UserNotAssignedToStore"],
                    "Người dùng phải được gán vào một cửa hàng."
                );
            }

            // Override store filter with user's assigned store (unless admin/manager)
            if (!await IsAdminOrManagerAsync())
            {
                input.StoreId = userStoreId.Value;
            }

            return await GetListAsync(input);
        }


        private async Task<Guid?> GetCurrentUserStoreIdAsync()
        {
            if (!_currentUser.Id.HasValue)
                return null;

            var user = await _userRepository.GetAsync(_currentUser.Id.Value);

            // Get AssignedStoreId from extra properties
            var storeIdProperty = user.GetProperty<Guid?>("AssignedStoreId");
            return storeIdProperty;
        }

        private async Task<bool> IsAdminOrManagerAsync()
        {
            if (!_currentUser.Id.HasValue)
                return false;

            return await IsInRoleAsync("admin") || await IsInRoleAsync("manager");
        }

        private async Task<bool> IsInRoleAsync(string roleName)
        {
            if (!_currentUser.Id.HasValue)
                return false;

            var user = await _userRepository.GetAsync(_currentUser.Id.Value);
            var roles = await _userRepository.GetRolesAsync(user.Id);
            return roles.Any(r => string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));
        }

        private async Task CheckStoreAccessAsync(Guid storeId)
        {
            // Admins and managers can access all stores
            if (await IsAdminOrManagerAsync())
            {
                return;
            }

            // Other users can only access their assigned store
            var userStoreId = await GetCurrentUserStoreIdAsync();
            if (!userStoreId.HasValue || userStoreId.Value != storeId)
            {
                throw new UserFriendlyException(
                    _localizer["Order:NoStoreAccess"],
                    "Bạn không có quyền truy cập dữ liệu của cửa hàng này."
                );
            }
        }
        private async Task<string> GenerateInStoreOrderNumberAsync(Guid storeId)
        {
            var store = await _storeRepository.GetAsync(storeId);
            var date = DateTime.Now.ToString("yyyyMMdd");

            // Get today's order count for this store
            var todayStart = DateTime.Today;
            var todayEnd = todayStart.AddDays(1);

            var query = await _orderRepository.GetQueryableAsync();
            var todayOrderCount = await AsyncExecuter.CountAsync(
                query.Where(x => x.StoreId == storeId &&
                    x.OrderType == OrderType.InStore &&
                    x.CreationTime >= todayStart &&
                    x.CreationTime < todayEnd)
            );

            var sequence = (todayOrderCount + 1).ToString("D4");
            return $"ST-{store.Code}-{date}-{sequence}";
        }

        private async Task<OrderDto> MapToOrderDtoAsync(Order order)
        {
            var dto = ObjectMapper.Map<Order, OrderDto>(order);

            // Map store name if order has a store
            if (order.StoreId.HasValue)
            {
                var store = await _storeRepository.GetAsync(order.StoreId.Value);
                dto.StoreName = store.Name;
            }

            return dto;
        }
    }
}