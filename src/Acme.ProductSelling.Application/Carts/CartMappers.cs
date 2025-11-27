using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Carts;


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CartMapper : MapperBase<Cart, CartDto>
{
    [MapProperty(nameof(Cart.Items), nameof(CartDto.CartItems))]
    [MapperIgnoreTarget(nameof(CartDto.TotalPrice))] // Computed getter in Dto
    [MapperIgnoreTarget(nameof(CartDto.TotalItemCount))] // Computed getter in Dto
    public override partial CartDto Map(Cart source);

    public override void Map(Cart source, CartDto destination)
    {
        throw new NotImplementedException();
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CartItemMapper : MapperBase<CartItem, CartItemDto>
{
    [MapperIgnoreTarget(nameof(CartItemDto.LineTotal))] // Computed
    [MapperIgnoreTarget(nameof(CartItemDto.ProductUrlSlug))] // Need separate enrichment usually
    public override partial CartItemDto Map(CartItem source);

    public override void Map(CartItem source, CartItemDto destination)
    {
        throw new NotImplementedException();
    }
}