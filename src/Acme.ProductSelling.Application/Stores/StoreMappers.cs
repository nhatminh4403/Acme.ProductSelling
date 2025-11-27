using System;
using Acme.ProductSelling.Stores;
using Acme.ProductSelling.Stores.Dtos;
using Acme.ProductSelling.StoreInventories;
using Acme.ProductSelling.StoreInventories.Dtos;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Stores;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class StoreMapper : MapperBase<Store, StoreDto>
{
    public override partial StoreDto Map(Store source);
    public override partial void Map(Store source, StoreDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateStoreMapper : MapperBase<CreateUpdateStoreDto, Store>
{
    // Define explicit constructor logic for Mapperly
    [ObjectFactory]
    Store Create(CreateUpdateStoreDto dto)
    {
        return new Store(
            Guid.NewGuid(),
            dto.Name,
            dto.Code,
            dto.Address ?? string.Empty,
            dto.City ?? string.Empty,
            dto.PhoneNumber
        );
    }

    // Explicitly ignore properties set via constructor to ensure clean mapping
    [MapperIgnoreTarget(nameof(Store.Id))]
    [MapperIgnoreTarget(nameof(Store.Name))]
    [MapperIgnoreTarget(nameof(Store.Code))]
    [MapperIgnoreTarget(nameof(Store.Address))]
    [MapperIgnoreTarget(nameof(Store.City))]
    [MapperIgnoreTarget(nameof(Store.PhoneNumber))]

    // Ignore auditing/internal props
    [MapperIgnoreTarget(nameof(Store.IsActive))]
    [MapperIgnoreTarget(nameof(Store.ExtraProperties))]
    [MapperIgnoreTarget(nameof(Store.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(Store.CreationTime))]
    [MapperIgnoreTarget(nameof(Store.CreatorId))]
    [MapperIgnoreTarget(nameof(Store.LastModificationTime))]
    [MapperIgnoreTarget(nameof(Store.LastModifierId))]
    [MapperIgnoreTarget(nameof(Store.IsDeleted))]
    [MapperIgnoreTarget(nameof(Store.DeleterId))]
    [MapperIgnoreTarget(nameof(Store.DeletionTime))]
    public override partial Store Map(CreateUpdateStoreDto source);

    public override void Map(CreateUpdateStoreDto source, Store destination)
    {
        throw new NotImplementedException();
    }
}
