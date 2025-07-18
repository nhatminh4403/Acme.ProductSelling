using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Orders.BackgroundJobs;
using Acme.ProductSelling.Orders.Hubs;
using Acme.ProductSelling.Payments;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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

        public OrderAppService(
            IRepository<Order, Guid> orderRepository,
            IRepository<Product, Guid> productRepository,
            IGuidGenerator guidGenerator,
            ICurrentUser currentUser,
            IRepository<Cart, Guid> cartRepository,
            IHubContext<OrderHub, IOrderClient> hubContext,
            IBackgroundJobManager backgroundJobManager,
            IPaymentGatewayResolver gatewayResolver,
            IOrderNotificationService orderNotificationService)
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
            var orderNumber =
                $"DH-{DateTime.UtcNow:yyyyMMddHHmmss}" +
                $"-{_guidGenerator.Create().ToString("N").Substring(0, 6)}";

            var customerId = _currentUser.Id;
            var cart = await (await _cartRepository.WithDetailsAsync(c => c.Items))
                                  .FirstOrDefaultAsync(c => c.UserId == customerId);

            if (cart == null || !cart.Items.Any())
            {
                throw new UserFriendlyException(L["ShoppingCartIsEmpty"]);
            }

            var order = new Order(
                _guidGenerator.Create(),
                orderNumber,
                DateTime.Now,
                customerId,
                input.CustomerName,
                input.CustomerPhone,
                input.ShippingAddress,
                input.PaymentMethod
            );

            var productIds = input.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = (await _productRepository.GetListAsync(p => productIds.Contains(p.Id)))
                           .ToDictionary(p => p.Id);

            foreach (var itemDto in input.Items)
            {
                if (!products.TryGetValue(itemDto.ProductId, out var product))
                {
                    throw new UserFriendlyException($"Product with ID {itemDto.ProductId} not found.");

                }
                if (product.StockCount < itemDto.Quantity)
                {
                    throw new UserFriendlyException
                        ($"Not enough stock for product '{product.ProductName}'." +
                        $" Available: {product.StockCount}, Requested: {itemDto.Quantity}");
                }
                var priceToUse = product.DiscountedPrice ?? product.OriginalPrice;
                order.AddOrderItem(product.Id, product.ProductName, priceToUse, itemDto.Quantity);

                product.StockCount -= itemDto.Quantity;
                await _productRepository.UpdateAsync(product, autoSave: false);
            }
            if (!order.OrderItems.Any())
            {
                throw new UserFriendlyException(L["NoValidItemsInCartToOrder"]);
            }

            order.CalculateTotals();
            var gateway = _paymentGatewayResolver.Resolve(input.PaymentMethod);

            // B4: Xử lý qua gateway
            var gatewayResult = await gateway.ProcessAsync(order);

            order.SetStatus(gatewayResult.NextOrderStatus);

            await _orderRepository.InsertAsync(order, autoSave: true);

            if (order.PaymentMethod == "COD" && order.Status == OrderStatus.Placed)
            {
                await _backgroundJobManager.EnqueueAsync<SetOrderPendingJobArgs>(
                    new SetOrderPendingJobArgs { OrderId = order.Id },
                    delay: TimeSpan.FromMinutes(5)
                );
            }

            // B8: Gửi thông báo
            await _orderNotificationService.NotifyOrderStatusChangeAsync(order);

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

            order.CancelByUser();

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
