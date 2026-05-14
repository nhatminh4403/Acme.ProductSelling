using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Specifications;
using Acme.ProductSelling.Specifications.Models;
using Riok.Mapperly.Abstractions;
using System.Linq;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Products;

#region Product Mappings 

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class ProductToProductDtoMapper : MapperBase<Product, ProductDto>
{
    private readonly SpecificationBaseToDtoMapper _specMapper;

    public ProductToProductDtoMapper(SpecificationBaseToDtoMapper specMapper)
    {
        _specMapper = specMapper;
    }
    public override ProductDto Map(Product source)
    {
        var dto = MapInternal(source);

        AfterMap(source, dto);

        return dto;
    }

    public override void Map(Product source, ProductDto destination)
    {
        MapInternal(source, destination);
        AfterMap(source, destination);
    }



    [MapProperty(nameof(Product.Category.Name), nameof(ProductDto.CategoryName))]
    [MapProperty(nameof(Product.Manufacturer.Name), nameof(ProductDto.ManufacturerName))]
    [MapProperty(nameof(Product.Category.SpecificationType), nameof(ProductDto.CategorySpecificationType))]

    [MapperIgnoreTarget(nameof(ProductDto.IsAvailableForPurchase))]
    [MapperIgnoreTarget(nameof(ProductDto.TotalStockAcrossAllStores))]
    [MapperIgnoreTarget(nameof(ProductDto.StoreAvailability))]
    [MapperIgnoreTarget(nameof(ProductDto.SpecificationBase))]
    private partial ProductDto MapInternal(Product source);
    [MapProperty(nameof(Product.Category.Name), nameof(ProductDto.CategoryName))]
    [MapProperty(nameof(Product.Manufacturer.Name), nameof(ProductDto.ManufacturerName))]
    [MapProperty(nameof(Product.Category.SpecificationType), nameof(ProductDto.CategorySpecificationType))]
    [MapperIgnoreTarget(nameof(ProductDto.IsAvailableForPurchase))]
    [MapperIgnoreTarget(nameof(ProductDto.TotalStockAcrossAllStores))]
    [MapperIgnoreTarget(nameof(ProductDto.StoreAvailability))]
    [MapperIgnoreTarget(nameof(ProductDto.SpecificationBase))]
    private partial void MapInternal(Product source, ProductDto destination);

    public override void AfterMap(Product source, ProductDto destination)
    {
        destination.IsAvailableForPurchase = source.IsAvailableForPurchase();

        destination.TotalStockAcrossAllStores = source.StoreInventories?.Sum(x => x.Quantity) ?? 0;
        if (source.SpecificationBase != null)
        {
            destination.SpecificationBase = _specMapper.Map(source.SpecificationBase);
        }
    }
}


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProductDtoToCreateUpdateProductDtoMapper : MapperBase<ProductDto, CreateUpdateProductDto>
{
    [MapperIgnoreTarget(nameof(CreateUpdateProductDto.ProductImageFile))]
    [MapperIgnoreTarget(nameof(CreateUpdateProductDto.ReleaseDate))]
    public override partial CreateUpdateProductDto Map(ProductDto source);

    [MapperIgnoreTarget(nameof(CreateUpdateProductDto.ProductImageFile))]
    [MapperIgnoreTarget(nameof(CreateUpdateProductDto.ReleaseDate))]
    public override partial void Map(ProductDto source, CreateUpdateProductDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateProductDtoToProductMapper : MapperBase<CreateUpdateProductDto, Product>
{
    [MapperIgnoreTarget(nameof(Product.Category))]
    [MapperIgnoreTarget(nameof(Product.Manufacturer))]
    [MapperIgnoreTarget(nameof(Product.IsActive))]
    [MapperIgnoreTarget(nameof(Product.StoreInventories))]
    [MapperIgnoreTarget(nameof(Product.SpecificationBase))]

    public override partial Product Map(CreateUpdateProductDto source);

    [MapperIgnoreTarget(nameof(Product.Category))]
    [MapperIgnoreTarget(nameof(Product.Manufacturer))]
    [MapperIgnoreTarget(nameof(Product.IsActive))]
    [MapperIgnoreTarget(nameof(Product.StoreInventories))]
    [MapperIgnoreTarget(nameof(Product.SpecificationBase))]
    public override partial void Map(CreateUpdateProductDto source, Product destination);
}

#endregion