using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Carts;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CartToCartDtoMapper : MapperBase<Cart, CartDto>
{
    private readonly CartItemToCartItemDtoMapper _itemMapper = new();

    [MapProperty(nameof(Cart.Items), nameof(CartDto.CartItems))]
    public override partial CartDto Map(Cart source);

    public override partial void Map(Cart source, CartDto destination);

    // Helper method for collection mapping
    private CartItemDto MapItem(CartItem item) => _itemMapper.Map(item);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CartItemToCartItemDtoMapper : MapperBase<CartItem, CartItemDto>
{
    [MapperIgnoreTarget(nameof(CartItemDto.ProductUrlSlug))]
    public override partial CartItemDto Map(CartItem source);

    [MapperIgnoreTarget(nameof(CartItemDto.ProductUrlSlug))]
    public override partial void Map(CartItem source, CartItemDto destination);
}