using Acme.ProductSelling.Products.Dtos;
using Riok.Mapperly.Abstractions;
using System;
using System.Linq;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Products;


#region Recently Viewed Mappings

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProductToRecentlyViewedProductDtoMapper : MapperBase<Product, RecentlyViewedProductDto>
{
    [MapProperty(nameof(Product.Id), nameof(RecentlyViewedProductDto.ProductId))]
    [MapperIgnoreTarget(nameof(RecentlyViewedProductDto.IsAvailableForPurchase))]
    [MapperIgnoreTarget(nameof(RecentlyViewedProductDto.TotalStockAcrossAllStores))]
    public override partial RecentlyViewedProductDto Map(Product source);

    public override partial void Map(Product source, RecentlyViewedProductDto destination);

    public override void AfterMap(Product source, RecentlyViewedProductDto destination)
    {
        destination.IsAvailableForPurchase = source.StockCount > 0 &&
            (!source.ReleaseDate.HasValue || source.ReleaseDate.Value <= DateTime.Now);
        destination.TotalStockAcrossAllStores = source.StoreInventories != null
            ? source.StoreInventories.Sum(x => x.Quantity) : 0;
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RecentlyViewedProductToDtoMapper : MapperBase<RecentlyViewedProduct, RecentlyViewedProductDto>
{
    [MapProperty(nameof(RecentlyViewedProduct.ProductId), nameof(RecentlyViewedProductDto.ProductId))]
    [MapProperty(nameof(RecentlyViewedProduct.Product.ProductName), nameof(RecentlyViewedProductDto.ProductName))]
    [MapProperty(nameof(RecentlyViewedProduct.Product.ImageUrl), nameof(RecentlyViewedProductDto.ImageUrl))]
    [MapProperty(nameof(RecentlyViewedProduct.Product.OriginalPrice), nameof(RecentlyViewedProductDto.OriginalPrice))]
    [MapProperty(nameof(RecentlyViewedProduct.Product.DiscountedPrice), nameof(RecentlyViewedProductDto.DiscountedPrice))]
    [MapProperty(nameof(RecentlyViewedProduct.Product.UrlSlug), nameof(RecentlyViewedProductDto.UrlSlug))]
    [MapProperty(nameof(RecentlyViewedProduct.Product.DiscountPercent), nameof(RecentlyViewedProductDto.DiscountPercent))]
    [MapperIgnoreTarget(nameof(RecentlyViewedProductDto.IsAvailableForPurchase))]
    [MapperIgnoreTarget(nameof(RecentlyViewedProductDto.TotalStockAcrossAllStores))]
    public override partial RecentlyViewedProductDto Map(RecentlyViewedProduct source);

    public override partial void Map(RecentlyViewedProduct source, RecentlyViewedProductDto destination);

    public override void AfterMap(RecentlyViewedProduct source, RecentlyViewedProductDto destination)
    {
        if (source.Product != null)
        {
            destination.IsAvailableForPurchase = source.Product.StockCount > 0 &&
                (!source.Product.ReleaseDate.HasValue || source.Product.ReleaseDate.Value <= DateTime.Now);
            destination.TotalStockAcrossAllStores = source.Product.StoreInventories != null
                ? source.Product.StoreInventories.Sum(x => x.Quantity) : 0;
        }
    }
}

#endregion