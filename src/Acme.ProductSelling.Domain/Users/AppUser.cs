using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Acme.ProductSelling.Users
{
    public class AppUser : IdentityUser, IUser
    {
        public string ShippingAddress { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public UserGender Gender { get; set; }
        protected AppUser() { }
    }
}
