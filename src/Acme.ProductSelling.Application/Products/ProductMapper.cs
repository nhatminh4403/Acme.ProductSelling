using System;
using Acme.ProductSelling.Products;
using Acme.ProductSelling.Products.Dtos;
using Acme.ProductSelling.Specifications;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Products;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class ProductMapper : MapperBase<Product, ProductDto>
{
    [MapProperty(nameof(Product.Category.Name), nameof(ProductDto.CategoryName))]
    [MapProperty(nameof(Product.Manufacturer.Name), nameof(ProductDto.ManufacturerName))]
    [MapperIgnoreTarget(nameof(ProductDto.StoreAvailability))]
    public override partial ProductDto Map(Product source);

    // AfterMap for calculated logic found in original AutoMapper profile
    public override void AfterMap(Product source, ProductDto destination)
    {
        destination.IsAvailableForPurchase = source.StockCount > 0
            && (!source.ReleaseDate.HasValue || source.ReleaseDate.Value <= DateTime.Now);
    }

    public override void Map(Product source, ProductDto destination)
    {
        throw new NotImplementedException();
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProductToCreateUpdateMapper : MapperBase<ProductDto, CreateUpdateProductDto>
{
    [MapperIgnoreTarget(nameof(CreateUpdateProductDto.ProductImageFile))]
    [MapperIgnoreTarget(nameof(CreateUpdateProductDto.ReleaseDate))] // Explicitly ignored in original profile
    public override partial CreateUpdateProductDto Map(ProductDto source);

    public override void Map(ProductDto source, CreateUpdateProductDto destination)
    {
        throw new NotImplementedException();
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateProductMapper : MapperBase<CreateUpdateProductDto, Product>
{
    [MapperIgnoreTarget(nameof(Product.Id))]
    [MapperIgnoreTarget(nameof(Product.Category))]
    [MapperIgnoreTarget(nameof(Product.Manufacturer))]
    [MapperIgnoreTarget(nameof(Product.IsActive))]
    [MapperIgnoreTarget(nameof(Product.StoreInventories))]
    [MapperIgnoreTarget(nameof(Product.OriginalPrice))] // Set via property/method usually but keeping basic map
    [MapperIgnoreTarget(nameof(Product.DiscountedPrice))] // Calculated property in entity
    [MapperIgnoreTarget(nameof(Product.DiscountPercent))] // Calculated property in entity logic

    // Ignored in original profile or ignored because handled by sub-properties logic inside Mapperly
    // Mapperly will map nested MonitorSpecification automatically if it finds the method in [UseMapper]
    // The explicit Ignore in AutoMapper profile suggested it might be handling it manually or avoiding overwrite
    // Here we let Mapperly map them if the properties match, unless strict manual handling is required.
    // However, adhering to original profile instructions where specifications were ignored in specific map:
    // "ForMember(dest => dest.MonitorSpecification, opt => opt.Ignore())" etc.

    [MapperIgnoreTarget(nameof(Product.MonitorSpecification))]
    [MapperIgnoreTarget(nameof(Product.MouseSpecification))]
    [MapperIgnoreTarget(nameof(Product.LaptopSpecification))]
    [MapperIgnoreTarget(nameof(Product.CpuSpecification))]
    [MapperIgnoreTarget(nameof(Product.GpuSpecification))]
    [MapperIgnoreTarget(nameof(Product.RamSpecification))]
    [MapperIgnoreTarget(nameof(Product.MotherboardSpecification))]
    [MapperIgnoreTarget(nameof(Product.StorageSpecification))]
    [MapperIgnoreTarget(nameof(Product.PsuSpecification))]
    [MapperIgnoreTarget(nameof(Product.CaseSpecification))]
    [MapperIgnoreTarget(nameof(Product.CpuCoolerSpecification))]
    [MapperIgnoreTarget(nameof(Product.KeyboardSpecification))]
    [MapperIgnoreTarget(nameof(Product.HeadsetSpecification))]
    [MapperIgnoreTarget(nameof(Product.CaseFanSpecification))]
    [MapperIgnoreTarget(nameof(Product.MemoryCardSpecification))]
    [MapperIgnoreTarget(nameof(Product.SpeakerSpecification))]
    [MapperIgnoreTarget(nameof(Product.MicrophoneSpecification))]
    [MapperIgnoreTarget(nameof(Product.WebcamSpecification))]
    [MapperIgnoreTarget(nameof(Product.MousepadSpecification))]
    [MapperIgnoreTarget(nameof(Product.ChairSpecification))]
    [MapperIgnoreTarget(nameof(Product.DeskSpecification))]
    [MapperIgnoreTarget(nameof(Product.SoftwareSpecification))]
    [MapperIgnoreTarget(nameof(Product.NetworkHardwareSpecification))]
    [MapperIgnoreTarget(nameof(Product.HandheldSpecification))]
    [MapperIgnoreTarget(nameof(Product.ConsoleSpecification))]
    [MapperIgnoreTarget(nameof(Product.HubSpecification))]
    [MapperIgnoreTarget(nameof(Product.CableSpecification))]
    [MapperIgnoreTarget(nameof(Product.ChargerSpecification))]
    [MapperIgnoreTarget(nameof(Product.PowerBankSpecification))]

    // Audited Props
    [MapperIgnoreTarget(nameof(Product.ExtraProperties))]
    [MapperIgnoreTarget(nameof(Product.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Product.CreationTime))]
    [MapperIgnoreTarget(nameof(Product.CreatorId))]
    [MapperIgnoreTarget(nameof(Product.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Product.LastModifierId))]
    [MapperIgnoreTarget(nameof(Product.IsDeleted))]
    [MapperIgnoreTarget(nameof(Product.DeleterId))]
    [MapperIgnoreTarget(nameof(Product.DeletionTime))]
    public override partial Product Map(CreateUpdateProductDto source);

    public override void Map(CreateUpdateProductDto source, Product destination)
    {
        throw new NotImplementedException();
    }
}