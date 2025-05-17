using Acme.ProductSelling.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
            try
            {
                var userId = GetCurrentUserId();
                // Include Items để thao tác trực tiếp
                var cart = await (await _cartRepository.WithDetailsAsync(c => c.Items))
                                .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    // Tạo giỏ hàng mới nếu chưa có
                    cart = new Cart(_guidGenerator.Create(), userId);
                    await _cartRepository.InsertAsync(cart, autoSave: true);

                    // Tải lại để đảm bảo có tất cả thuộc tính
                    cart = await (await _cartRepository.WithDetailsAsync(c => c.Items))
                                .FirstOrDefaultAsync(c => c.Id == cart.Id);
                }

                return cart;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetOrCreateCurrentUserCartAsync: {ex.Message}");
                throw new UserFriendlyException("Could not access your shopping cart. Please try again later.");
            }
        }

        public async Task<CartDto> GetAsync()
        {
            try
            {
                var cart = await GetOrCreateCurrentUserCartAsync();

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
                            if (product.DiscountedPrice == null)
                            {
                                itemDto.ProductName = product.ProductName;
                                itemDto.ProductPrice = product.DiscountedPrice.Value;
                            }
                            else
                            {
                                itemDto.ProductName = product.ProductName;
                                itemDto.ProductPrice = product.OriginalPrice;
                            }
                        }
                    }
                    // Xóa item khỏi DTO nếu SP không tìm thấy
                    cartDto.CartItems.RemoveAll(item => !products.ContainsKey(item.ProductId));
                }

                return cartDto;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetAsync: {ex.Message}");
                throw new UserFriendlyException("Failed to retrieve shopping cart. Please try again.");
            }
        }

        public async Task AddItemAsync(AddToCartInput input)
        {
            try
            {
                // Kiểm tra input
                if (input == null)
                {
                    throw new ArgumentNullException(nameof(input));
                }

                if (input.ProductId == Guid.Empty)
                {
                    throw new UserFriendlyException("Invalid product ID.");
                }

                if (input.Quantity <= 0)
                {
                    throw new UserFriendlyException("Quantity must be greater than zero.");
                }

                // Kiểm tra sản phẩm tồn tại
                var product = await _productRepository.GetAsync(input.ProductId);
                var cart = await GetOrCreateCurrentUserCartAsync(); // Lấy hoặc tạo cart

                // Kiểm tra tồn kho
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == input.ProductId);
                int requestedQuantity = input.Quantity + (existingItem?.Quantity ?? 0); // Tổng số lượng yêu cầu

                if (product.StockCount < requestedQuantity)
                {
                    throw new UserFriendlyException($"Not enough stock for '{product.ProductName}'." +
                        $" Available: {product.StockCount}, Requested total: {requestedQuantity}");
                }
                if (product.DiscountedPrice == null)
                {
                    cart.AddOrUpdateItem(
                        input.ProductId,
                        input.Quantity,
                        _guidGenerator,
                        product.ProductName,
                        product.OriginalPrice
                    );
                }
                else
                {
                    cart.AddOrUpdateItem(
                        input.ProductId,
                        input.Quantity,
                        _guidGenerator,
                        product.ProductName,
                        product.DiscountedPrice.Value
                    );
                }
                    // Thêm hoặc cập nhật item trong giỏ hàng


                    await _cartRepository.UpdateAsync(cart, autoSave: true);
            }
            catch (EntityNotFoundException)
            {
                throw new UserFriendlyException("The product you are trying to add does not exist.");
            }
            catch (UserFriendlyException)
            {
                throw; // Re-throw UserFriendlyException
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in AddItemAsync: {ex.Message}");
                throw new UserFriendlyException("Could not add item to cart. Please try again later.");
            }
        }

        public async Task UpdateItemAsync(UpdateCartItemInput input)
        {
            try
            {
                var cart = await GetOrCreateCurrentUserCartAsync(); // Lấy cart
                var itemToUpdate = cart.Items.FirstOrDefault(i => i.Id == input.CartItemId);

                if (itemToUpdate == null)
                {
                    throw new EntityNotFoundException(typeof(CartItem), input.CartItemId);
                }

                var product = await _productRepository.GetAsync(itemToUpdate.ProductId);
                if (product.StockCount < input.Quantity)
                {
                    throw new UserFriendlyException($"Not enough stock for '{product.ProductName}'." +
                        $" Available: {product.StockCount}, Requested: {input.Quantity}");
                }

                // Cập nhật số lượng qua phương thức của CartItem (hoặc Cart nếu logic phức tạp hơn)
                itemToUpdate.SetQuantity(input.Quantity);

                // Lưu thay đổi vào Cart Aggregate Root
                await _cartRepository.UpdateAsync(cart, autoSave: true);
            }
            catch (EntityNotFoundException)
            {
                throw new UserFriendlyException("The item you are trying to update no longer exists in your cart.");
            }
            catch (UserFriendlyException)
            {
                throw; // Re-throw UserFriendlyException
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in UpdateItemAsync: {ex.Message}");
                throw new UserFriendlyException("Could not update item in cart. Please try again later.");
            }
        }

        public async Task RemoveItemAsync(Guid cartItemId)
        {
            try
            {
                var cart = await GetOrCreateCurrentUserCartAsync(); // Lấy cart

                // Xóa item thông qua phương thức của Cart Aggregate Root
                cart.RemoveItem(cartItemId);

                // Lưu thay đổi vào Cart Aggregate Root
                await _cartRepository.UpdateAsync(cart, autoSave: true);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in RemoveItemAsync: {ex.Message}");
                throw new UserFriendlyException("Could not remove item from cart. Please try again later.");
            }
        }

        public async Task<int> GetItemCountAsync()
        {
            try
            {
                var cart = await GetOrCreateCurrentUserCartAsync(); // Lấy cart
                return cart.Items.Sum(i => i.Quantity);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetItemCountAsync: {ex.Message}");
                // Trả về 0 thay vì ném lỗi để trải nghiệm người dùng tốt hơn
                return 0;
            }
        }

        public async Task ClearAsync()
        {
            try
            {
                var cart = await GetOrCreateCurrentUserCartAsync(); // Lấy cart

                // Xóa item thông qua phương thức của Cart Aggregate Root
                cart.ClearItems();

                // Lưu thay đổi vào Cart Aggregate Root
                await _cartRepository.UpdateAsync(cart, autoSave: true);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in ClearAsync: {ex.Message}");
                throw new UserFriendlyException("Could not clear your cart. Please try again later.");
            }
        }
    }
}