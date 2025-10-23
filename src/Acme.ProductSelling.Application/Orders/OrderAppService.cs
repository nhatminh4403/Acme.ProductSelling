using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.BackgroundJobs.OrderPending;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Hubs;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Payments;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
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
using Volo.Abp.Users;
namespace Acme.ProductSelling.Orders
{
    public class OrderAppService : ApplicationService, IOrderAppService
    {
        private readonly IRepository<Order, Guid> _orderRepository;
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
        public OrderAppService(
            IRepository<Order, Guid> orderRepository,
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
            IDataFilter<ISoftDelete> softDeleteFilter)
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
                var totalCount = await AsyncExecuter.CountAsync(query);
                var items = await AsyncExecuter.ToListAsync(
                    query.OrderBy(input.Sorting ?? "CreationTime DESC").PageBy(input)
                );
                return new PagedResultDto<OrderDto>(
                    totalCount,
                    ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
                );
            }
        }
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
    }
}