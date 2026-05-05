using System;
using System.Collections.Generic;
using System.Text;

namespace Acme.ProductSelling.Account.Dtos
{
    public class UpdateShippingAddressDto : CreateAddressDto
    {
        public Guid Id { get; set; }
    }
}
