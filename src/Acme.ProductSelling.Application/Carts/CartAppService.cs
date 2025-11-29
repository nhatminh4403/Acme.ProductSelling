using Acme.ProductSelling.Permissions;
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
using Acme.ProductSelling.Products.Services;
using System.Linq.Dynamic.Core;

namespace Acme.ProductSelling.Carts;

[Authorize]
public class CartAppService : ApplicationService, ICartAppService
{
    private readonly IRepository<Cart, Guid> _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IGuidGenerator _guidGenerator;
    private readonly CartToCartDtoMapper _cartMapper;
    private readonly CartItemToCartItemDtoMapper _cartItemMapper;
    private readonly ILogger<CartAppService> _logger;

    public CartAppService(
                IRepository<Cart, Guid> cartRepository,
                IProductRepository productRepository,
                ICurrentUser currentUser,
                IGuidGenerator guidGenerator,
                CartToCartDtoMapper cartMapper,
                CartItemToCartItemDtoMapper cartItemMapper,
                ILogger<CartAppService> logger)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _currentUser = currentUser;
        _guidGenerator = guidGenerator;
        _cartMapper = cartMapper;
        _cartItemMapper = cartItemMapper;
        _logger = logger;
    }

    public async Task<CartDto> AddItemToCartAsync(AddToCartInput input)
    {
        var userId = CurrentUser.GetId();

        // Validate product exists and get its details
        var product = await _productRepository.GetAsync(input.ProductId);

        if (product == null)
        {
            throw new UserFriendlyException($"Product with ID {input.ProductId} not found.");
        }

        // Check if product is available for purchase
        if (!product.IsAvailableForPurchase())
        {
            throw new UserFriendlyException($"Product '{product.ProductName}' is not available for purchase.");
        }

        // Check stock availability
        if (product.StockCount < input.Quantity)
        {
            throw new UserFriendlyException($"Insufficient stock. Only {product.StockCount} items available.");
        }

        // Get or create the user's cart
        var cart = await GetOrCreateCartAsync(userId);

        // Use discounted price if available, otherwise use original price
        var effectivePrice = product.DiscountedPrice ?? product.OriginalPrice;

        // Add or update the item in the cart
        cart.AddOrUpdateItem(
            product.Id,
            input.Quantity,
            _guidGenerator,
            product.ProductName,
            effectivePrice
        );

        // Save the cart
        await _cartRepository.UpdateAsync(cart, autoSave: true);

        return await MapCartToDtoAsync(cart);
    }

    public async Task ClearCartAsync()
    {
        var userId = CurrentUser.GetId();

        var queryable = await _cartRepository.GetQueryableAsync();
        var cart = await queryable
            .Where(c => c.UserId == userId)
            .Include(c => c.Items)
            .FirstOrDefaultAsync();

        if (cart != null)
        {
            cart.ClearItems();
            await _cartRepository.UpdateAsync(cart, autoSave: true);
        }
    }

    public async Task<int> GetItemCountAsync()
    {
        var userId = CurrentUser.GetId();
        var queryable = await _cartRepository.GetQueryableAsync();
        var cart = await queryable
            .Where(c => c.UserId == userId)
            .Include(c => c.Items)
            .FirstOrDefaultAsync();

        return cart?.Items.Sum(i => i.Quantity) ?? 0;
    }

    public async Task<CartDto> GetUserCartAsync()
    {
        var userId = CurrentUser.GetId();
        var cart = await GetOrCreateCartAsync(userId);

        return await MapCartToDtoAsync(cart);
    }

    public async Task<CartDto> RemoveCartItemAsync(Guid cartItemId)
    {
        var userId = CurrentUser.GetId();

        var queryable = await _cartRepository.GetQueryableAsync();
        var cart = await queryable
            .Where(c => c.UserId == userId)
            .Include(c => c.Items)
            .FirstOrDefaultAsync();

        if (cart == null)
        {
            throw new UserFriendlyException("Cart not found.");
        }

        cart.RemoveItem(cartItemId);

        await _cartRepository.UpdateAsync(cart, autoSave: true);

        return await MapCartToDtoAsync(cart);
    }

    public async Task<CartDto> UpdateCartItemAsync(UpdateCartItemInput input)
    {
        var userId = CurrentUser.GetId();

        var queryable = await _cartRepository.GetQueryableAsync();
        var cart = await queryable
            .Where(c => c.UserId == userId)
            .Include(c => c.Items)
            .FirstOrDefaultAsync();

        if (cart == null)
        {
            throw new UserFriendlyException("Cart not found.");
        }

        var cartItem = cart.Items.FirstOrDefault(i => i.Id == input.CartItemId);

        if (cartItem == null)
        {
            throw new UserFriendlyException("Cart item not found.");
        }

        cartItem.SetQuantity(input.Quantity);

        await _cartRepository.UpdateAsync(cart, autoSave: true);

        return await MapCartToDtoAsync(cart);
    }



    private async Task<Cart> GetOrCreateCartAsync(Guid userId)
    {
        var queryable = await _cartRepository.GetQueryableAsync();
        var cart = await queryable
                        .Where(c => c.UserId == userId)
                        .Include(c => c.Items)
                        .FirstOrDefaultAsync();

        if (cart == null)
        {
            cart = new Cart(_guidGenerator.Create(), userId);
            await _cartRepository.InsertAsync(cart, autoSave: true);
        }

        return cart;
    }

    private async Task<CartDto> MapCartToDtoAsync(Cart cart)
    {
        var cartDto = _cartMapper.Map(cart);

        var productIds = cart.Items.Select(i => i.ProductId).ToList();

        var productQueryable = await _productRepository.GetQueryableAsync();
        var products = await productQueryable
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p);

        foreach (var itemDto in cartDto.CartItems)
        {
            if (products.TryGetValue(itemDto.ProductId, out var product))
            {
                itemDto.ProductUrlSlug = product.UrlSlug;
            }
        }

        return cartDto;
    }
}