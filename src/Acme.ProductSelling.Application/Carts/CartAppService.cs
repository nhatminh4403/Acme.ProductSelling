using Acme.ProductSelling.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Carts
{
    public class CartAppService : ApplicationService, ICartAppService
    {
        private readonly IRepository<Cart, Guid> _cartRepository; // Repository cho Cart
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;
        public CartAppService(
                    IRepository<Cart, Guid> cartRepository, // Inject Cart repo
                    IRepository<Product, Guid> productRepository,
                    ICurrentUser currentUser,
                    IGuidGenerator guidGenerator)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _currentUser = currentUser;
            _guidGenerator = guidGenerator;
        }


        private Guid GetCurrentUserId()
        {
            // Do có [Authorize] ở class, về lý thuyết Id sẽ không bao giờ null
            // Nhưng kiểm tra lại cho chắc chắn
            return _currentUser.Id ?? throw new AbpAuthorizationException("User is not authenticated.");
        }
        private async Task<Cart> GetOrCreateCurrentUserCartAsync()
        {
            var userId = GetCurrentUserId();
            // Include Items để thao tác trực tiếp
            var cart = await (await _cartRepository.WithDetailsAsync(c => c.Items))
                            .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                // Tạo giỏ hàng mới nếu chưa có
                cart = new Cart(_guidGenerator.Create(), userId);
            }
            return cart;
        }
        public async Task<CartDto> GetAsync()
        {
            var cart = await GetOrCreateCurrentUserCartAsync(); // Lấy giỏ hàng (có thể mới)

            // Lấy thông tin Product cho các item
            var cartDto = ObjectMapper.Map<Cart, CartDto>(cart); // Map Cart -> CartDto
            var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();

            if (productIds.Any())
            {
                var products = (await _productRepository.GetListAsync(p => productIds.Contains(p.Id)))
                                .ToDictionary(p => p.Id);

                foreach (var itemDto in cartDto.CartItems)
                {
                    if (products.TryGetValue(itemDto.ProductId, out var product))
                    {
                        itemDto.ProductName = product.ProductName;
                        itemDto.ProductPrice = product.Price;

                    }
                   
                }
                // Xóa item khỏi DTO nếu SP không tìm thấy
                cartDto.CartItems.RemoveAll(item => !products.ContainsKey(item.ProductId));
            }

            return cartDto;
        }

        public async Task AddItemAsync(AddToCartInput input)
        {
            var product = await _productRepository.GetAsync(input.ProductId);
            var cart = await GetOrCreateCurrentUserCartAsync(); // Lấy hoặc tạo cart

            // Kiểm tra tồn kho (logic có thể phức tạp hơn nếu cộng dồn)
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == input.ProductId);
            int requestedQuantity = input.Quantity + (existingItem?.Quantity ?? 0); // Tổng số lượng yêu cầu
            if (product.StockCount < requestedQuantity)
            {
                throw new 
                    UserFriendlyException($"Not enough stock for '{product.ProductName}'." +
                    $" Available: {product.StockCount}, Requested total: {requestedQuantity}");
            }


            cart.AddOrUpdateItem(input.ProductId, input.Quantity, _guidGenerator,input.ProductName,input.ProductPrice);

            await _cartRepository.UpdateAsync(cart, autoSave: true); // Update sẽ tự động Insert nếu cart là mới
        }

        public async Task UpdateItemAsync(UpdateCartItemInput input)
        {
            var cart = await GetOrCreateCurrentUserCartAsync(); // Lấy cart
            var itemToUpdate = cart.Items.FirstOrDefault(i => i.Id == input.CartItemId);

            if (itemToUpdate == null)
            {
                throw new EntityNotFoundException(typeof(CartItem), input.CartItemId);
            }

            var product = await _productRepository.GetAsync(itemToUpdate.ProductId);
            if (product.StockCount < input.Quantity) { /* Lỗi tồn kho */ }

            // Cập nhật số lượng qua phương thức của CartItem (hoặc Cart nếu logic phức tạp hơn)
            itemToUpdate.SetQuantity(input.Quantity);

            // Lưu thay đổi vào Cart Aggregate Root
            await _cartRepository.UpdateAsync(cart, autoSave: true);
        }

        public async Task RemoveItemAsync(Guid cartItemId)
        {
            var cart = await GetOrCreateCurrentUserCartAsync(); // Lấy cart

            // Xóa item thông qua phương thức của Cart Aggregate Root
            cart.RemoveItem(cartItemId);

            // Lưu thay đổi vào Cart Aggregate Root
            await _cartRepository.UpdateAsync(cart, autoSave: true);
        }

        public async Task<int> GetItemCountAsync()
        {
            var cart = await GetOrCreateCurrentUserCartAsync(); // Lấy cart
            return cart.Items.Sum(i => i.Quantity);
        }

        public async Task ClearAsync()
        {
            var cart = await GetOrCreateCurrentUserCartAsync(); // Lấy cart

            // Xóa item thông qua phương thức của Cart Aggregate Root
            cart.ClearItems();

            // Lưu thay đổi vào Cart Aggregate Root
            await _cartRepository.UpdateAsync(cart, autoSave: true);
        }
    }
}
