using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Carts;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CartToCartDtoMapper : MapperBase<Cart, CartDto>
{
    [MapProperty(nameof(Cart.Items), nameof(CartDto.CartItems))]
    public override partial CartDto Map(Cart source);
    public override partial void Map(Cart source, CartDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CartItemToCartItemDtoMapper : MapperBase<CartItem, CartItemDto>
{
    [MapperIgnoreTarget(nameof(CartItemDto.ProductUrlSlug))]
    public override partial CartItemDto Map(CartItem source);
    [MapperIgnoreTarget(nameof(CartItemDto.ProductUrlSlug))]
    public override partial void Map(CartItem source, CartItemDto destination);
}
