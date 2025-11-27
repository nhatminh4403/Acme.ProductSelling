using Acme.ProductSelling.Specifications.Lookups.DTOs;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Products.Lookups;


#region Lookup Mappings

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CpuSocketToProductLookupDtoMapper : MapperBase<CpuSocket, ProductLookupDto<Guid>>
{
    public override partial ProductLookupDto<Guid> Map(CpuSocket source);
    public override partial void Map(CpuSocket source, ProductLookupDto<Guid> destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ChipsetToProductLookupDtoMapper : MapperBase<Chipset, ProductLookupDto<Guid>>
{
    public override partial ProductLookupDto<Guid> Map(Chipset source);
    public override partial void Map(Chipset source, ProductLookupDto<Guid> destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RamTypeToProductLookupDtoMapper : MapperBase<RamType, ProductLookupDto<Guid>>
{
    public override partial ProductLookupDto<Guid> Map(RamType source);
    public override partial void Map(RamType source, ProductLookupDto<Guid> destination);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SwitchTypeToProductLookupDtoMapper : MapperBase<SwitchType, ProductLookupDto<Guid>>
{
    public override partial ProductLookupDto<Guid> Map(SwitchType source);
    public override partial void Map(SwitchType source, ProductLookupDto<Guid> destination);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class FormFactorToProductLookupDtoMapper : MapperBase<FormFactor, ProductLookupDto<Guid>>
{
    public override partial ProductLookupDto<Guid> Map(FormFactor source);
    public override partial void Map(FormFactor source, ProductLookupDto<Guid> destination);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class MaterialToProductLookupDtoMapper : MapperBase<Material, ProductLookupDto<Guid>>
{
    public override partial ProductLookupDto<Guid> Map(Material source);
    public override partial void Map(Material source, ProductLookupDto<Guid> destination);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class PanelTypeToProductLookupDtoMapper : MapperBase<PanelType, ProductLookupDto<Guid>>
{
    public override partial ProductLookupDto<Guid> Map(PanelType source);
    public override partial void Map(PanelType source, ProductLookupDto<Guid> destination);
}
#endregion
