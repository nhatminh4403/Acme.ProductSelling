using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Acme.ProductSelling.Account.Dtos
{
    public class AddressDto : EntityDto<Guid>
    {
        public string FullAddress { get; set; }
        public bool IsDefaultAddress { get; set; }
        
    }
}
