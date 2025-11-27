using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Manufacturers;

#region Manufacturer Mappings

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class ManufacturerToManufacturerDtoMapper : MapperBase<Manufacturer, ManufacturerDto>
{
    public override partial ManufacturerDto Map(Manufacturer source);
    public override partial void Map(Manufacturer source, ManufacturerDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateManufacturerDtoToManufacturerMapper : MapperBase<CreateUpdateManufacturerDto, Manufacturer>
{
    public override partial Manufacturer Map(CreateUpdateManufacturerDto source);
    public override partial void Map(CreateUpdateManufacturerDto source, Manufacturer destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ManufacturerToManufacturerLookupDtoMapper : MapperBase<Manufacturer, ManufacturerLookupDto>
{
    public override partial ManufacturerLookupDto Map(Manufacturer source);
    public override partial void Map(Manufacturer source, ManufacturerLookupDto destination);
}

#endregion