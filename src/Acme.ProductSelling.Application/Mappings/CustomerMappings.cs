using Acme.ProductSelling.Account.Dtos;
using Acme.ProductSelling.Users;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Mapperly;

namespace Acme.ProductSelling.Mappings;


[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AddressToAddressDtoMapper : MapperBase<Address, AddressDto>
{
    public override partial AddressDto Map(Address source);
    public override partial void Map(Address source, AddressDto destination);
}