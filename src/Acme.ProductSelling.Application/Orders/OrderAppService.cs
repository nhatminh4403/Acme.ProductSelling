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
using Volo.Abp.Security.Encryption;
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
            Logger.LogInformation($"B1: Bắt đầu tiến trình tạo đơn hàng cho user {_currentUser.Id}.");

            // B1: Lấy giỏ hàng của người dùng từ CSDL
            var cart = await (await _cartRepository.WithDetailsAsync(c => c.Items))
                                  .FirstOrDefaultAsync(c => c.UserId == _currentUser.Id);

            if (cart == null || !cart.Items.Any())
            {
                Logger.LogWarning("Tạo đơn hàng thất bại: Giỏ hàng trống.");
                throw new UserFriendlyException(L["ShoppingCartIsEmpty"]);
            }
            Logger.LogInformation($"B2: Lấy giỏ hàng thành công với {cart.Items.Count} sản phẩm.");

            // B2: Tạo thực thể Order
            var order = new Order(
                GuidGenerator.Create(),
                $"DH-{DateTime.UtcNow:yyyyMMddHHmmss}-{GuidGenerator.Create().ToString("N").Substring(0, 6)}",
                DateTime.Now,
                _currentUser.Id,
                input.CustomerName,
                input.CustomerPhone,
                input.ShippingAddress,
                input.PaymentMethod
            );

            // B3: Kiểm tra và thêm sản phẩm
            var productIdsInCart = cart.Items.Select(i => i.ProductId).Distinct().ToList();
            var productsInDb = (await _productRepository.GetListAsync(p => productIdsInCart.Contains(p.Id)))
                               .ToDictionary(p => p.Id);

            foreach (var cartItem in cart.Items)
            {
                if (!productsInDb.TryGetValue(cartItem.ProductId, out var product))
                {
                    throw new UserFriendlyException($"Sản phẩm '{cartItem.ProductName}' không còn tồn tại.");
                }
                if (product.StockCount < cartItem.Quantity)
                {
                    throw new UserFriendlyException($"Không đủ số lượng cho sản phẩm '{product.ProductName}'.");
                }
                order.AddOrderItem(product.Id, product.ProductName,
                    product.DiscountedPrice ?? product.OriginalPrice, cartItem.Quantity);
                product.StockCount -= cartItem.Quantity;
                await _productRepository.UpdateAsync(product);
            }
            Logger.LogInformation($"B3: Đã thêm {order.OrderItems.Count} sản phẩm vào đơn hàng và giảm stock.");


            // B4: Tính tổng tiền và xử lý qua gateway
            order.CalculateTotals();
            Logger.LogInformation("B4: Gọi GatewayResolver cho phương thức: {PaymentMethod}", input.PaymentMethod);

            var gateway = _paymentGatewayResolver.Resolve(input.PaymentMethod);

            var gatewayResult = await gateway.ProcessAsync(order);

            order.SetStatus(gatewayResult.NextOrderStatus);

            Logger.LogInformation("B5: Gateway xử lý thành công. Trạng thái tiếp theo là {NextStatus}.", gatewayResult.NextOrderStatus);


            // B6: Lưu đơn hàng
            await _orderRepository.InsertAsync(order, autoSave: true);
            Logger.LogInformation("B6: Đã lưu đơn hàng {OrderId} vào CSDL.", order.Id);


            // B7: Lên lịch job cho COD
            if (order.PaymentMethod == "COD" && order.Status == OrderStatus.Placed)
            {
                await _backgroundJobManager.EnqueueAsync<SetOrderPendingJobArgs>(
                    new SetOrderPendingJobArgs { OrderId = order.Id },
                    delay: TimeSpan.FromMinutes(5)
                );
                Logger.LogInformation("B7: Đã lên lịch background job cho đơn hàng COD.");
            }

            // B8: Dọn dẹp giỏ hàng
            await _cartRepository.DeleteAsync(cart.Id, autoSave: true);
            Logger.LogInformation("B8: Đã xóa giỏ hàng {CartId}.", cart.Id);

            // B9: Gửi thông báo
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);
            Logger.LogInformation("B9: Đã gửi thông báo SignalR. Hoàn tất tạo đơn hàng.");


            return new CreateOrderResultDto
            {
                Order = ObjectMapper.Map<Order, OrderDto>(order),
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
                throw new UserFriendlyException(L["OrderNotFound"]);
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
                throw new AbpAuthorizationException("User not authenticated.");
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

            await _orderRepository.UpdateAsync(order, autoSave: true);

            // GỬI THÔNG BÁO REAL-TIME
            await _orderHubContext.Clients.All.ReceiveOrderStatusUpdate(
                order.Id,
                order.Status.ToString(),
                L[order.Status.ToString()]
            );

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        [Authorize]
        public async Task DeleteAsync(Guid id)
        {
            var order = await _orderRepository.GetAsync(id, includeDetails: true); // include OrderItems
            if (order.CustomerId != CurrentUser.Id) throw new AbpAuthorizationException("Không có quyền.");

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

    }
}
