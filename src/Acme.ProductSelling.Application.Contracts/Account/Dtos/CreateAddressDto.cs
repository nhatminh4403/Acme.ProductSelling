using Acme.ProductSelling.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Acme.ProductSelling.Account.Dtos
{
    public class CreateAddressDto
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(AddressConsts.MaxAddressLength)]
        public string FullAddress { get; set; }
    }
}
