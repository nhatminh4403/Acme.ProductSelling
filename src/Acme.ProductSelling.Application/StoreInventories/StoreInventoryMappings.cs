using Acme.ProductSelling.StoreInventories.Dtos;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.StoreInventories;


#region Store Inventory Mappings

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class StoreInventoryToStoreInventoryDtoMapper : MapperBase<StoreInventory, StoreInventoryDto>
{
    [MapperIgnoreTarget(nameof(StoreInventoryDto.StoreName))]
    [MapperIgnoreTarget(nameof(StoreInventoryDto.ProductName))]
    [MapperIgnoreTarget(nameof(StoreInventoryDto.ProductImageUrl))]
    [MapperIgnoreTarget(nameof(StoreInventoryDto.NeedsReorder))]
    public override partial StoreInventoryDto Map(StoreInventory source);

    [MapperIgnoreTarget(nameof(StoreInventoryDto.StoreName))]
    [MapperIgnoreTarget(nameof(StoreInventoryDto.ProductName))]
    [MapperIgnoreTarget(nameof(StoreInventoryDto.ProductImageUrl))]
    [MapperIgnoreTarget(nameof(StoreInventoryDto.NeedsReorder))]
    public override partial void Map(StoreInventory source, StoreInventoryDto destination);

    public override void AfterMap(StoreInventory source, StoreInventoryDto destination)
    {
        destination.NeedsReorder = source.Quantity <= source.ReorderLevel;
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateStoreInventoryDtoToStoreInventoryMapper : MapperBase<CreateUpdateStoreInventoryDto, StoreInventory>
{
    public override StoreInventory Map(CreateUpdateStoreInventoryDto source)
    {
        // 1. Construct with mandatory constructor arguments
        var entity = new StoreInventory(
            Guid.NewGuid(), // Generate new ID
            source.StoreId,
            source.ProductId,
            source.Quantity,
            source.ReorderLevel,
            source.ReorderQuantity
        );

        // 2. Call the update method to map remaining properties (like IsAvailableForSale)
        Map(source, entity);

        return entity;
    }

    [MapperIgnoreTarget(nameof(StoreInventory.Id))]
    [MapperIgnoreTarget(nameof(StoreInventory.Store))]
    [MapperIgnoreTarget(nameof(StoreInventory.Product))]
    [MapperIgnoreTarget(nameof(StoreInventory.Quantity))]        // Mapped via Constructor
    [MapperIgnoreTarget(nameof(StoreInventory.ReorderLevel))]    // Mapped via Constructor
    [MapperIgnoreTarget(nameof(StoreInventory.ReorderQuantity))] // Mapped via Constructor
    [MapperIgnoreTarget(nameof(StoreInventory.StoreId))]         // Mapped via Constructor
    [MapperIgnoreTarget(nameof(StoreInventory.ProductId))]
    public override partial void Map(CreateUpdateStoreInventoryDto source, StoreInventory destination);
}

#endregion