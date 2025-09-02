using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.BackgroundJobs;
using Acme.ProductSelling.Orders.Hubs;
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
            IStringLocalizer<ProductSellingResource> localizer)
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
            //GetPolicyName = ProductSellingPermissions.Orders.Default;
            //CreatePolicyName = ProductSellingPermissions.Orders.Create;
            //UpdatePolicyName = ProductSellingPermissions.Orders.Edit;
            //DeletePolicyName = ProductSellingPermissions.Orders.Delete;
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
                Logger.LogInformation("Đã cập nhật trạng thái đơn hàng {OrderId} thành {Status}", order.Id, order.Status);

                // 6. Gửi các sự kiện và thông báo cần thiết
                // Ví dụ: Gửi Event để gửi email, thông báo cho kho hàng...
                // await _distributedEventBus.PublishAsync(new OrderConfirmedEto(order.Id));

                // Gửi thông báo real-time
                await _orderNotificationService.NotifyOrderStatusChangeAsync(order);

                // 7. Trả về thông tin đơn hàng đã được cập nhật
                return ObjectMapper.Map<Order, OrderDto>(order);
            }
            else
            {
                Logger.LogWarning("Bỏ qua xác nhận." +
                      " Đơn hàng {OrderId} không ở trạng thái PendingPayment. " +
                      "Trạng thái hiện tại: {Status}", order.Id, order.Status);
                // Nếu đơn hàng đã được xác nhận trước đó, chỉ cần trả về thông tin
                return ObjectMapper.Map<Order, OrderDto>(order);
            }


        }


        [Authorize(ProductSellingPermissions.Orders.Default)]
        public async Task<PagedResultDto<OrderDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var query = (await _orderRepository.GetQueryableAsync())
                .Include(o => o.OrderItems);

            var totalCount = await AsyncExecuter.CountAsync(query);
            var items = await AsyncExecuter.
                ToListAsync(query.OrderBy(input.Sorting ?? "CreationTime DESC").PageBy(input));

            return new PagedResultDto<OrderDto>(
                totalCount,
                ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
            );
        }
        public async Task<CreateOrderResultDto> CreateAsync(CreateOrderDto input)
        {
            var customerId = _currentUser.Id.Value;

            var existingPendingOrder = await (await _orderRepository.WithDetailsAsync(o => o.OrderItems))
                .FirstOrDefaultAsync(o =>
                    o.CustomerId == customerId &&
                    o.PaymentStatus == PaymentStatus.Pending /*&&
                    o.Status == OrderStatus.Placed*/
                );


            // B1: Lấy giỏ hàng hiện tại của người dùng. Sẽ dùng cho cả hai trường hợp.
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

                // --- BƯỚC CẬP NHẬT ĐƠN HÀNG CŨ ---

                // 1. Hoàn lại stock cho các sản phẩm trong đơn hàng cũ
                foreach (var oldItem in orderToProcess.OrderItems)
                {
                    var product = await _productRepository.FindAsync(oldItem.ProductId);
                    if (product != null)
                    {
                        product.StockCount += oldItem.Quantity;
                        await _productRepository.UpdateAsync(product);
                    }
                }

                // 2. Xóa tất cả các item cũ khỏi đơn hàng
                orderToProcess.OrderItems.Clear();

                // 3. Cập nhật lại thông tin giao hàng và phương thức thanh toán
                orderToProcess.UpdateShippingInfo(input.CustomerName, input.CustomerPhone, input.ShippingAddress);
                orderToProcess.UpdatePaymentMethod(input.PaymentMethod); // Cần tạo các phương thức này trong Order entity
            }
            else
            {
                Logger.LogInformation("Bắt đầu tiến trình tạo đơn hàng mới cho user {UserId}.", customerId);

                // --- BƯỚC TẠO ĐƠN HÀNG MỚI ---
                orderToProcess = new Order(
                    GuidGenerator.Create(),
                    $"DH-{DateTime.UtcNow:yyyyMMddHHmmss}-{GuidGenerator.Create().ToString("N").Substring(0, 6)}",
                    DateTime.Now, customerId, input.CustomerName, input.CustomerPhone,
                    input.ShippingAddress, input.PaymentMethod
                );

                // Trạng thái ban đầu cho đơn hàng mới
                orderToProcess.SetStatus(OrderStatus.Placed);
            }

            // --- BƯỚC CHUNG: ĐỒNG BỘ ĐƠN HÀNG VỚI GIỎ HÀNG HIỆN TẠI ---

            // 1. Thêm các item từ giỏ hàng hiện tại vào đơn hàng (cũ hoặc mới)
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
                    product.StockCount -= cartItem.Quantity; // Giảm stock mới
                    await _productRepository.UpdateAsync(product);
                }
            }

            // 2. Tính lại tổng tiền
            orderToProcess.CalculateTotals();

            // 3. Đặt trạng thái thanh toán
            if (input.PaymentMethod == PaymentMethods.COD)
            {
                orderToProcess.SetPaymentStatus(PaymentStatus.Unpaid);
                orderToProcess.SetStatus(OrderStatus.Placed); // Đơn COD thì trạng thái là Placed
            }
            else
            {
                orderToProcess.SetPaymentStatus(PaymentStatus.Pending);

                orderToProcess.SetStatus(OrderStatus.Placed);
            }

            // 4. Xử lý qua gateway
            var gateway = _paymentGatewayResolver.Resolve(input.PaymentMethod);
            var gatewayResult = await gateway.ProcessAsync(orderToProcess);

            // 5. Lưu đơn hàng (Insert nếu là mới, Update nếu là cũ)
            await _orderRepository.UpsertAsync(orderToProcess, autoSave: true);

            // 6. Lên lịch job cho COD (logic không đổi)
            if (orderToProcess.PaymentMethod == PaymentMethods.COD && orderToProcess.Status == OrderStatus.Placed)
            {
                await _backgroundJobManager.EnqueueAsync<SetOrderPendingJobArgs>(
                    new SetOrderPendingJobArgs { OrderId = orderToProcess.Id },
                    delay: TimeSpan.FromSeconds(30) // Giữ nguyên logic cũ
                );
            }

            // 7. Dọn dẹp giỏ hàng
            await _cartRepository.DeleteAsync(cart, autoSave: true);

            // 8. Gửi thông báo
            await _orderNotificationService.NotifyOrderStatusChangeAsync(orderToProcess);

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
            var order = await _orderRepository.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
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

        [Authorize(ProductSellingPermissions.Orders.Edit)] // Phân quyền cho Admin
        public async Task<OrderDto> UpdateStatusAsync(Guid id, UpdateOrderStatusDto input)
        {
            var order = await _orderRepository.GetAsync(id);

            order.SetStatus(input.NewStatus);
            order.SetPaymentStatus(input.NewPaymentStatus);

            await _orderRepository.UpdateAsync(order, autoSave: true);

            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        [Authorize]
        public async Task DeleteAsync(Guid id)
        {
            var order = await _orderRepository.GetAsync(id, includeDetails: true); // include OrderItems
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
        [Authorize(ProductSellingPermissions.Orders.ConfirmCodPayment)]
        public async Task MarkAsCodPaidAndCompletedAsync(Guid orderId)
        {
            var order = await _orderRepository.GetAsync(orderId);

            // 2. Gọi phương thức nghiệp vụ trong Entity. 
            // Entity sẽ tự kiểm tra các quy tắc logic.
            order.MarkAsCodPaidAndCompleted();

            // 3. Lưu lại thay đổi
            await _orderRepository.UpdateAsync(order, autoSave: true);

            // 4. Gửi các thông báo cần thiết
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);

            Logger.LogInformation("Đơn hàng COD {OrderId} đã được xác nhận thanh toán và hoàn thành.", orderId);
        }
    }
}
