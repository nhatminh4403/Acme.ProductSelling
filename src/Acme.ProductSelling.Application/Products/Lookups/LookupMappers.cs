using Acme.ProductSelling.Products.Lookups;
using Acme.ProductSelling.Specifications.Lookups.DTOs;
using Riok.Mapperly.Abstractions;
using System;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Mappers.Lookups
{
    #region Lookup Mappers - Specifications to DTOs
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class CpuSocketToCpuSocketDtoMapper : MapperBase<CpuSocket, CpuSocketDto>
    {
        public override partial CpuSocketDto Map(CpuSocket source);
        public override partial void Map(CpuSocket source, CpuSocketDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class MaterialToMaterialDtoMapper : MapperBase<Material, MaterialDto>
    {
        public override partial MaterialDto Map(Material source);
        public override partial void Map(Material source, MaterialDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class ChipsetToChipsetDtoMapper : MapperBase<Chipset, ChipsetDto>
    {
        public override partial ChipsetDto Map(Chipset source);
        public override partial void Map(Chipset source, ChipsetDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class FormFactorToFormFactorDtoMapper : MapperBase<FormFactor, FormFactorDto>
    {
        public override partial FormFactorDto Map(FormFactor source);
        public override partial void Map(FormFactor source, FormFactorDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class PanelTypeToPanelTypeDtoMapper : MapperBase<PanelType, PanelTypeDto>
    {
        public override partial PanelTypeDto Map(PanelType source);
        public override partial void Map(PanelType source, PanelTypeDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class RamTypeToRamTypeDtoMapper : MapperBase<RamType, RamTypeDto>
    {
        public override partial RamTypeDto Map(RamType source);
        public override partial void Map(RamType source, RamTypeDto destination);
    }

    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    public partial class SwitchTypeToSwitchTypeDtoMapper : MapperBase<SwitchType, SwitchTypeDto>
    {
        public override partial SwitchTypeDto Map(SwitchType source);
        public override partial void Map(SwitchType source, SwitchTypeDto destination);
    }
    #endregion

    #region Lookup Mappers - To ProductLookupDto
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
    #endregion

}
