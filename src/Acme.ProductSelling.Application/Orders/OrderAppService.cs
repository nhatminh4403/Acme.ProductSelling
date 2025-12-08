using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.BackgroundJobs.OrderPending;
using Acme.ProductSelling.Orders.Dtos;
using Acme.ProductSelling.Orders.Hubs;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Payments;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.StoreInventories;
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
        private readonly OrderToOrderDtoMapper _orderMapper;
        //private readonly OrderListMapper _orderListMapper;
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
            IStoreInventoryRepository storeInventoryRepository,
            OrderToOrderDtoMapper orderMapper)
            //OrderListMapper orderListMapper)
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
            _orderMapper = orderMapper;
            //_orderListMapper = orderListMapper;
        }

        [Authorize]
        public async Task<OrderDto> ConfirmPayPalOrderAsync(Guid guid)
        {
            Logger.LogInformation("[ConfirmPayPal] START - OrderId: {OrderId}, UserId: {UserId}", guid, _currentUser.Id);

            var customerId = _currentUser.Id;
            var order = await _orderRepository.GetAsync(guid);

            Logger.LogDebug("[ConfirmPayPal] Order retrieved - OrderNumber: {OrderNumber}, PaymentStatus: {PaymentStatus}, OrderStatus: {OrderStatus}",
                order.OrderNumber, order.PaymentStatus, order.OrderStatus);

            if (customerId == null)
            {
                Logger.LogWarning("[ConfirmPayPal] FAILED - User not authenticated. OrderId: {OrderId}", guid);
                throw new AbpAuthorizationException(L["Account:UserNotAuthenticated"]);
            }

            if (order == null || order.CustomerId != customerId)
            {
                Logger.LogWarning("[ConfirmPayPal] FAILED - Order not found or unauthorized. OrderId: {OrderId}, CustomerId: {CustomerId}, RequestUserId: {RequestUserId}",
                    guid, order?.CustomerId, customerId);
                throw new EntityNotFoundException(typeof(Order), guid);
            }

            if (order.PaymentStatus == PaymentStatus.Pending)
            {
                Logger.LogInformation("[ConfirmPayPal] Marking order as paid - OrderId: {OrderId}, OrderNumber: {OrderNumber}",
                    order.Id, order.OrderNumber);

                order.MarkAsPaidOnline();
                await _orderRepository.UpdateAsync(order, autoSave: true);

                Logger.LogInformation("[ConfirmPayPal] Order updated successfully - OrderId: {OrderId}, NewStatus: {OrderStatus}, NewPaymentStatus: {PaymentStatus}",
                    order.Id, order.OrderStatus, order.PaymentStatus);

                await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
                Logger.LogDebug("[ConfirmPayPal] Notification sent for OrderId: {OrderId}", order.Id);

                return ObjectMapper.Map<Order, OrderDto>(order);
            }
            else
            {
                Logger.LogWarning("[ConfirmPayPal] SKIP - Order not in Pending state. OrderId: {OrderId}, CurrentStatus: {PaymentStatus}",
                    order.Id, order.PaymentStatus);
                return _orderMapper.Map(order);
            }
        }

        [Authorize(ProductSellingPermissions.Orders.Default)]
        public async Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListInput input)
        {
            Logger.LogInformation("[GetList] START - UserId: {UserId}, Filters: StoreId={StoreId}, OrderType={OrderType}, OrderStatus={OrderStatus}, PaymentStatus={PaymentStatus}, DateRange={StartDate}-{EndDate}, IncludeDeleted={IncludeDeleted}",
                _currentUser.Id, input.StoreId, input.OrderType, input.OrderStatus, input.PaymentStatus, input.StartDate, input.EndDate, input.IncludeDeleted);

            using (input.IncludeDeleted ? _softDeleteFilter.Disable() : null)
            {
                var query = (await _orderRepository.GetQueryableAsync())
                    .Include(o => o.OrderItems);

                IQueryable<Order> filteredQuery = query;
                var filterCount = 0;

                if (input.StoreId.HasValue)
                {
                    filteredQuery = query.Where(o => o.StoreId == input.StoreId.Value);
                    filterCount++;
                    Logger.LogDebug("[GetList] Applied StoreId filter: {StoreId}", input.StoreId.Value);
                }

                if (input.OrderType.HasValue)
                {
                    filteredQuery = query.Where(o => o.OrderType == input.OrderType.Value);
                    filterCount++;
                    Logger.LogDebug("[GetList] Applied OrderType filter: {OrderType}", input.OrderType.Value);
                }

                if (input.OrderStatus.HasValue)
                {
                    filteredQuery = query.Where(o => o.OrderStatus == input.OrderStatus.Value);
                    filterCount++;
                    Logger.LogDebug("[GetList] Applied OrderStatus filter: {OrderStatus}", input.OrderStatus.Value);
                }

                if (input.PaymentStatus.HasValue)
                {
                    filteredQuery = query.Where(o => o.PaymentStatus == input.PaymentStatus.Value);
                    filterCount++;
                    Logger.LogDebug("[GetList] Applied PaymentStatus filter: {PaymentStatus}", input.PaymentStatus.Value);
                }

                if (input.StartDate.HasValue)
                {
                    filteredQuery = query.Where(o => o.CreationTime >= input.StartDate.Value);
                    filterCount++;
                    Logger.LogDebug("[GetList] Applied StartDate filter: {StartDate}", input.StartDate.Value);
                }

                if (input.EndDate.HasValue)
                {
                    filteredQuery = query.Where(o => o.CreationTime <= input.EndDate.Value);
                    filterCount++;
                    Logger.LogDebug("[GetList] Applied EndDate filter: {EndDate}", input.EndDate.Value);
                }

                var userStoreId = await GetCurrentUserStoreIdAsync();
                var isAdmin = await IsAdminOrManagerAsync();

                Logger.LogDebug("[GetList] User access check - UserStoreId: {UserStoreId}, IsAdminOrManager: {IsAdmin}",
                    userStoreId, isAdmin);

                if (userStoreId.HasValue && !isAdmin)
                {
                    filteredQuery = query.Where(o => o.StoreId == userStoreId.Value);
                    filterCount++;
                    Logger.LogInformation("[GetList] Non-admin user restricted to store: {StoreId}", userStoreId.Value);
                }

                var totalCount = await AsyncExecuter.CountAsync(filteredQuery);
                Logger.LogInformation("[GetList] Query executed - TotalCount: {TotalCount}, FiltersApplied: {FilterCount}",
                    totalCount, filterCount);

                var items = await AsyncExecuter.ToListAsync(
                    query.OrderBy(input.Sorting ?? "CreationTime DESC").PageBy(input)
                );

                Logger.LogInformation("[GetList] COMPLETED - Returned {ItemCount} items out of {TotalCount} total",
                    items.Count, totalCount);

                return new PagedResultDto<OrderDto>(
                    totalCount,
                    _orderMapper.MapList(items)
                );
            }
        }

        [Authorize(ProductSellingPermissions.Orders.Create)]
        public async Task<CreateOrderResultDto> CreateAsync(CreateOrderDto input)
        {
            Logger.LogInformation("[CreateOrder] START - UserId: {UserId}, PaymentMethod: {PaymentMethod}, CustomerName: {CustomerName}",
                _currentUser.Id, input.PaymentMethod, input.CustomerName);

            var customerId = _currentUser.Id.Value;

            var existingPendingOrder = await (await _orderRepository.WithDetailsAsync(o => o.OrderItems))
                .FirstOrDefaultAsync(o =>
                    o.CustomerId == customerId &&
                    o.PaymentStatus == PaymentStatus.Pending
                );

            if (existingPendingOrder != null)
            {
                Logger.LogInformation("[CreateOrder] Found existing pending order - OrderId: {OrderId}, OrderNumber: {OrderNumber}, ItemCount: {ItemCount}",
                    existingPendingOrder.Id, existingPendingOrder.OrderNumber, existingPendingOrder.OrderItems.Count);
            }

            var cart = await (await _cartRepository.WithDetailsAsync(c => c.Items))
                              .FirstOrDefaultAsync(c => c.UserId == customerId);

            if (cart == null || !cart.Items.Any())
            {
                Logger.LogWarning("[CreateOrder] FAILED - Cart is empty. UserId: {UserId}", customerId);
                throw new UserFriendlyException(L["Cart:IsEmpty"]);
            }

            Logger.LogInformation("[CreateOrder] Cart retrieved - CartId: {CartId}, ItemCount: {ItemCount}, TotalItems: {TotalQuantity}",
                cart.Id, cart.Items.Count, cart.Items.Sum(i => i.Quantity));

            Order orderToProcess;

            if (existingPendingOrder != null)
            {
                Logger.LogInformation("[CreateOrder] Reusing pending order - Restoring stock for {ItemCount} items",
                    existingPendingOrder.OrderItems.Count);

                orderToProcess = existingPendingOrder;

                foreach (var oldItem in orderToProcess.OrderItems)
                {
                    var product = await _productRepository.FindAsync(oldItem.ProductId);
                    if (product != null)
                    {
                        var oldStock = product.StockCount;
                        product.StockCount += oldItem.Quantity;
                        await _productRepository.UpdateAsync(product);
                        Logger.LogDebug("[CreateOrder] Stock restored - ProductId: {ProductId}, OldStock: {OldStock}, NewStock: {NewStock}, RestoredQty: {Quantity}",
                            product.Id, oldStock, product.StockCount, oldItem.Quantity);
                    }
                }

                orderToProcess.OrderItems.Clear();
                orderToProcess.UpdateShippingInfo(input.CustomerName, input.CustomerPhone, input.ShippingAddress);
                orderToProcess.UpdatePaymentMethod(input.PaymentMethod);

                Logger.LogInformation("[CreateOrder] Pending order updated - OrderId: {OrderId}", orderToProcess.Id);
            }
            else
            {
                var orderNumber = $"DH-{DateTime.UtcNow:yyyyMMddHHmmss}-{GuidGenerator.Create().ToString("N").Substring(0, 6)}";
                Logger.LogInformation("[CreateOrder] Creating new order - OrderNumber: {OrderNumber}", orderNumber);

                orderToProcess = new Order(
                    GuidGenerator.Create(),
                    orderNumber,
                    DateTime.Now, customerId, input.CustomerName, input.CustomerPhone,
                    input.ShippingAddress, input.PaymentMethod
                );
                orderToProcess.SetStatus(OrderStatus.Placed);

                Logger.LogInformation("[CreateOrder] New order created - OrderId: {OrderId}, OrderNumber: {OrderNumber}",
                    orderToProcess.Id, orderToProcess.OrderNumber);
            }

            var productIdsInCart = cart.Items.Select(i => i.ProductId).ToList();
            var productsInDb = (await _productRepository.GetListAsync(p => productIdsInCart.Contains(p.Id)))
                               .ToDictionary(p => p.Id);

            Logger.LogDebug("[CreateOrder] Products loaded - RequestedCount: {RequestedCount}, FoundCount: {FoundCount}",
                productIdsInCart.Count, productsInDb.Count);

            var itemsProcessed = 0;
            foreach (var cartItem in cart.Items)
            {
                if (productsInDb.TryGetValue(cartItem.ProductId, out var product))
                {
                    if (product.StockCount < cartItem.Quantity)
                    {
                        Logger.LogWarning("[CreateOrder] FAILED - Insufficient stock. ProductId: {ProductId}, ProductName: {ProductName}, Required: {Required}, Available: {Available}",
                            product.Id, product.ProductName, cartItem.Quantity, product.StockCount);

                        string ExceptionMessage = L["Product:Stock:NotEnoughStock", product.ProductName, product.StockCount];
                        throw new UserFriendlyException(ExceptionMessage);
                    }

                    orderToProcess.AddOrderItem(product.Id,
                                                product.ProductName,
                                                product.DiscountedPrice ?? product.OriginalPrice,
                                                cartItem.Quantity);

                    var oldStock = product.StockCount;
                    product.StockCount -= cartItem.Quantity;
                    await _productRepository.UpdateAsync(product);
                    itemsProcessed++;

                    Logger.LogDebug("[CreateOrder] Item added - ProductId: {ProductId}, ProductName: {ProductName}, Quantity: {Quantity}, Price: {Price}, OldStock: {OldStock}, NewStock: {NewStock}",
                        product.Id, product.ProductName, cartItem.Quantity, product.DiscountedPrice ?? product.OriginalPrice, oldStock, product.StockCount);
                }
                else
                {
                    Logger.LogWarning("[CreateOrder] Product not found in database - ProductId: {ProductId}", cartItem.ProductId);
                }
            }

            Logger.LogInformation("[CreateOrder] Items processed - Total: {ItemsProcessed}, TotalQuantity: {TotalQuantity}",
                itemsProcessed, orderToProcess.OrderItems.Sum(i => i.Quantity));

            orderToProcess.CalculateTotals();
            Logger.LogInformation("[CreateOrder] Totals calculated  Total: {Total}",
                orderToProcess.TotalAmount);

            if (input.PaymentMethod == PaymentMethods.COD)
            {
                orderToProcess.SetStatus(OrderStatus.Placed);
                Logger.LogInformation("[CreateOrder] COD order - Status set to Placed");
            }
            else
            {
                if (orderToProcess.PaymentStatus != PaymentStatus.Pending)
                {
                    orderToProcess.SetPaymentStatus(PaymentStatus.Pending);
                    Logger.LogInformation("[CreateOrder] Online payment - PaymentStatus set to Pending");
                }
                else
                {
                    Logger.LogInformation("[CreateOrder] Online payment - PaymentStatus already Pending, skipping status change");
                }
                orderToProcess.SetStatus(OrderStatus.Placed);
                Logger.LogInformation("[CreateOrder] Online payment - Status set to Placed, PaymentStatus set to Pending");
            }

            var gateway = _paymentGatewayResolver.Resolve(input.PaymentMethod);
            Logger.LogDebug("[CreateOrder] Payment gateway resolved - PaymentMethod: {PaymentMethod}, Gateway: {Gateway}",
                input.PaymentMethod, gateway.GetType().Name);

            var gatewayResult = await gateway.ProcessAsync(orderToProcess);
            Logger.LogInformation("[CreateOrder] Payment gateway processed - RedirectUrl: {RedirectUrl}",
                gatewayResult.RedirectUrl ?? "None");

            await _orderRepository.UpsertAsync(orderToProcess, autoSave: true);
            Logger.LogInformation("[CreateOrder] Order saved - OrderId: {OrderId}, OrderNumber: {OrderNumber}",
                orderToProcess.Id, orderToProcess.OrderNumber);

            if (orderToProcess.PaymentMethod == PaymentMethods.COD && orderToProcess.OrderStatus == OrderStatus.Placed)
            {
                await _backgroundJobManager.EnqueueAsync<SetOrderBackgroundJobArgs>(
                    new SetOrderBackgroundJobArgs { OrderId = orderToProcess.Id },
                    //delay: TimeSpan.FromSeconds(30)\
                    delay: TimeSpan.FromHours(1)
                );
                Logger.LogInformation("[CreateOrder] Background job scheduled - OrderId: {OrderId}, Delay: 30s",
                    orderToProcess.Id);
            }

            await _cartRepository.DeleteAsync(cart, autoSave: true);
            Logger.LogInformation("[CreateOrder] Cart deleted - CartId: {CartId}", cart.Id);

            await _orderNotificationService.NotifyOrderStatusChangeAsync(orderToProcess);
            Logger.LogDebug("[CreateOrder] Notification sent");

            await _orderHistoryAppService.LogOrderChangeAsync(
                orderToProcess.Id,
                OrderStatus.Pending,
                orderToProcess.OrderStatus,
                PaymentStatus.Unpaid,
                orderToProcess.PaymentStatus,
                //"Order created"
                _localizer["Order:Created"]
            );
            Logger.LogDebug("[CreateOrder] Order history logged");

            Logger.LogInformation("[CreateOrder] COMPLETED - OrderId: {OrderId}, OrderNumber: {OrderNumber}, TotalAmount: {TotalAmount}",
                orderToProcess.Id, orderToProcess.OrderNumber, orderToProcess.TotalAmount);

            return new CreateOrderResultDto
            {
                Order = _orderMapper.Map(orderToProcess),
                RedirectUrl = gatewayResult.RedirectUrl
            };
        }

        [Authorize]
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

        [Authorize(ProductSellingPermissions.Orders.Edit)]
        public async Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusDto input)
        {
            Logger.LogInformation("[UpdateStatus] START - OrderId: {OrderId}, NewOrderStatus: {NewOrderStatus}, NewPaymentStatus: {NewPaymentStatus}, UserId: {UserId}",
                id, input.NewStatus, input.NewPaymentStatus, _currentUser.Id);

            var order = await _orderRepository.GetAsync(id);
            var oldStatus = order.OrderStatus;
            var oldPaymentStatus = order.PaymentStatus;

            Logger.LogDebug("[UpdateStatus] Current state - OrderStatus: {OldOrderStatus}, PaymentStatus: {OldPaymentStatus}",
                oldStatus, oldPaymentStatus);

            order.SetStatus(input.NewStatus);
            order.SetPaymentStatus(input.NewPaymentStatus);

            await _orderRepository.UpdateAsync(order, autoSave: true);

            Logger.LogInformation("[UpdateStatus] Order updated - OrderId: {OrderId}, OldStatus: {OldStatus} -> NewStatus: {NewStatus}, OldPaymentStatus: {OldPaymentStatus} -> NewPaymentStatus: {NewPaymentStatus}",
                id, oldStatus, input.NewStatus, oldPaymentStatus, input.NewPaymentStatus);

            await _orderHistoryAppService.LogOrderChangeAsync(
                id,
                oldStatus,
                order.OrderStatus,
                oldPaymentStatus,
                order.PaymentStatus,
                "Status updated by admin"
            );

            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            Logger.LogDebug("[UpdateStatus] Notification sent");

            Logger.LogInformation("[UpdateStatus] COMPLETED - OrderId: {OrderId}", id);

            return _orderMapper.Map(order);
        }

        [Authorize(ProductSellingPermissions.Orders.Edit)]
        public async Task ShipOrderAsync(Guid orderId)
        {
            Logger.LogInformation("[ShipOrder] START - OrderId: {OrderId}, UserId: {UserId}", orderId, _currentUser.Id);

            var order = await _orderRepository.GetAsync(orderId);
            var oldOrderStatus = order.OrderStatus;
            var oldPaymentStatus = order.PaymentStatus;

            Logger.LogDebug("[ShipOrder] Order loaded - OrderNumber: {OrderNumber}, CurrentStatus: {CurrentStatus}, PaymentMethod: {PaymentMethod}, PaymentStatus: {PaymentStatus}",
                order.OrderNumber, order.OrderStatus, order.PaymentMethod, order.PaymentStatus);

            if (order.OrderStatus != OrderStatus.Confirmed && order.OrderStatus != OrderStatus.Processing)
            {
                Logger.LogWarning("[ShipOrder] FAILED - Invalid order status. OrderId: {OrderId}, CurrentStatus: {CurrentStatus}, AllowedStatuses: [Confirmed, Processing]",
                    orderId, order.OrderStatus);
                throw new UserFriendlyException(
                    L["Order:CannotShip"],
                    $"Chỉ có thể ship đơn hàng ở trạng thái Confirmed hoặc Processing. Trạng thái hiện tại: {order.OrderStatus}"
                );
            }

            order.SetStatus(OrderStatus.Shipped);

            if (order.PaymentMethod == PaymentMethods.COD)
            {
                order.SetPendingOnDelivery();
                Logger.LogInformation("[ShipOrder] COD order - PaymentStatus set to PendingOnDelivery. OrderId: {OrderId}", orderId);
            }
            else
            {
                if (order.PaymentStatus != PaymentStatus.Paid)
                {
                    Logger.LogWarning("[ShipOrder] WARNING - Shipping non-COD order without paid status. OrderId: {OrderId}, PaymentMethod: {PaymentMethod}, PaymentStatus: {PaymentStatus}",
                        orderId, order.PaymentMethod, order.PaymentStatus);
                }
            }

            await _orderRepository.UpdateAsync(order, autoSave: true);

            Logger.LogInformation("[ShipOrder] Order updated - OrderId: {OrderId}, OldStatus: {OldStatus} -> NewStatus: Shipped, OldPaymentStatus: {OldPaymentStatus} -> NewPaymentStatus: {NewPaymentStatus}",
                orderId, oldOrderStatus, oldPaymentStatus, order.PaymentStatus);

            await _orderHistoryAppService.LogOrderChangeAsync(
                orderId,
                oldOrderStatus,
                order.OrderStatus,
                oldPaymentStatus,
                order.PaymentStatus,
                "Order shipped"
            );

            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            Logger.LogDebug("[ShipOrder] Notification sent");

            Logger.LogInformation("[ShipOrder] COMPLETED - OrderId: {OrderId}, OrderNumber: {OrderNumber}", orderId, order.OrderNumber);
        }

        [Authorize(ProductSellingPermissions.Orders.Edit)]
        public async Task DeliverOrderAsync(Guid orderId)
        {
            Logger.LogInformation("[DeliverOrder] START - OrderId: {OrderId}, UserId: {UserId}", orderId, _currentUser.Id);

            var order = await _orderRepository.GetAsync(orderId);
            var oldOrderStatus = order.OrderStatus;
            var oldPaymentStatus = order.PaymentStatus;

            Logger.LogDebug("[DeliverOrder] Order loaded - OrderNumber: {OrderNumber}, CurrentStatus: {CurrentStatus}, PaymentMethod: {PaymentMethod}, PaymentStatus: {PaymentStatus}",
                order.OrderNumber, order.OrderStatus, order.PaymentMethod, order.PaymentStatus);

            if (order.OrderStatus != OrderStatus.Shipped)
            {
                Logger.LogWarning("[DeliverOrder] FAILED - Invalid order status. OrderId: {OrderId}, CurrentStatus: {CurrentStatus}, RequiredStatus: Shipped",
                    orderId, order.OrderStatus);
                throw new UserFriendlyException(
                    L["Order:CannotDeliver"],
                    $"Chỉ có thể giao đơn hàng ở trạng thái Shipped. Trạng thái hiện tại: {order.OrderStatus}"
                );
            }

            if (order.PaymentMethod == PaymentMethods.COD)
            {
                order.MarkAsCodPaidAndCompleted(_localizer);
                Logger.LogInformation("[DeliverOrder] COD order completed - OrderId: {OrderId}, Payment collected and confirmed", orderId);
            }
            else
            {
                order.SetStatus(OrderStatus.Delivered);
                Logger.LogInformation("[DeliverOrder] Online payment order delivered - OrderId: {OrderId}", orderId);
            }

            await _orderRepository.UpdateAsync(order, autoSave: true);

            Logger.LogInformation("[DeliverOrder] Order updated - OrderId: {OrderId}, OldStatus: {OldStatus} -> NewStatus: {NewStatus}, OldPaymentStatus: {OldPaymentStatus} -> NewPaymentStatus: {NewPaymentStatus}",
                orderId, oldOrderStatus, order.OrderStatus, oldPaymentStatus, order.PaymentStatus);

            await _orderHistoryAppService.LogOrderChangeAsync(
                orderId,
                oldOrderStatus,
                order.OrderStatus,
                oldPaymentStatus,
                order.PaymentStatus,
                "Order delivered"
            );

            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            Logger.LogDebug("[DeliverOrder] Notification sent");

            Logger.LogInformation("[DeliverOrder] COMPLETED - OrderId: {OrderId}, OrderNumber: {OrderNumber}", orderId, order.OrderNumber);
        }

        [Authorize(ProductSellingPermissions.Orders.Create)]
        public async Task<OrderDto> CreateInStoreOrderAsync(CreateInStoreOrderDto input)
        {
            Logger.LogInformation("[CreateInStoreOrder] START - UserId: {UserId}, CustomerName: {CustomerName}, PaymentMethod: {PaymentMethod}, ItemCount: {ItemCount}",
                _currentUser.Id, input.CustomerName, input.PaymentMethod, input.Items.Count);

            var userStoreId = await GetCurrentUserStoreIdAsync();
            if (!userStoreId.HasValue)
            {
                Logger.LogWarning("[CreateInStoreOrder] FAILED - User not assigned to store. UserId: {UserId}", _currentUser.Id);
                throw new UserFriendlyException(
                    _localizer["Order:UserNotAssignedToStore"],
                    "Người dùng phải được gán vào một cửa hàng để tạo đơn hàng."
                );
            }

            Logger.LogInformation("[CreateInStoreOrder] User assigned to StoreId: {StoreId}", userStoreId.Value);

            var currentUser = await _userRepository.GetAsync(_currentUser.Id.Value);
            var store = await _storeRepository.GetAsync(userStoreId.Value);

            Logger.LogDebug("[CreateInStoreOrder] Store loaded - StoreId: {StoreId}, StoreName: {StoreName}, StoreCode: {StoreCode}",
                store.Id, store.Name, store.Code);

            var orderNumber = await GenerateInStoreOrderNumberAsync(userStoreId.Value);
            Logger.LogInformation("[CreateInStoreOrder] Order number generated - OrderNumber: {OrderNumber}", orderNumber);

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

            Logger.LogDebug("[CreateInStoreOrder] Order object created - OrderId: {OrderId}, SellerId: {SellerId}, SellerName: {SellerName}",
                order.Id, currentUser.Id, currentUser.Name ?? currentUser.UserName);

            var productIds = input.Items.Select(i => i.ProductId).ToList();
            var products = (await _productRepository.GetListAsync(p => productIds.Contains(p.Id)))
                          .ToDictionary(p => p.Id);

            Logger.LogDebug("[CreateInStoreOrder] Products loaded - RequestedCount: {RequestedCount}, FoundCount: {FoundCount}",
                productIds.Count, products.Count);

            var itemsProcessed = 0;
            foreach (var itemDto in input.Items)
            {
                if (!products.TryGetValue(itemDto.ProductId, out var product))
                {
                    Logger.LogWarning("[CreateInStoreOrder] FAILED - Product not found. ProductId: {ProductId}", itemDto.ProductId);
                    throw new UserFriendlyException(
                        _localizer["Product:NotFound"],
                        $"Không tìm thấy sản phẩm với ID: {itemDto.ProductId}"
                    );
                }

                Logger.LogDebug("[CreateInStoreOrder] Processing item - ProductId: {ProductId}, ProductName: {ProductName}, Quantity: {Quantity}, IsActive: {IsActive}",
                    product.Id, product.ProductName, itemDto.Quantity, product.IsActive);

                if (!product.IsActive)
                {
                    Logger.LogWarning("[CreateInStoreOrder] FAILED - Product not active. ProductId: {ProductId}, ProductName: {ProductName}",
                        product.Id, product.ProductName);
                    throw new UserFriendlyException(
                        _localizer["Product:NotActive"],
                        $"Sản phẩm '{product.ProductName}' không còn hoạt động."
                    );
                }

                if (product.ReleaseDate.HasValue && product.ReleaseDate.Value > DateTime.Now)
                {
                    Logger.LogWarning("[CreateInStoreOrder] FAILED - Product not yet released. ProductId: {ProductId}, ProductName: {ProductName}, ReleaseDate: {ReleaseDate}",
                        product.Id, product.ProductName, product.ReleaseDate.Value);
                    throw new UserFriendlyException(
                        _localizer["Product:NotYetReleased"],
                        $"Sản phẩm '{product.ProductName}' sẽ có sẵn từ {product.ReleaseDate.Value:dd/MM/yyyy}."
                    );
                }

                var hasStock = await _storeInventoryRepository.HasSufficientStockAsync(
                    userStoreId.Value,
                    product.Id,
                    itemDto.Quantity
                );

                if (!hasStock)
                {
                    var availableStock = await GetAvailableStockAsync(userStoreId.Value, product.Id);
                    Logger.LogWarning("[CreateInStoreOrder] FAILED - Insufficient stock. StoreId: {StoreId}, ProductId: {ProductId}, ProductName: {ProductName}, Required: {Required}, Available: {Available}",
                        userStoreId.Value, product.Id, product.ProductName, itemDto.Quantity, availableStock);
                    throw new UserFriendlyException(
                        _localizer["Product:Stock:NotEnoughStockInStore", product.ProductName, store.Name, availableStock]
                    );
                }

                order.AddOrderItem(
                    product.Id,
                    product.ProductName,
                    product.DiscountedPrice ?? product.OriginalPrice,
                    itemDto.Quantity
                );

                var storeInventory = await _storeInventoryRepository.GetByStoreAndProductAsync(
                    userStoreId.Value,
                    product.Id
                );
                var oldStock = storeInventory.Quantity;
                storeInventory.RemoveStock(itemDto.Quantity);
                await _storeInventoryRepository.UpdateAsync(storeInventory);
                itemsProcessed++;

                Logger.LogDebug("[CreateInStoreOrder] Item added & stock reduced - ProductId: {ProductId}, Quantity: {Quantity}, Price: {Price}, OldStock: {OldStock}, NewStock: {NewStock}",
                    product.Id, itemDto.Quantity, product.DiscountedPrice ?? product.OriginalPrice, oldStock, storeInventory.Quantity);
            }

            Logger.LogInformation("[CreateInStoreOrder] Items processed - Total: {ItemsProcessed}, TotalQuantity: {TotalQuantity}",
                itemsProcessed, order.OrderItems.Sum(i => i.Quantity));

            order.CalculateTotals();
            Logger.LogInformation("[CreateInStoreOrder] Totals calculated - Total: {Total}",
                order.TotalAmount);

            await _orderRepository.InsertAsync(order, autoSave: true);
            Logger.LogInformation("[CreateInStoreOrder] Order saved - OrderId: {OrderId}, OrderNumber: {OrderNumber}",
                order.Id, order.OrderNumber);

            await _orderHistoryAppService.LogOrderChangeAsync(
                order.Id,
                OrderStatus.Pending,
                order.OrderStatus,
                PaymentStatus.Unpaid,
                order.PaymentStatus,
                "In-store order created"
            );
            Logger.LogDebug("[CreateInStoreOrder] Order history logged");

            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            Logger.LogDebug("[CreateInStoreOrder] Notification sent");

            Logger.LogInformation("[CreateInStoreOrder] COMPLETED - OrderId: {OrderId}, OrderNumber: {OrderNumber}, StoreId: {StoreId}, SellerId: {SellerId}",
                order.Id, order.OrderNumber, userStoreId.Value, currentUser.Id);

            return await MapToOrderDtoAsync(order);
        }

        [Authorize(ProductSellingPermissions.Orders.Complete)]
        public async Task<OrderDto> CompleteInStorePaymentAsync(Guid orderId, CompleteInStorePaymentDto input)
        {
            Logger.LogInformation("[CompleteInStorePayment] START - OrderId: {OrderId}, PaidAmount: {PaidAmount}, UserId: {UserId}",
                orderId, input.PaidAmount, _currentUser.Id);

            var order = await _orderRepository.GetAsync(orderId);

            Logger.LogDebug("[CompleteInStorePayment] Order loaded - OrderNumber: {OrderNumber}, OrderType: {OrderType}, OrderStatus: {OrderStatus}, PaymentStatus: {PaymentStatus}, TotalAmount: {TotalAmount}",
                order.OrderNumber, order.OrderType, order.OrderStatus, order.PaymentStatus, order.TotalAmount);

            if (order.OrderType != OrderType.InStore)
            {
                Logger.LogWarning("[CompleteInStorePayment] FAILED - Invalid order type. OrderId: {OrderId}, OrderType: {OrderType}, Expected: InStore",
                    orderId, order.OrderType);
                throw new UserFriendlyException(
                    _localizer["Order:OnlyForInStoreOrders"],
                    "Phương thức này chỉ dành cho đơn hàng tại cửa hàng."
                );
            }

            await CheckStoreAccessAsync(order.StoreId.Value);
            Logger.LogDebug("[CompleteInStorePayment] Store access verified - StoreId: {StoreId}", order.StoreId.Value);

            var currentUser = await _userRepository.GetAsync(_currentUser.Id.Value);
            Logger.LogDebug("[CompleteInStorePayment] Cashier info - CashierId: {CashierId}, CashierName: {CashierName}",
                currentUser.Id, currentUser.Name ?? currentUser.UserName);

            var oldOrderStatus = order.OrderStatus;
            var oldPaymentStatus = order.PaymentStatus;

            order.CompletePaymentInStore(
                currentUser.Id,
                currentUser.Name ?? currentUser.UserName,
                input.PaidAmount
            );

            Logger.LogInformation("[CompleteInStorePayment] Payment completed - OrderId: {OrderId}, OldOrderStatus: {OldOrderStatus} -> NewOrderStatus: {NewOrderStatus}, OldPaymentStatus: {OldPaymentStatus} -> NewPaymentStatus: {NewPaymentStatus}, PaidAmount: {PaidAmount}",
                orderId, oldOrderStatus, order.OrderStatus, oldPaymentStatus, order.PaymentStatus, input.PaidAmount);

            await _orderRepository.UpdateAsync(order, autoSave: true);
            Logger.LogDebug("[CompleteInStorePayment] Order updated in database");

            await _orderHistoryAppService.LogOrderChangeAsync(
                orderId,
                oldOrderStatus,
                order.OrderStatus,
                oldPaymentStatus,
                order.PaymentStatus,
                $"Payment completed by cashier. Amount: {input.PaidAmount:C}"
            );
            Logger.LogDebug("[CompleteInStorePayment] Order history logged");

            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            Logger.LogDebug("[CompleteInStorePayment] Notification sent");

            Logger.LogInformation("[CompleteInStorePayment] COMPLETED - OrderId: {OrderId}, CashierId: {CashierId}",
                orderId, currentUser.Id);

            return await MapToOrderDtoAsync(order);
        }

        [Authorize(ProductSellingPermissions.Orders.Fulfill)]
        public async Task<OrderDto> FulfillInStoreOrderAsync(Guid orderId)
        {
            Logger.LogInformation("[FulfillInStoreOrder] START - OrderId: {OrderId}, UserId: {UserId}",
                orderId, _currentUser.Id);

            var order = await _orderRepository.GetAsync(orderId);

            Logger.LogDebug("[FulfillInStoreOrder] Order loaded - OrderNumber: {OrderNumber}, OrderType: {OrderType}, OrderStatus: {OrderStatus}, PaymentStatus: {PaymentStatus}",
                order.OrderNumber, order.OrderType, order.OrderStatus, order.PaymentStatus);

            if (order.OrderType != OrderType.InStore)
            {
                Logger.LogWarning("[FulfillInStoreOrder] FAILED - Invalid order type. OrderId: {OrderId}, OrderType: {OrderType}, Expected: InStore",
                    orderId, order.OrderType);
                throw new UserFriendlyException(
                    _localizer["Order:OnlyForInStoreOrders"],
                    "Phương thức này chỉ dành cho đơn hàng tại cửa hàng."
                );
            }

            await CheckStoreAccessAsync(order.StoreId.Value);
            Logger.LogDebug("[FulfillInStoreOrder] Store access verified - StoreId: {StoreId}", order.StoreId.Value);

            var currentUser = await _userRepository.GetAsync(_currentUser.Id.Value);
            Logger.LogDebug("[FulfillInStoreOrder] Warehouse staff info - StaffId: {StaffId}, StaffName: {StaffName}",
                currentUser.Id, currentUser.Name ?? currentUser.UserName);

            var oldOrderStatus = order.OrderStatus;

            order.FulfillInStore(
                currentUser.Id,
                currentUser.Name ?? currentUser.UserName
            );

            Logger.LogInformation("[FulfillInStoreOrder] Order fulfilled - OrderId: {OrderId}, OldStatus: {OldStatus} -> NewStatus: {NewStatus}, FulfilledBy: {FulfilledBy}",
                orderId, oldOrderStatus, order.OrderStatus, currentUser.Name ?? currentUser.UserName);

            await _orderRepository.UpdateAsync(order, autoSave: true);
            Logger.LogDebug("[FulfillInStoreOrder] Order updated in database");

            await _orderHistoryAppService.LogOrderChangeAsync(
                orderId,
                oldOrderStatus,
                order.OrderStatus,
                order.PaymentStatus,
                order.PaymentStatus,
                "Order fulfilled - items given to customer"
            );
            Logger.LogDebug("[FulfillInStoreOrder] Order history logged");

            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            Logger.LogDebug("[FulfillInStoreOrder] Notification sent");

            Logger.LogInformation("[FulfillInStoreOrder] COMPLETED - OrderId: {OrderId}, FulfillerId: {FulfillerId}",
                orderId, currentUser.Id);

            return await MapToOrderDtoAsync(order);
        }

        /*
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

        */
        [Authorize]
        public async Task DeleteAsync(Guid id)
        {
            Logger.LogInformation("[DeleteOrder] START - OrderId: {OrderId}, UserId: {UserId}", id, CurrentUser.Id);

            var order = await _orderRepository.GetAsync(id, includeDetails: true);

            Logger.LogDebug("[DeleteOrder] Order loaded - OrderNumber: {OrderNumber}, CustomerId: {CustomerId}, ItemCount: {ItemCount}",
                order.OrderNumber, order.CustomerId, order.OrderItems.Count);

            if (order.CustomerId != CurrentUser.Id)
            {
                Logger.LogWarning("[DeleteOrder] FAILED - Unauthorized. OrderId: {OrderId}, OrderCustomerId: {OrderCustomerId}, RequestUserId: {RequestUserId}",
                    id, order.CustomerId, CurrentUser.Id);
                throw new AbpAuthorizationException(L["Account:Unauthorized"]);
            }
            if (order.OrderStatus != OrderStatus.Placed && order.OrderStatus != OrderStatus.Pending)
            {
                Logger.LogWarning("[DeleteOrder] FAILED - Invalid status. OrderId: {OrderId}, Status: {Status}",
                    id, order.OrderStatus);
                throw new UserFriendlyException(
                    L["Order:CannotCancelOrderInCurrentStatus"],
                    $"Cannot cancel order in status: {order.OrderStatus}"
                );
            }

            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                Logger.LogWarning("[DeleteOrder] FAILED - Order already paid. OrderId: {OrderId}", id);
                throw new UserFriendlyException(
                    L["Order:CannotCancelPaidOrder"],
                    "Cannot cancel a paid order."
                );
            }
            var oldStatus = order.OrderStatus;
            var oldPaymentStatus = order.PaymentStatus;

            order.CancelByUser(_localizer);

            Logger.LogInformation("[DeleteOrder] Order cancelled - OrderId: {OrderId}, OldStatus: {OldStatus}, NewStatus: {NewStatus}",
                id, oldStatus, order.OrderStatus);

            var stockRestored = 0;
            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.GetAsync(item.ProductId);
                var oldStock = product.StockCount;
                product.StockCount += item.Quantity;
                await _productRepository.UpdateAsync(product, autoSave: false); // Save at the end

                Logger.LogDebug("[DeleteOrder] Stock restored - ProductId: {ProductId}, Quantity: {Quantity}, OldStock: {OldStock}, NewStock: {NewStock}",
                    item.ProductId, item.Quantity, oldStock, product.StockCount);
            }

            Logger.LogInformation("[DeleteOrder] Stock restoration completed - ItemsProcessed: {ItemsProcessed}", stockRestored);

            await _orderRepository.UpdateAsync(order, autoSave: true);
            Logger.LogDebug("[DeleteOrder] Order updated in database");


            await _orderHistoryAppService.LogOrderChangeAsync(
                id,
                oldStatus,
                order.OrderStatus,     
                oldPaymentStatus,
                order.PaymentStatus,    
                _localizer["Order:CancelledByUser"] 
            );
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            Logger.LogDebug("[DeleteOrder] Notification sent");

            


            Logger.LogInformation("[DeleteOrder] COMPLETED - OrderId: {OrderId}", id);
        }

        [Authorize(ProductSellingPermissions.Orders.Default)]
        public async Task RestoreOrderAsync(Guid orderId)
        {
            Logger.LogInformation("[RestoreOrder] START - OrderId: {OrderId}, UserId: {UserId}", orderId, CurrentUser.Id);

            var query = await _orderRepository.GetQueryableAsync();
            var order = await query.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || !order.IsDeleted)
            {
                Logger.LogWarning("[RestoreOrder] FAILED - Order not found or not deleted. OrderId: {OrderId}, Found: {Found}, IsDeleted: {IsDeleted}",
                    orderId, order != null, order?.IsDeleted);
                throw new UserFriendlyException(L["Order:NotFoundOrNotDeleted"]);
            }

            Logger.LogDebug("[RestoreOrder] Deleted order found - OrderNumber: {OrderNumber}, DeletionTime: {DeletionTime}, DeleterId: {DeleterId}",
                order.OrderNumber, order.DeletionTime, order.DeleterId);

            order.IsDeleted = false;
            order.DeletionTime = null;
            order.DeleterId = null;

            await _orderRepository.UpdateAsync(order, autoSave: true);

            Logger.LogInformation("[RestoreOrder] COMPLETED - OrderId: {OrderId}, OrderNumber: {OrderNumber}, RestoredBy: {AdminId}",
                orderId, order.OrderNumber, CurrentUser.Id);
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

        private async Task CheckStoreAccessAsync(Guid storeId)
        {
            Logger.LogDebug("[CheckStoreAccess] START - StoreId: {StoreId}, UserId: {UserId}", storeId, _currentUser.Id);

            if (await IsAdminOrManagerAsync())
            {
                Logger.LogDebug("[CheckStoreAccess] Access granted - User is admin/manager");
                return;
            }

            var userStoreId = await GetCurrentUserStoreIdAsync();
            if (!userStoreId.HasValue || userStoreId.Value != storeId)
            {
                Logger.LogWarning("[CheckStoreAccess] FAILED - No store access. UserId: {UserId}, UserStoreId: {UserStoreId}, RequestedStoreId: {RequestedStoreId}",
                    _currentUser.Id, userStoreId?.ToString() ?? "None", storeId);
                throw new UserFriendlyException(
                    _localizer["Order:NoStoreAccess"],
                    "Bạn không có quyền truy cập dữ liệu của cửa hàng này."
                );
            }

            Logger.LogDebug("[CheckStoreAccess] Access granted - User assigned to store");
        }

        private async Task<string> GenerateInStoreOrderNumberAsync(Guid storeId)
        {
            Logger.LogDebug("[GenerateOrderNumber] START - StoreId: {StoreId}", storeId);

            var store = await _storeRepository.GetAsync(storeId);
            var date = DateTime.Now.ToString("yyyyMMdd");
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
            var orderNumber = $"ST-{store.Code}-{date}-{sequence}";

            Logger.LogInformation("[GenerateOrderNumber] COMPLETED - StoreId: {StoreId}, StoreCode: {StoreCode}, TodayOrderCount: {Count}, OrderNumber: {OrderNumber}",
                storeId, store.Code, todayOrderCount, orderNumber);

            return orderNumber;
        }

        private async Task<int> GetAvailableStockAsync(Guid storeId, Guid productId)
        {
            var inventory = await _storeInventoryRepository.GetByStoreAndProductAsync(storeId, productId);
            var stock = inventory?.Quantity ?? 0;

            Logger.LogDebug("[GetAvailableStock] StoreId: {StoreId}, ProductId: {ProductId}, AvailableStock: {Stock}",
                storeId, productId, stock);

            return stock;
        }

        private async Task<OrderDto> MapToOrderDtoAsync(Order order)
        {
            Logger.LogDebug("[MapToOrderDto] START - OrderId: {OrderId}", order.Id);

            var dto = _orderMapper.Map(order);

            if (order.StoreId.HasValue)
            {
                var store = await _storeRepository.GetAsync(order.StoreId.Value);
                dto.StoreName = store.Name;
                Logger.LogDebug("[MapToOrderDto] Store name added - StoreId: {StoreId}, StoreName: {StoreName}",
                    store.Id, store.Name);
            }

            Logger.LogDebug("[MapToOrderDto] COMPLETED - OrderId: {OrderId}", order.Id);
            return dto;
        }

        // Additional methods remain the same...
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

        [Authorize(ProductSellingPermissions.Orders.ConfirmCodPayment)]
        public async Task MarkAsCodPaidAndCompletedAsync(Guid orderId)
        {
            Logger.LogInformation("[MarkAsCodPaidAndCompleted] START - OrderId: {OrderId}, UserId: {UserId}",
                orderId, _currentUser.Id);

            var order = await _orderRepository.GetAsync(orderId);

            Logger.LogDebug("[MarkAsCodPaidAndCompleted] Order loaded - OrderNumber: {OrderNumber}, PaymentMethod: {PaymentMethod}, OrderStatus: {OrderStatus}, PaymentStatus: {PaymentStatus}",
                order.OrderNumber, order.PaymentMethod, order.OrderStatus, order.PaymentStatus);

            var oldOrderStatus = order.OrderStatus;
            var oldPaymentStatus = order.PaymentStatus;

            order.MarkAsCodPaidAndCompleted(_localizer);

            Logger.LogInformation("[MarkAsCodPaidAndCompleted] Order marked as completed - OrderId: {OrderId}, OldStatus: {OldOrderStatus} -> NewStatus: {NewOrderStatus}, OldPaymentStatus: {OldPaymentStatus} -> NewPaymentStatus: {NewPaymentStatus}",
                orderId, oldOrderStatus, order.OrderStatus, oldPaymentStatus, order.PaymentStatus);

            await _orderRepository.UpdateAsync(order, autoSave: true);
            Logger.LogDebug("[MarkAsCodPaidAndCompleted] Order updated in database");

            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            Logger.LogDebug("[MarkAsCodPaidAndCompleted] Notification sent");

            Logger.LogInformation("[MarkAsCodPaidAndCompleted] COMPLETED - OrderId: {OrderId}", orderId);
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

        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(Guid orderId)
        {
            Logger.LogInformation("[GetOrderHistory] START - OrderId: {OrderId}", orderId);
            var history = await _orderHistoryAppService.GetOrderHistoryAsync(orderId);
            Logger.LogInformation("[GetOrderHistory] COMPLETED - OrderId: {OrderId}, HistoryCount: {Count}",
                orderId, history.Count);
            return history;
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

        [Authorize]
        public async Task<PagedResultDto<OrderDto>> GetStoreOrdersAsync(GetOrderListInput input)
        {
            Logger.LogInformation("[GetStoreOrders] START - UserId: {UserId}, InputStoreId: {InputStoreId}",
                _currentUser.Id, input.StoreId);

            var userStoreId = await GetCurrentUserStoreIdAsync();

            if (!userStoreId.HasValue)
            {
                Logger.LogWarning("[GetStoreOrders] FAILED - User not assigned to store. UserId: {UserId}", _currentUser.Id);
                throw new UserFriendlyException(
                    _localizer["Order:UserNotAssignedToStore"],
                    "Người dùng phải được gán vào một cửa hàng."
                );
            }

            if (!await IsAdminOrManagerAsync())
            {
                input.StoreId = userStoreId.Value;
                Logger.LogInformation("[GetStoreOrders] Non-admin user restricted to StoreId: {StoreId}", userStoreId.Value);
            }

            return await GetListAsync(input);
        }
    }
}