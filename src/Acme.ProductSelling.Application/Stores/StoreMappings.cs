using Acme.ProductSelling.Stores.Dtos;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Stores;

#region Store Mappings

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
[MapExtraProperties]
public partial class StoreToStoreDtoMapper : MapperBase<Store, StoreDto>
{
    public override partial StoreDto Map(Store source);
    public override partial void Map(Store source, StoreDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateUpdateStoreDtoToStoreMapper : MapperBase<CreateUpdateStoreDto, Store>
{
    public override Store Map(CreateUpdateStoreDto source)
    {
        // 1. Construct with mandatory args
        var entity = new Store(
            Guid.NewGuid(),
            source.Name,
            source.Code,
            source.Address,
            source.City,
            source.PhoneNumber
        );

        // 2. Map remaining properties
        Map(source, entity);

        return entity;
    }    
    // Ignore properties already handled by Constructor in the update method prevents accidental overwrites or redundant checks,
    // though for strings it usually doesn't hurt. strictly enforcing ignores below:
    [MapperIgnoreTarget(nameof(Store.Id))]
    [MapperIgnoreTarget(nameof(Store.IsActive))]
    public override partial void Map(CreateUpdateStoreDto source, Store destination);
}

#endregion