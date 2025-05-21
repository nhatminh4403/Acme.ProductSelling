using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Categories;
using Acme.ProductSelling.Permissions;
using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;
namespace Acme.ProductSelling.Orders
{
    public class OrderAppService : CrudAppService<Order, OrderDto,
        Guid, PagedAndSortedResultRequestDto, CreateOrderDto>, IOrderAppService
    {
        private readonly IRepository<Order, Guid> _orderRepository;
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Cart, Guid> _cartRepository;

        public OrderAppService(
           IRepository<Order, Guid> orderRepository,
           IRepository<Product, Guid> productRepository,
           IGuidGenerator guidGenerator,
           ICurrentUser currentUser,
                        IRepository<Cart, Guid> cartRepository ) : base(orderRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _guidGenerator = guidGenerator;
            _currentUser = currentUser;
            _cartRepository = cartRepository;
            GetPolicyName = ProductSellingPermissions.Orders.Default;
            CreatePolicyName = ProductSellingPermissions.Orders.Create;
            UpdatePolicyName = ProductSellingPermissions.Orders.Edit;
            DeletePolicyName = ProductSellingPermissions.Orders.Delete;
        }
        public override async Task<PagedResultDto<OrderDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await Repository.GetQueryableAsync();

            var query = queryable.Include(o => o.OrderItems).AsNoTracking();
            var queryResult = await AsyncExecuter.ToListAsync(query);

            var orderDtos = ObjectMapper.Map<List<Order>, List<OrderDto>>(queryResult);

            var totalCount = await Repository.GetCountAsync();

            return new PagedResultDto<OrderDto>(
                totalCount,
                orderDtos
            );
        }

        public override async Task<OrderDto> CreateAsync(CreateOrderDto input)
        {
            var orderNumber = $"DH-{DateTime.UtcNow:yyyyMMddHHmmss}-{_guidGenerator.Create().ToString("N").Substring(0, 6)}";

            var customerId = _currentUser.Id;
            var customerName = input.CustomerName;
            var cart = await (await _cartRepository.WithDetailsAsync(c => c.Items)) // Include Items
                                  .FirstOrDefaultAsync(c => c.UserId == customerId);
            if (cart == null || !cart.Items.Any())
            {
                throw new UserFriendlyException(L["ShoppingCartIsEmpty"]);
            }
            var order = new Order
            {
                OrderNumber = orderNumber,
                CustomerId = customerId,
                CustomerName = customerName,
                CustomerPhone = input.CustomerPhone,
                ShippingAddress = input.ShippingAddress,
                OrderDate = DateTime.UtcNow,

            };


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

                order.AddOrderItem(product.Id, product.ProductName, product.OriginalPrice, itemDto.Quantity);

                product.StockCount -= itemDto.Quantity;
                await _productRepository.UpdateAsync(product, autoSave: false); 
            }
            if (!order.OrderItems.Any())
            {
                throw new UserFriendlyException(L["NoValidItemsInCartToOrder"]);
            }

            order.CalculateTotals();

            await _orderRepository.InsertAsync(order, autoSave: true);
            return ObjectMapper.Map<Order, OrderDto>(order);
        }
        [Authorize]
        public override async Task<OrderDto> GetAsync(Guid id)
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

/*        [Authorize] // Bảo vệ nếu cần
        public async Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListInput input)
        {
            var queryable = await _orderRepository.GetQueryableAsync();

            if (input.CustomerId.HasValue) // Lọc theo KH nếu có ID
            {
                queryable = queryable.Where(o => o.CustomerId == input.CustomerId.Value);
            }
            // Bỏ lọc theo Status

            if (!input.Filter.IsNullOrWhiteSpace())
            {
                queryable = queryable.Where(o => o.OrderNumber.Contains(input.Filter) ||
                                                o.CustomerName.Contains(input.Filter));
            }

            var totalCount = await AsyncExecuter.CountAsync(queryable);

            queryable = queryable
                .OrderBy(input.Sorting ?? nameof(Order.OrderDate) + " DESC")
                .PageBy(input);

            var orders = await AsyncExecuter.ToListAsync(queryable);

            return new PagedResultDto<OrderDto>(
                totalCount,
                ObjectMapper.Map<List<Order>, List<OrderDto>>(orders)
            );
        }*/
        public async Task<OrderDto> GetByOrderNumberAsync(string orderNumber)
        {
            var order = await _orderRepository.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
            if (order == null)
            {
                throw new UserFriendlyException(L["OrderNotFound"]);
            }
            return ObjectMapper.Map<Order, OrderDto>(order);
        }
        [Authorize]
        public async Task<PagedResultDto<OrderDto>> GetListForCurrentUserAsync(PagedAndSortedResultRequestDto input)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
            {
                throw new AbpAuthorizationException("User not authenticated.");
            }

            var currentUserId = _currentUser.Id.Value;

            var queryable = (await _orderRepository.GetQueryableAsync())
                            .Where(o => o.CustomerId == currentUserId);
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var orders = await AsyncExecuter.ToListAsync(
                queryable
                    .OrderBy(input.Sorting ?? $"{nameof(Order.OrderDate)} DESC") // Default sort by OrderDate descending
                    .PageBy(input)
            );
            return new PagedResultDto<OrderDto>(
                totalCount,
                ObjectMapper.Map<List<Order>, List<OrderDto>>(orders)
            );
        }

    }
}
