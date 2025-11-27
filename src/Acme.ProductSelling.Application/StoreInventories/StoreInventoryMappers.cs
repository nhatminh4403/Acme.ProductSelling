using Acme.ProductSelling.StoreInventories.Dtos;
using Riok.Mapperly.Abstractions;
using System;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.StoreInventories;

#region StoreInventory Mappers

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class StoreInventoryMapper : MapperBase<StoreInventory, StoreInventoryDto>
{
    [MapperIgnoreTarget(nameof(StoreInventoryDto.StoreName))]
    [MapperIgnoreTarget(nameof(StoreInventoryDto.ProductName))]
    [MapperIgnoreTarget(nameof(StoreInventoryDto.ProductImageUrl))]
    [MapperIgnoreTarget(nameof(StoreInventoryDto.NeedsReorder))]
    public override partial StoreInventoryDto Map(StoreInventory source);

    public override void Map(StoreInventory source, StoreInventoryDto destination)
    {
        throw new NotImplementedException();
    }
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateStoreInventoryMapper : MapperBase<CreateUpdateStoreInventoryDto, StoreInventory>
{
    [ObjectFactory]
    StoreInventory Create(CreateUpdateStoreInventoryDto dto)
    {
        return new StoreInventory(
            Guid.NewGuid(),
            dto.StoreId,
            dto.ProductId,
            dto.Quantity,
            dto.ReorderLevel,
            dto.ReorderQuantity
        );
    }

    [MapperIgnoreTarget(nameof(StoreInventory.Id))]
    [MapperIgnoreTarget(nameof(StoreInventory.StoreId))]
    [MapperIgnoreTarget(nameof(StoreInventory.ProductId))]
    [MapperIgnoreTarget(nameof(StoreInventory.Quantity))]
    [MapperIgnoreTarget(nameof(StoreInventory.ReorderLevel))]
    [MapperIgnoreTarget(nameof(StoreInventory.ReorderQuantity))]

    [MapperIgnoreTarget(nameof(StoreInventory.Store))]
    [MapperIgnoreTarget(nameof(StoreInventory.Product))]
    [MapperIgnoreTarget(nameof(StoreInventory.ExtraProperties))]
    [MapperIgnoreTarget(nameof(StoreInventory.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(StoreInventory.CreationTime))]
    [MapperIgnoreTarget(nameof(StoreInventory.CreatorId))]
    [MapperIgnoreTarget(nameof(StoreInventory.LastModificationTime))]
    [MapperIgnoreTarget(nameof(StoreInventory.LastModifierId))]
    [MapperIgnoreTarget(nameof(StoreInventory.IsDeleted))]
    [MapperIgnoreTarget(nameof(StoreInventory.DeleterId))]
    [MapperIgnoreTarget(nameof(StoreInventory.DeletionTime))]
    public override partial StoreInventory Map(CreateUpdateStoreInventoryDto source);

    public override void Map(CreateUpdateStoreInventoryDto source, StoreInventory destination)
    {
        throw new NotImplementedException();
    }
}

#endregion
