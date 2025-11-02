using Acme.ProductSelling.Users;
using System;
using Volo.Abp.Identity;

namespace Acme.ProductSelling.Identity
{
    public class AppUserDto : IdentityUserDto
    {
        public string ShippingAddress { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public UserGender Gender { get; set; }
    }
}
