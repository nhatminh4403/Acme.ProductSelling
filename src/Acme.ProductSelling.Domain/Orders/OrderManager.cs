using Acme.ProductSelling.Carts;
using Acme.ProductSelling.Localization;
using Acme.ProductSelling.Orders.Services;
using Acme.ProductSelling.Products.Services;
using Acme.ProductSelling.StoreInventories;
using Acme.ProductSelling.Stores;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace Acme.ProductSelling.Orders
{
    public class OrderManager : DomainService
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IStoreInventoryRepository _storeInventoryRepository;

        private readonly IGuidGenerator _guidGenerator;
        private readonly IStringLocalizer<ProductSellingResource> _localizer;
        public OrderManager(IProductRepository productRepository,
                            IOrderRepository orderRepository,
                            IStoreRepository storeRepository,
                            IGuidGenerator guidGenerator,
                            IStringLocalizer<ProductSellingResource> localizer,
                            IStoreInventoryRepository storeInventoryRepository)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _storeRepository = storeRepository;
            _guidGenerator = guidGenerator;
            _localizer = localizer;
            _storeInventoryRepository = storeInventoryRepository;
        }
        public async Task<OnlineOrder> CreateOnlineOrderAsync(
                   Guid customerId,
                   string customerName,
                   string customerPhone,
                   string shippingAddress,
                   string paymentMethod,
                   List<CartItem> cartItems)
        {
            var orderNumber = await GenerateOrderNumberAsync();

            var order = new OnlineOrder(
                _guidGenerator.Create(),
                orderNumber,
                Clock.Now,
                customerId,
                customerName,
                customerPhone,
                shippingAddress,
                paymentMethod
            );
            //order.SetStatus(OrderStatus.Placed);

            var productIds = cartItems.Select(i => i.ProductId).Distinct().ToList();
            var products = await _productRepository.GetListAsync(p => productIds.Contains(p.Id));

            foreach (var item in cartItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null) throw new UserFriendlyException(ProductSellingDomainErrorCodes.ProductNotFound);

                if (product.StockCount < item.Quantity)
                {
                    throw new UserFriendlyException(ProductSellingDomainErrorCodes.InventoryInsufficientStock)
                        .WithData("Name", product.ProductName)
                        .WithData("Quantity", product.StockCount)
                        .WithData("Requested", item.Quantity);
                }

                // Add item
                order.AddOrderItem(
                    product.Id,
                    product.ProductName,
                    product.DiscountedPrice ?? product.OriginalPrice,
                    item.Quantity
                );

                // DEDUCT STOCK (Online Logic)
                product.StockCount -= item.Quantity;
            }

            order.CalculateTotals();
            return order;
        }

        public async Task RestoreStockAsync(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.FindByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.StockCount += item.Quantity;

                    await _productRepository.UpdateAsync(product);
                }
            }
        }
        public async Task<InStoreOrder> CreateInStoreOrderAsync(
            Guid storeId,
            Guid sellerId,
            string sellerName,
            string customerName,
            string customerPhone,
            string paymentMethod,
            List<(Guid ProductId, int Quantity)> items)
        {
            // Validate Store Access & Generate Number
            var store = await _storeRepository.GetAsync(storeId);
            var orderNumber = await GenerateOrderNumberAsync(store);

            var order = new InStoreOrder(
                _guidGenerator.Create(),
                orderNumber,
                Clock.Now,
                storeId,
                sellerId,
                sellerName,
                customerName,
                customerPhone,
                paymentMethod
            );

            // Fetch Product Info
            var productIds = items.Select(x => x.ProductId).Distinct().ToList();
            var products = await _productRepository.GetListAsync(p => productIds.Contains(p.Id));

            foreach (var itemDto in items)
            {
                var product = products.FirstOrDefault(p => p.Id == itemDto.ProductId);
                if (product == null) throw new UserFriendlyException(ProductSellingDomainErrorCodes.ProductNotFound);

                // Active Checks
                if (!product.IsActive) throw new UserFriendlyException(ProductSellingDomainErrorCodes.ProductNotActive)
                    .WithData("Name", product.ProductName);
                if (product.ReleaseDate.HasValue && product.ReleaseDate.Value > DateTime.Now)
                    throw new BusinessException(ProductSellingDomainErrorCodes.IdentityDataSeedingFailed)
                        //.WithData("User", product.)
                        //.WithData("Role", roleNameToAssign)
                        //.WithData("Reason", errorDetails)
                        .WithData("Name", product.ProductName);

                // Store Stock Check
                var hasStock = await _storeInventoryRepository.HasSufficientStockAsync(storeId, product.Id, itemDto.Quantity);
                if (!hasStock)
                {
                    var currentStock = (await _storeInventoryRepository.GetByStoreAndProductAsync(storeId, product.Id))?.Quantity ?? 0;
                    throw new UserFriendlyException(ProductSellingDomainErrorCodes.StoreInventoryInsufficientStock)
                        .WithData("Name", product.ProductName)
                        .WithData("StoreName", store.Name)
                        .WithData("Quantity", currentStock)
                        .WithData("Requested", itemDto.Quantity);
                }

                order.AddOrderItem(
                    product.Id,
                    product.ProductName,
                    product.DiscountedPrice ?? product.OriginalPrice,
                    itemDto.Quantity
                );

                // DEDUCT STOCK (In-Store Logic)
                var inventory = await _storeInventoryRepository.GetByStoreAndProductAsync(storeId, product.Id);
                inventory.RemoveStock(itemDto.Quantity);
                await _storeInventoryRepository.UpdateAsync(inventory);
            }

            order.CalculateTotals();
            return order;
        }

        private async Task<string> GenerateOrderNumberAsync(Store store = null)
        {
            if (store != null)
            {
                return $"DH-{Clock.Now:yyyyMMddHHmmss}-ST{store?.Code}-{_guidGenerator.Create().ToString("N").Substring(0, 6)}";
            }
            else return $"DH-{Clock.Now:yyyyMMddHHmmss}-{_guidGenerator.Create().ToString("N").Substring(0, 6)}";
        }
    }
}
