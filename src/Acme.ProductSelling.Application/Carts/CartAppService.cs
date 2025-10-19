using Acme.ProductSelling.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
    [Authorize] // Chỉ cho phép người dùng đã đăng nhập truy cập
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

            return _currentUser.Id ??
                throw new AbpAuthorizationException(L["Account:UserNotAuthenticated"]);
            //"User is not authenticated."
        }

        private async Task<Cart> GetOrCreateCurrentUserCartAsync()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cart = await (await _cartRepository.WithDetailsAsync(c => c.Items))
                                .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart(_guidGenerator.Create(), userId);
                    await _cartRepository.InsertAsync(cart, autoSave: true);

                    cart = await (await _cartRepository.WithDetailsAsync(c => c.Items))
                                .FirstOrDefaultAsync(c => c.Id == cart.Id);
                }

                return cart;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetOrCreateCurrentUserCartAsync: {ex.Message}");
                throw new UserFriendlyException(L["Cart:CartInccessible.TryAgain"]);
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
                            itemDto.ProductName = product.ProductName;

                            if (product.DiscountedPrice.HasValue)
                            {
                                itemDto.ProductPrice = product.DiscountedPrice.Value;
                            }
                            else
                            {
                                itemDto.ProductPrice = product.OriginalPrice;
                            }
                            itemDto.ProductUrlSlug = product.UrlSlug;
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
                throw new UserFriendlyException(L["Cart:CartFailedToRetrieve.TryAgain"]);
            }//"Failed to retrieve shopping cart. Please try again."
        }
        [IgnoreAntiforgeryToken]
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
                    throw new UserFriendlyException(L["Product:ID:Invalid"]);
                    //"Invalid product ID."
                    // "Mã sản phẩm không hợp lệ."
                }

                if (input.Quantity <= 0)
                {
                    throw new UserFriendlyException(L["Product:Quantity:GreaterThanZero"]);
                    // "Số lượng phải lớn hơn 0."
                    //"Quantity must be greater than zero."
                }

                // Kiểm tra sản phẩm tồn tại
                var product = await _productRepository.GetAsync(input.ProductId);
                var cart = await GetOrCreateCurrentUserCartAsync(); // Lấy hoặc tạo cart

                // Kiểm tra tồn kho
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == input.ProductId);
                int requestedQuantity = input.Quantity + (existingItem?.Quantity ?? 0); // Tổng số lượng yêu cầu

                if (product.StockCount < requestedQuantity)
                {
                    // Nếu tồn kho không đủ, ném ngoại lệ
                    var errorMessage = L["Product:Stock:NotEnough", product.ProductName, product.StockCount, requestedQuantity];

                    throw new UserFriendlyException(errorMessage);
                    /*  en   {
                        "NotEnoughStock": "Not enough stock for '{0}'. Available: {1}, Requested: {2}"
                    }
                   vi  "NotEnoughStock": "Không đủ hàng cho '{0}'. Có sẵn: {1}, Yêu cầu: {2}"*/


                }
                var priceToUse = product.DiscountedPrice ?? product.OriginalPrice;
                cart.AddOrUpdateItem(
                      input.ProductId,
                      input.Quantity,
                      _guidGenerator,
                      product.ProductName,
                      priceToUse
                 );
                // Thêm hoặc cập nhật item trong giỏ hàng


                await _cartRepository.UpdateAsync(cart, autoSave: true);
            }
            catch (EntityNotFoundException)
            {
                throw new UserFriendlyException(L["Product:Nonexistent:Add"]);
                // "Sản phẩm bạn đang cố gắng thêm không tồn tại."
                //"The product you are trying to add does not exist."
            }
            catch (UserFriendlyException)
            {
                throw; // Re-throw UserFriendlyException
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in AddItemAsync: {ex.Message}");

                throw new UserFriendlyException(L["Product:CanNotAddToCart.TryAgain"]);
                // "Không thể thêm sản phẩm vào giỏ hàng. Vui lòng thử lại sau."
                //"Could not add product to cart. Please try again later."
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
                    throw new UserFriendlyException(
                        L["Product:Stock:NotEnoughStock", product.ProductName, product.StockCount, input.Quantity]
                    );
                    /*  en   {
                             "NotEnoughStock": "Not enough stock for '{0}'. Available: {1}, Requested: {2}"
                         }
                        vi  "NotEnoughStock": "Không đủ hàng cho '{0}'. Có sẵn: {1}, Yêu cầu: {2}"*/
                }

                // Cập nhật số lượng qua phương thức của CartItem (hoặc Cart nếu logic phức tạp hơn)
                itemToUpdate.SetQuantity(input.Quantity);

                await _cartRepository.UpdateAsync(cart, autoSave: true);
            }
            catch (EntityNotFoundException)
            {
                throw new UserFriendlyException(L["Product:Nonexistent:Update"]);
                // "Mặt hàng bạn đang cố gắng cập nhật không còn tồn tại trong giỏ hàng của bạn."
                //"The item you are trying to update no longer exists in your cart."
            }
            catch (UserFriendlyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in UpdateItemAsync: {ex.Message}");
                throw new UserFriendlyException(L["Cart:Item:UnableToUpdate.TryAgain"]);

                // "Không thể cập nhật mặt hàng trong giỏ hàng. Vui lòng thử lại sau."
                //"Could not update item in cart. Please try again later."
            }
        }

        public async Task RemoveItemAsync(Guid cartItemId)
        {
            try
            {
                var cart = await GetOrCreateCurrentUserCartAsync();

                cart.RemoveItem(cartItemId);

                await _cartRepository.UpdateAsync(cart, autoSave: true);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in RemoveItemAsync: {ex.Message}");
                throw new UserFriendlyException(L["Cart:Item:UnableToRemove.TryAgain"]);
                // "Không thể xóa mặt hàng khỏi giỏ hàng. Vui lòng thử lại sau."
                //"Could not remove item from cart. Please try again later."
            }
        }

        public async Task<int> GetItemCountAsync()
        {
            try
            {
                var cart = await GetOrCreateCurrentUserCartAsync();
                return cart.Items.Sum(i => i.Quantity);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in GetItemCountAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task ClearAsync()
        {
            try
            {
                var cart = await GetOrCreateCurrentUserCartAsync();

                cart.ClearItems();

                await _cartRepository.UpdateAsync(cart, autoSave: true);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error in ClearAsync: {ex.Message}");
                throw new UserFriendlyException(L["Cart:UnableToClear.TryAgain"]);
                // "Không thể xóa giỏ hàng của bạn. Vui lòng thử lại sau."
                // "Could not clear your cart. Please try again later."

            }
        }
    }
}