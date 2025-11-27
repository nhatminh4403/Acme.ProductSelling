using Acme.ProductSelling.Manufacturers;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Manufacturers;

#region Manufacturer Mappers

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ManufacturerMapper : MapperBase<Manufacturer, ManufacturerDto>
{
    public override partial ManufacturerDto Map(Manufacturer source);
    public override partial void Map(Manufacturer source, ManufacturerDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateManufacturerMapper : MapperBase<CreateUpdateManufacturerDto, Manufacturer>
{
    [MapperIgnoreTarget(nameof(Manufacturer.Id))]
    [MapperIgnoreTarget(nameof(Manufacturer.Products))]
    //[MapperIgnoreTarget(nameof(Manufacturer.ExtraProperties))]
    //[MapperIgnoreTarget(nameof(Manufacturer.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Manufacturer.CreationTime))]
    [MapperIgnoreTarget(nameof(Manufacturer.CreatorId))]
    [MapperIgnoreTarget(nameof(Manufacturer.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Manufacturer.LastModifierId))]
    [MapperIgnoreTarget(nameof(Manufacturer.IsDeleted))]
    [MapperIgnoreTarget(nameof(Manufacturer.DeleterId))]
    [MapperIgnoreTarget(nameof(Manufacturer.DeletionTime))]
    public override partial Manufacturer Map(CreateUpdateManufacturerDto source);

    [MapperIgnoreTarget(nameof(Manufacturer.Id))]
    [MapperIgnoreTarget(nameof(Manufacturer.Products))]
    //[MapperIgnoreTarget(nameof(Manufacturer.ExtraProperties))]
    //[MapperIgnoreTarget(nameof(Manufacturer.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Manufacturer.CreationTime))]
    [MapperIgnoreTarget(nameof(Manufacturer.CreatorId))]
    [MapperIgnoreTarget(nameof(Manufacturer.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Manufacturer.LastModifierId))]
    [MapperIgnoreTarget(nameof(Manufacturer.IsDeleted))]
    [MapperIgnoreTarget(nameof(Manufacturer.DeleterId))]
    [MapperIgnoreTarget(nameof(Manufacturer.DeletionTime))]
    public override partial void Map(CreateUpdateManufacturerDto source, Manufacturer destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ManufacturerLookupMapper : MapperBase<Manufacturer, ManufacturerLookupDto>
{
    public override partial ManufacturerLookupDto Map(Manufacturer source);

    public override void Map(Manufacturer source, ManufacturerLookupDto destination)
    {
        throw new System.NotImplementedException();
    }
}
#endregion
